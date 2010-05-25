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
//	limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Connectster.Server.Database;
using Connectster.Shopify;
using Connectster.Shopster;
using log4net;
using Shopster.API.Service.SDK.Core;
using Shopster.API.Service.SDK.Core.Soap;

namespace Connectster.Server
{
    //Implements all the commands that can be done by Shopsterify
    public class ConnectsterController
    {

        private static readonly ConnectsterController _instance = new ConnectsterController();
        private static readonly ILog logger = LogManager.GetLogger("ConnectsterController");

        private readonly ConnectsterDatabase _database;
        private readonly ShopsterCommunicator _shopsterComm;
        private readonly ShopifyCommunicator _shopifyComm;
        
        private readonly ShopifyMetafield _metaFieldShopsterProductId;

        private ConnectsterController()
        {
            _database = new ConnectsterDatabase();
            _shopsterComm = ShopsterCommunicator.Instance();
            _shopifyComm = ShopifyCommunicator.Instance();

            //Todo: Change these namespaces, keys, valuetype into a config file entry.
            _metaFieldShopsterProductId = new ShopifyMetafield
                                              {
                                                  Namespace = "Connectster",
                                                  Key = "ShopsterProductId",
                                                  ValueType = "integer"
                                              };
        }

        public static ConnectsterController Instance()
        {
            return _instance;
        }

        public int DoSync(ConnectsterUser user) //here for easy calling
        {
            return SyncProducts(user) + SyncOrders(user);
        }

        public List<ConnectsterProduct> GetAllShopsterifyProductsForUser(ConnectsterUser user)
        {
            return _database.SelectProductForUser(user);
        }

        public ConnectsterUser GetUser(ApiContext apiContext, ShopifyStoreAuth shopifyAuth)
        {
            return _database.SelectConnectsterUser(apiContext, shopifyAuth);
        }


