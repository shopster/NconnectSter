//	Copyright 2010 Shopster E-Commerce Inc.
//
//	Licensed under the Apache License, Version 2.0 (the "License");
//	you may not use this file except in compliance with the License.
//	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
//	
//	Unless required by applicable law or agreed to in writing, software
//	distributed under the License is distributed on an "AS IS" BASIS,
//	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//	See the License for the specific language governing permissions and
//	limitations under the License.using System;

using System;
using System.Collections.Generic;
using System.Linq;
using Connectster.Server;
using Connectster.Server.Interfaces;
using Connectster.Shopster.Converters;
using log4net;
using Shopster.API.Service.SDK.Call;
using Shopster.API.Service.SDK.Core;
using Shopster.API.Service.SDK.Core.Exceptions;
using Shopster.API.Service.SDK.Core.Soap;

namespace Connectster.Shopster
{
    internal class ShopsterCommunicator
    {
        #region InventoryCategory Calls

        private static readonly ILog ApiLogger = LogManager.GetLogger("ShopsterAPICommunications");

        private static Dictionary<int, InventoryCategoryType> _categories;
                                                              //we store the categories here because they will be referenced often.

        private static readonly ShopsterCommunicator _instance = new ShopsterCommunicator();

        private ShopsterCommunicator()
        {
            _categories = new Dictionary<int, InventoryCategoryType>();
        }

        public static ShopsterCommunicator Instance()
        {
            return _instance;
        }

        /// <summary>
        /// Tries to get the category (either from the communicator's cache, or from API if not in cache)
        /// </summary>
        /// <param name="apiContext"></param>
        /// <param name="categoryId"></param>
        /// <returns>Category if successful, null otherwise.</returns>
        public InventoryCategoryType GetCategory(ApiContext apiContext, int categoryId)
        {
            if (_categories.Keys.Contains(categoryId))
            {
                return _categories[categoryId];
            }
            List<InventoryCategoryType> someCategories = GetCategoryFromApi(apiContext, categoryId);
            if ((someCategories != null && someCategories.Count != 0)) //if someCategories != null and !empty
            {
                foreach (InventoryCategoryType newCategory in someCategories)
                {
                    if (! _categories.Keys.ToList().Contains(Convert.ToInt32(newCategory.CategoryId)))
                    {
                        _categories.Add(Convert.ToInt32(newCategory.CategoryId), newCategory);
                    }
                }
            }

            //Depending on the reliability of the API, this maybe overkill, could just return categories[categoryId].
            return _categories.Keys.Contains(categoryId) ? _categories[categoryId] : null;
        }

        /// <summary>
        /// Attempts to get a category from the API, it makes no guarantees that it will succeed. 
        /// </summary>
        /// <param name="apiContext"></param>
        /// <param name="categoryId"></param>
        /// <returns>It will return a list of all categories it finds during the search. The list may or maynot contain the requested category.</returns>
        private List<InventoryCategoryType> GetCategoryFromApi(ApiContext apiContext, int categoryId)
        {
            var categories = new Dictionary<int, InventoryCategoryType>(50);
            int pageNum = 0;
            while (!categories.Keys.Contains(categoryId))
            {
                List<InventoryCategoryType> pageOfCategories = GetPageOfInventoryCategories(apiContext, pageNum++, 50);
                if (!(pageOfCategories != null && pageOfCategories.Count != 0))
                    //(if pageOfCategories == null or count==0)
                {
                    //Break out because we didnt get anything more back.
                    break;
                }

                //Union the pageOfCategories into the category cache.
                foreach (InventoryCategoryType newCat in pageOfCategories)
                {
                    categories.Add(Convert.ToInt32(newCat.CategoryId), newCat);
                }
            }
            return categories.Values.ToList();
        }

        private static List<InventoryCategoryType> GetPageOfInventoryCategories(ApiContext apiContext, int pageNum,
                                                                         int pageSize)
        {
            Dictionary<int, InventoryCategoryType> returnList = null;
            //const int pageSize = 50;
            try
            {
                var call = new GetInventoryCategoriesCall(apiContext)
                               {
                                   Request = new GetInventoryCategoriesRequestType
                                                 {
                                                     PageIndex = pageNum,
                                                     PageSize = pageSize
                                                 }
                               };
                call.Execute();

                if (call.Response.Status == ResponseStatusType.Failed)
                {
                    // Not sure what to do
                    return null;
                }

                //TODO : handle warnings
                if (call.Response.Status == ResponseStatusType.Success ||
                    call.Response.Status == ResponseStatusType.SuccessWithWarnings)
                {
                    returnList = new Dictionary<int, InventoryCategoryType>(call.Response.Categories.Count);
                    foreach (InventoryCategoryType category in call.Response.Categories)
                    {
                        returnList.Add(Convert.ToInt32(category.CategoryId), category);
                    }
                }
            }
            catch (ApiException ae)
            {
                string result = ae.Message;


                if (ae is ApiFaultException)
                {
                    var afe = (ApiFaultException) ae;
                    result = string.Format("({0}) {1}: {2}", afe.Error.SeverityCode, result,
                                           afe.Error.Message);
                    //TODO: Logging
                    ApiLogger.ErrorFormat(
                        "ShopsterCommunicator::GetPageOfInventoryCategories(): ApiFaultException: ({0}) {1}: {2}",
                        afe.Error.SeverityCode, result,
                        afe.Error.Message);
                }
                return null;
            }

            return returnList == null ? new List<InventoryCategoryType>(1) : returnList.Values.ToList();
        }


        /// <summary>
        /// Places an order on shopster. 
        /// </summary>
        /// <param name="apiContext"></param>
        /// <param name="orderToPlace"></param>
        /// <param name="productMap"></param>
        /// <returns>Null if failed, Shopster NewOrderId otherwise. </returns>
        public int? PlaceOrder(ApiContext apiContext, IOrder orderToPlace, Dictionary<int, int> productMap)
        {
            try
            {
                var call = new PlaceOrderCall(apiContext) {Request = new PlaceOrderRequestType()};
                string messageId = orderToPlace.GetHashCode().ToString(); //This should be unique enough right?
                call.Request.MessageID = messageId;

                call.Request.Cart = ShopsterOrderConverter.ToShopsterOrder(orderToPlace, productMap).orderCart;
                if (call.Request.Cart != null && call.Request.Cart.Items != null && call.Request.Cart.Items.Count > 0)
                {
                    call.Execute();

                    if (call.Response.Status == ResponseStatusType.Failed)
                    {
                        //Todo: Fix this hack, should still return null.
                        //Todo: Implement email/messaging in API to send something to the user.
                        ApiLogger.ErrorFormat(
                            "ShopsterCommunicator::PlaceOrder(): Reponse.Status is 'Failed' for ShopsterAccessToken({0}), ",
                            apiContext.AccessToken);
                        return null;
                    }

                    //TODO : handle warnings
                    if (call.Response.Status == ResponseStatusType.Success ||
                        call.Response.Status == ResponseStatusType.SuccessWithWarnings)
                    {
                        if (call.Response.MessageID != messageId || call.Response.NewOrderId == null)
                        {
//Should never happen in syncronous environment. 
                            ApiLogger.WarnFormat(
                                "ShopsterCommunicator::PlaceOrder(): Strange situation: API Call was assigned messageId({0}), but returned messageId({1}).",
                                messageId, call.Response.MessageID);
                        }

                        try
                        {
                            return Convert.ToInt32(call.Response.NewOrderId);
                        }
                        catch (Exception e)
                        {
                            ApiLogger.ErrorFormat(
                                "ShopsterCommunicator::PlaceOrder(): Couldnt convert call.Response.NewOrderId to int32. Exception is ({0})",
                                e.Message);
                            return null;
                        }
                    }
                }
            }
            catch (ApiException ae)
            {
                string result = ae.Message;


                if (ae is ApiFaultException)
                {
                    var afe = (ApiFaultException) ae;
                    result = string.Format("({0}) {1}: {2}", afe.Error.SeverityCode, result,
                                           afe.Error.Message);
                    //TODO: Logging
                    ApiLogger.ErrorFormat("ShopsterCommunicator::PlaceOrder(): ApiFaultException: ({0}) {1}: {2}",
                                          afe.Error.SeverityCode, result,
                                          afe.Error.Message);
                }
                return null;
            }
            return null;
        }