        /// <summary>
        /// Pushes all shopster products to shopify. Deals with all error states with an existing shopster Item. Deals with cases 1357, all ending in case7's state (see wiki).
        /// </summary>
        /// <param name="shopsterItems"></param>
        /// <param name="productMap"></param>
        /// <param name="shopifyMap"></param>
        /// <param name="shopifyIds"></param>
        /// <param name="shopifyItems"></param>
        /// <param name="storeAuth"></param>
        /// <param name="apiContext"></param>
        /// <returns>A count of how many products were fixed/changed.</returns>
        private int PushProductToShopify(IEnumerable<InventoryItemType> shopsterItems, ConnectsterProductMap productMap,
                                         ShopifyMetafieldMap shopifyMap, IList<int> shopifyIds,
                                         List<ShopifyProduct> shopifyItems, ShopifyStoreAuth storeAuth,
                                         ApiContext apiContext)
        {
            if (shopifyItems == null) throw new ArgumentNullException("shopifyItems");
            int returnCount = 0;
            //All of these are ending up in a Case7 (All good) state.
            foreach (InventoryItemType shopsterItem in shopsterItems)
            {
                InventoryItemType inventoryItem = shopsterItem;
                IList<ConnectsterProduct> mappings = productMap.GetMappedItems()
                    .Where(p => p.SourceId == Convert.ToInt32(inventoryItem.ItemId)).Select(p => p).ToList();
                if (mappings.Count > 0) //Cases 3,7
                {
                    foreach (ConnectsterProduct mapping in mappings)
                    {
                        if (!shopifyIds.Contains(mapping.DestinationId)) //If we have a broken mapping. (Case3)
                        {
                            //if the metafields have a mapping, fix the mapping 
                            if (shopifyMap.GetDestinationIds().Contains(Convert.ToInt32(shopsterItem.ItemId)))
                            {
                                //There is a shopifyItem that wants to be linked to this shopsterItem
                                if (shopifyItems.Count > 0)
                                {
                                    ShopifyProduct shopifyItem = shopifyItems
                                        .Where(item => item.Id ==
                                                       shopifyMap.GetSourcesForDestiation(
                                                           Convert.ToInt32(shopsterItem.ItemId)).Take(1).First())
                                        .Select(item => item).First();
                                    if (shopifyItem.Id != null) mapping.DestinationId = (int) shopifyItem.Id;
                                    productMap.AddMapping(mapping); //Add mapping will update if it is already there. 
                                    return returnCount;
                                }
                                
                            }
                            else
                            {
                                //upload the product

                                List<InventoryCategoryType> categories =
                                    (shopsterItem.Categories == null || shopsterItem.Categories.Count < 1)
                                        ? new List<InventoryCategoryType>(1)
                                        : new List<InventoryCategoryType>(shopsterItem.Categories.Count);

                                if (shopsterItem.Categories != null)
                                    categories.AddRange(shopsterItem.Categories.Select(categoryId => _shopsterComm.GetCategory(apiContext, categoryId)));


                                ShopifyProduct sP = ShopsterifyConverter.convertItem(shopsterItem, categories);
                                ShopifyResponse<ShopifyProduct> response = _shopifyComm.CreateProduct(storeAuth, sP);
                                if (response.State == ResponseState.OK)
                                {
                                    _metaFieldShopsterProductId.Value = Convert.ToInt32(shopsterItem.ItemId).ToString();
                                    ShopifyResponse<ShopifyMetafield> responseMetafield =
                                        _shopifyComm.CreateMetafield(storeAuth, _metaFieldShopsterProductId,
                                                                    response.ResponseObject.Id);
                                    if (responseMetafield == null) throw new NotImplementedException();

                                    mapping.DestinationId = response.ResponseObject.Id != null
                                                                ? (int) response.ResponseObject.Id
                                                                : 0;
                                    productMap.AddMapping(mapping); //Add mapping will update if it is already there. 
                                    returnCount++;
                                }
                            }
                        }
                        else //Case7 update and ensure metafield map is correct. 
                        {
                            //Todo enable the below comparison of Sourcedate and DestinationDate
                            //if (mapping.SourceDate > mapping.DestinationDate)

                            if (mapping.DestinationDate < DateTime.UtcNow.AddMinutes(-5))
                            {
                                List<InventoryCategoryType> categories =
                                    (shopsterItem.Categories == null || shopsterItem.Categories.Count < 1)
                                        ? new List<InventoryCategoryType>(1)
                                        : new List<InventoryCategoryType>(shopsterItem.Categories.Count);
                                if (shopsterItem.Categories != null)
                                    categories.AddRange(shopsterItem.Categories.Select(categoryId => _shopsterComm.GetCategory(apiContext, categoryId)));

                                ShopifyProduct shopifyProduct = ShopsterifyConverter.convertItem(shopsterItem,
                                                                                                 categories);

                                shopifyProduct.Id = productMap.GetProductTable()[Convert.ToInt32(shopsterItem.ItemId)];
                                ShopifyVariant[] shopifyVars =
                                    shopifyItems.Where(item => item.Id == shopifyProduct.Id).Select(
                                        item => item.Variants).FirstOrDefault();

                                foreach (ShopifyVariant sV in shopifyProduct.Variants)
                                {
                                    sV.ProductId = shopifyProduct.Id;
                                    if (shopifyVars != null) sV.Id = shopifyVars[0].Id;
                                }

                                ShopifyResponse<ShopifyProduct> response = _shopifyComm.UpdateProduct(storeAuth,
                                                                                                     shopifyProduct);
                                if (response.State == ResponseState.OK)
                                {
                                    //Todo add debug logging that this item was updated correctly
                                    returnCount++;
                                    logger.InfoFormat(
                                        "ConnectsterController:: Updated shopify item({0}) successfully.",
                                        shopifyProduct.Id);

                                    _database.UpdateShopifyProductTimeStamp(response.ResponseObject, DateTime.UtcNow);
                                }
                            }
                            else
                            {
                                logger.InfoFormat("ConnectsterController:: Skipped update due to timestamps.");
                            }
                        }
                    }
                }
                else //Cases 1, 5 -- Product exists, no mappings found, maybe on shopify
                {
                    InventoryItemType inventoryItemType = shopsterItem;
                    IList<ShopifyMetafieldMapping> metaFieldMappings = shopifyMap.GetMappedItems()
                        .Where(map => map.DestinationId == Convert.ToInt32(inventoryItemType.ItemId))
                        .Select(map => map).ToList();

                    if (metaFieldMappings.Count > 0)
                        //There are products on shopify that think they belong to this shopsteritem
                    {
                        //Case 5
                        foreach (ShopifyMetafieldMapping metaFieldMapping in metaFieldMappings)
                        {
                            //rebuild the mapping, this call pushes through to the db.
                            ShopifyMetafieldMapping mapping = metaFieldMapping;
                            InventoryItemType item1 = shopsterItem;
                            productMap.AddMapping(new ConnectsterProduct(shopsterItem, shopifyItems
                                                                                           .Where(
                                                                                               item =>
                                                                                               (item.Id ==
                                                                                                mapping.
                                                                                                    SourceId) &&
                                                                                               (mapping.
                                                                                                    DestinationId ==
                                                                                                Convert.ToInt32(
                                                                                                    item1.ItemId)))
                                                                                           .Select(item => item).Take(1)
                                                                                           .ToList()[0]));
                            returnCount++;
                        }
                    }
                    else //There aren't any products on shopify that belong to this shopsterItem
                    {
                        //Case 1
                        //upload the product
                        List<InventoryCategoryType> categories =
                            (shopsterItem.Categories == null || shopsterItem.Categories.Count < 1)
                                ? new List<InventoryCategoryType>(1)
                                : new List<InventoryCategoryType>(shopsterItem.Categories.Count);

                        if (shopsterItem.Categories != null)
                            categories.AddRange(shopsterItem.Categories.Select(categoryId => _shopsterComm.GetCategory(apiContext, categoryId)));

                        ShopifyProduct sP = ShopsterifyConverter.convertItem(shopsterItem, categories);
                        ShopifyResponse<ShopifyProduct> response = _shopifyComm.CreateProduct(storeAuth, sP);
                        if (response.State == ResponseState.OK)
                        {
                            _metaFieldShopsterProductId.Value = Convert.ToInt32(shopsterItem.ItemId).ToString();
                            ShopifyResponse<ShopifyMetafield> responseMetafield = _shopifyComm.CreateMetafield(
                                storeAuth, _metaFieldShopsterProductId, response.ResponseObject.Id);
                            if (responseMetafield.State == ResponseState.OK)
                            {
                                //create the mapping 

                                var mapping = new ConnectsterProduct(shopsterItem, response.ResponseObject);
                                productMap.AddMapping(mapping);
                            }


                            returnCount++;
                        }
                    }
                }
            }

            return returnCount;
        }

        //End of PushProductToShopify


        /// <summary>
        /// Deals with any orphaned cases with a Mapped remaining. Attempts to clean up any shopify Items in the process.
        /// </summary>
        /// <param name="shopsterIds"></param>
        /// <param name="productMap"></param>
        /// <param name="shopifyMapping"></param>
        /// <param name="shopifyIds"></param>
        /// <param name="shopifyAuth"></param>
        /// <returns>Number of "items" affected</returns>
        private int FixOrphanedMappings(IList<int> shopsterIds, ConnectsterProductMap productMap,
                                        ShopifyMetafieldMap shopifyMapping, IList<int> shopifyIds,
                                        ShopifyStoreAuth shopifyAuth)
        {
            int returnCount = 0;

            //Foreach productMap for which there is no corresponding shopsterItem.
            // Delete the map, and if applicable delete the shopifyItem.
            foreach (ConnectsterProduct shopsterifyProduct in productMap.GetMappedItems()
                .Where(p => !shopsterIds.Contains(p.SourceId)).Select(p => p).ToList())
            {
                //case 2
                if (!shopifyIds.Contains(shopsterifyProduct.DestinationId))
                {
                    productMap.DeleteMapping(shopsterifyProduct);
                    returnCount++;
                }
                else //case 6
                {
                    if (DeleteProductAndDeleteMap(shopifyAuth, productMap, shopsterifyProduct))
                    {
                        shopifyIds.Remove(shopsterifyProduct.DestinationId);
                        ConnectsterProduct product = shopsterifyProduct;
                        ShopifyMetafieldMapping toRemove =
                            shopifyMapping.GetMappedItems().Where(
                                map => map.SourceId == product.DestinationId).Select(map => map).
                                FirstOrDefault();
                        shopifyMapping.GetMappedItems().Remove(toRemove);
                        returnCount++;
                    }
                }
            }
            return returnCount;
        }

        /// <summary>
        /// Deletes completely orphaned ShopifyProducts (one's which are tagged) but not related to an existing shopsteritem or a mapping.
        /// </summary>
        /// <param name="shopsterItems"></param>
        /// <param name="productMap"></param>
        /// <param name="shopifyMapping"></param>
        /// <param name="shopifyAuth"></param>
        /// <returns></returns>
        private int DeleteOrphanedShopifyProducts(IEnumerable<InventoryItemType> shopsterItems,
                                                  ConnectsterProductMap productMap, ShopifyMetafieldMap shopifyMapping,
                                                  ShopifyStoreAuth shopifyAuth)
        {
            //DeleteOrphanedShopifyProducts(productMap, shopifyMapping, user, shopifyAuth)
            List<ShopifyMetafieldMapping> notMappedProductsList = shopifyMapping.GetMappedItems()
                .Where(item => (!productMap.GetDestinationIds().Contains(item.SourceId))).Select(item => item).ToList();

            return notMappedProductsList.Where(notMappedProduct => shopsterItems.Where(item => Convert.ToInt32(item.ItemId) == notMappedProduct.DestinationId).Count() == 0).Count(notMappedProduct => _shopifyComm.DeleteProduct(shopifyAuth, notMappedProduct.SourceId));
        }