        #region InventoryItem Calls

        public List<InventoryItemType> GetAllInventoryItemsForUser(MyApiContext apiContext, string group)
        {
            if (apiContext == null)
            {
                throw new ArgumentException("apiContext may not be null");
            }


            switch (group)
            {
                case "Group1":
                case "Group2":
                case "Group3":
                case "Group4":
                case "Group5":
                case "All":
                    break;
                default:
                    group = "All"; //Just default, no need to fail. 
                    break;
            }


            const int pageSize = 50;
            var returnList = new Dictionary<int, InventoryItemType>();

            try
            {
                int pageNum = 0; //to keep our current page number

                var call = new GetInventoryItemsCall(apiContext) {Request = new GetInventoryItemsRequestType()};

                do
                {
                    call.Request.PageIndex = pageNum++;
                    call.Request.PageSize = pageSize;
                    call.Request.DetailGroups = group;
                    call.Execute();

                    if (call.Response.Status == ResponseStatusType.Failed)
                    {
                        // Not sure what to do
                        ApiLogger.DebugFormat(
                            "ShopsterCommunicator::GetAllInventoryItemsForUser(): call.Response.Status == Failed. PageNum({0}), PageSize({1}), Group({2})",
                            pageNum, pageSize, group);
                        return null;
                            //return null to indcate error, otherwise shopsterify will think a bunch of items have been deleted.
                    }

                    //TODO : handle warnings from shopsterAPI
                    if (call.Response.Status == ResponseStatusType.Success ||
                        call.Response.Status == ResponseStatusType.SuccessWithWarnings)
                    {
                        foreach (InventoryItemType item in call.Response.Items)
                        {
                            returnList.Add(Convert.ToInt32(item.ItemId), item);
                        }
                        if (call.Response.Status == ResponseStatusType.SuccessWithWarnings)
                        {
                            int i = 0;
                            foreach (var error in call.Response.Errors)
                            {
                                ApiLogger.WarnFormat(
                                    "ShopsterCommunicator::GetAllInventoryItemsForUser(): Warning/Error[{0}] for user({1}) : {2}",
                                    i, apiContext.AccessToken, error.Message);
                                i++;
                            }
                        }
                    }

                    //Repeat the call until we have enough items (==call.Response.NumFound) or we've gone through enough pages. 
                } while (
                    !(returnList.Count >= call.Response.NumFound || pageNum > ((call.Response.NumFound/pageSize) + 1)));

                return returnList.Count > 0 ? returnList.Values.ToList() : new List<InventoryItemType>(1);
                    //Return the list
            }
            catch (ApiException ae)
            {
                string result = ae.Message;


                if (ae is ApiFaultException)
                {
                    var afe = (ApiFaultException) ae;
                    result = string.Format("({0}) {1}: {2}", afe.Error.SeverityCode, result,
                                           afe.Error.Message);
                    ApiLogger.DebugFormat(
                        "ShopsterCommunicator::GetAllInventoryItemsForUser(): ApiFaultException: ({0}) {1}: {2}",
                        afe.Error.SeverityCode, result,
                        afe.Error.Message);
                }
                return null;
            }
            //Return in either try or catch.		
        }

        #endregion
    }

    #endregion
}