        private int SyncProducts(ConnectsterUser user)
        {
            int returnCount = 0;

            //Ask the db for credentials
            ShopifyStoreAuth shopifyAuth = _database.SelectShopifyUserDetails(user);
            ShopsterUser shopsterUser = _database.SelectShopsterUserDetails(user);

            var apiContext = new MyApiContext
                                 {
                                     AccessToken = shopsterUser.AuthToken,
                                     AccessTokenSecret = shopsterUser.AuthSecret
                                 };

            List<InventoryItemType> shopsterList;
            ConnectsterProductMap productMap;
            List<ShopifyProduct> shopifyItems;
            ShopifyMetafieldMap shopifyMapping;

            //Todo: Decide the lowest DetailGroup needed
            try
            {
                shopsterList =
                    _shopsterComm.GetAllInventoryItemsForUser(apiContext, "All").Where(
                        item => item.IsStoreVisible == true).Select(item => item).ToList();
                productMap = new ConnectsterProductMap(user);
                shopifyItems = _shopifyComm.GetAllProducts(shopifyAuth);
                shopifyMapping = new ShopifyMetafieldMap(_shopifyComm, shopifyAuth);
            }
            catch (Exception e)
            {
                logger.FatalFormat(
                    "ConnectsterController::SyncProducts(): Exception while creating lists. Exception is ({0}).",
                    e.Message);
                return 0;
                //todo : throw our own exception class to the program to indicate the user should be disabled or ignored for a while.
            }


            if (shopifyItems == null)
            {
                //todo : again we need to throw our own exception class to indicate the user should be disabled or ignored for a while.
                return 0;
            }

            //create these lists here to reduce redundancy in proceeding calls.
            List<int> shopsterIds = shopsterList.Select(item => Convert.ToInt32(item.ItemId)).ToList();
            List<int> shopifyIds = shopifyItems.Select(item => (int) item.Id).ToList();
            _database.UpdateShopsterProductTimeStamps(shopsterList, DateTime.UtcNow);


#if DEBUG
            //Some debugging info 
            foreach (ConnectsterProduct mappedItem in productMap.GetMappedItems())
            {
                logger.DebugFormat(
                    "ConnectsterController::CreateListsAndBegin(): ShopsterifyDB has ShopsterId({0}, Date({1:yyyy-MM-dd HH:mm:ss}) \tMapped to \t ShopifyId({2}, Date({3:yyyy-MM-dd HH:mm:ss})",
                    mappedItem.SourceId, mappedItem.SourceDate, mappedItem.DestinationId, mappedItem.DestinationDate);
            }

            foreach (ShopifyProduct item in shopifyItems)
            {
                logger.DebugFormat(
                    "ConnectsterController::CreateListsAndBegin(): Shopify has shopifyItem({0}, Date:{1: yyyy-MM-dd HH:mm:ss})",
                    item.Id, ((DateTime) item.Variants[0].UpdatedAt));
            }

            foreach (ShopifyMetafieldMapping mapping in shopifyMapping.GetMappedItems())
            {
                logger.DebugFormat("Shopify has Item({0}) --> ShopsterId({1})", mapping.SourceId, mapping.DestinationId);
            }
#endif

            //Now do the actual syncronizing actions

            //This will ensure that all shopster products are in the db and on shopify
            returnCount += PushProductToShopify(shopsterList, productMap, shopifyMapping, shopifyIds, shopifyItems,
                                                shopifyAuth, apiContext);

            //This will clean up all the broken Maps
            returnCount += FixOrphanedMappings(shopsterIds, productMap, shopifyMapping, shopifyIds, shopifyAuth);

            //This will delete all orphaned shopify items (not mapped at all, but tagged as shopster items).
            return (returnCount +
                    DeleteOrphanedShopifyProducts(shopsterList, productMap, shopifyMapping, shopifyAuth));
        }

        private bool DeleteProductAndDeleteMap(ShopifyStoreAuth shopifyAuth, ConnectsterProductMap productMap,
                                               ConnectsterProduct item)
        {
            //Attempt to delete the shopsterItem from shopify
            if (!_shopifyComm.DeleteProduct(shopifyAuth, item.DestinationId))
            {
                //maybe a communication error, try next time around
                //todo: error, logs
                return false;
            }

            //Delete the Mapping
            return (productMap.DeleteMapping(item));
        }

        //TODO : Make shopsterify user contain apiContext and shopifyAuth objects, then code that takes both a ApiContext and ShopifyStoreAuth can just take a shopsterifyUser
        private bool CreateProductAndMap(ApiContext apiContext, ShopifyStoreAuth shopifyAuth,
                                         InventoryItemType shopsterItem, ShopifyMetafieldMap shopifyMapping)
        {
            //Steps are: Upload Item to Shopify, Create metafield, Update map according to new productId

            //Upload to shopify
            List<InventoryCategoryType> categories =
                (shopsterItem.Categories == null || shopsterItem.Categories.Count < 1)
                    ? new List<InventoryCategoryType>(1)
                    : new List<InventoryCategoryType>(shopsterItem.Categories.Count);
            if (shopsterItem.Categories != null)
                categories.AddRange(shopsterItem.Categories.Select(categoryId => _shopsterComm.GetCategory(apiContext, categoryId)));

            //get the categories for this item

            ShopifyResponse<ShopifyProduct> response = _shopifyComm.CreateProduct(shopifyAuth,
                                                                                 ShopsterifyConverter.convertItem(
                                                                                     shopsterItem, categories));
            if (response.State == ResponseState.OK)
            {
                _metaFieldShopsterProductId.Value = shopsterItem.ItemId;
                if (response.ResponseObject.Id != null)
                {
                    if (response.ResponseObject.Created != null)
                    {
                        ShopifyMetafieldMapping newMap =
                            ShopifyMetafieldMapping.CreateMetafieldMapping((int) response.ResponseObject.Id,
                                                                           (DateTime) response.ResponseObject.Created,
                                                                           _metaFieldShopsterProductId);

                        //Update map (add mapping will create/update the ShopifyMetafield on shopify)
                        if (shopifyMapping.AddMapping(newMap))
                        {
#if DEBUG
                            logger.DebugFormat(
                                "ConnectsterController::CreateProductAndMap(): CreatedProduct({0}) and mapping on Shopify.",
                                response.ResponseObject.Id);
#endif
                            return true;
                        }
                    }
                }
            }
#if DEBUG
            logger.DebugFormat(
                "ConnectsterController::CreateProductAndMap(): attempted to create mapping and failed. ShopsterToken({0}), ShopifySubdomain({1}, shopsterItemId({2})",
                apiContext.AccessToken, shopifyAuth.StoreSubDomain, shopsterItem.ItemId);
#endif
            return false;
        }

        internal List<ConnectsterUser> GetAllUsers()
        {
            return _database.SelectAllUsers();
        }


        private int SyncOrders(ConnectsterUser user)
        {
            int returnCount = 0;

            //Ask the db for credentials
            ShopifyStoreAuth shopifyAuth = _database.SelectShopifyUserDetails(user);
            ShopsterUser shopsterUser = _database.SelectShopsterUserDetails(user);

            var apiContext = new MyApiContext
                                 {
                                     AccessToken = shopsterUser.AuthToken,
                                     AccessTokenSecret = shopsterUser.AuthSecret
                                 };

            List<ShopifyOrder> shopifyOrders = _shopifyComm.GetListAllOrders(shopifyAuth);

            //Think of this dictionary as Dictionary<ShopifyOrderId, ShopsterOrderId>
            Dictionary<int, int> shopsterifyOrderMap = _database.SelectOrderMappingsForUser(user);
            List<ConnectsterProduct> productMappingsList = _database.SelectProductForUser(user);
            var productDictionary = new Dictionary<int, int>(productMappingsList.Count);
            foreach (ConnectsterProduct item in productMappingsList)
            {
                productDictionary.Add(item.SourceId, item.DestinationId);
            }

            if (shopsterifyOrderMap == null || shopifyOrders == null)
            {
                //Todo Logging and fail
                return 0;
            }

            foreach (ShopifyOrder shopifyOrder in shopifyOrders)
            {
                if (shopsterifyOrderMap.Keys.Contains(shopifyOrder.Id))
                {
                    continue; //Skip this order, it already has been processed by shopsterify
                }

                int? newOrder;
                if ((newOrder = _shopsterComm.PlaceOrder(apiContext, shopifyOrder, productDictionary)) != null)
                {
                    returnCount++;
                    if (_database.CreateOrderMapping(user, shopifyOrder, (int) newOrder))
                    {
                    }
                }
            }


            return returnCount;
        }
    }
}