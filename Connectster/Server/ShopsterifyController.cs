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
using Shopster.API.Service.SDK.Core;
using Shopster.API.Service.SDK.Core.Soap;
using log4net;

namespace Connectster.Server
{
	//Implements all the commands that can be done by Shopsterify
	public class ShopsterifyController
	{
		
		private static ShopsterifyDatabase database;
		private static ShopsterifyController instance = new ShopsterifyController();
		private static ShopsterCommunicator shopsterComm;
		private static ShopifyCommunicator shopifyComm;
		private ShopifyMetafield metaField_ShopsterProductId;
	
		private static ILog logger = log4net.LogManager.GetLogger("ShopsterifyController");


		private ShopsterifyController()
		{
			database = new ShopsterifyDatabase();
			shopsterComm = ShopsterCommunicator.Instance();
			shopifyComm = ShopifyCommunicator.Instance();


			//Todo: Change these namespaces, keys, valuetype into a config file entry.
			metaField_ShopsterProductId = new ShopifyMetafield();
			metaField_ShopsterProductId.Namespace = "Shopsterify";
			metaField_ShopsterProductId.Key = "ShopsterProductId";
			metaField_ShopsterProductId.ValueType = "integer";
		}

		public static ShopsterifyController Instance()
		{
			return instance;
		}

		public int DoSync(ShopsterifyUser user) //here for easy calling
		{
			return SyncProducts(user) + SyncOrders(user);
		}

		public List<ShopsterifyProduct> GetAllShopsterifyProductsForUser(ShopsterifyUser user)
		{
			return database.SelectProductForUser(user);
		}

		public ShopsterifyUser GetUser(ApiContext apiContext, ShopifyStoreAuth shopifyAuth)
		{
			return database.SelectShopsterifyUser(apiContext, shopifyAuth);
		}

		
		/// <summary>
		/// Pushes all shopster products to shopify. Deals with all error states with an existing shopster Item. Deals with cases 1357, all ending in case7's state (see wiki).
		/// </summary>
		/// <param name="shopsterItems"></param>
		/// <param name="productMap"></param>
		/// <param name="shopifyMap"></param>
		/// <param name="shopifyIds"></param>
		/// <param name="storeAuth"></param>
		/// <param name="user"></param>
		/// <returns>A count of how many products were fixed/changed.</returns>
		/// 
		private int PushProductToShopify(List<InventoryItemType> shopsterItems, ShopsterifyProductMap productMap, ShopifyMetafieldMap shopifyMap, IList<int> shopifyIds, List<ShopifyProduct> shopifyItems,  ShopifyStoreAuth storeAuth, ApiContext apiContext, ShopsterifyUser user)
		{
			int returnCount = 0;
			//All of these are ending up in a Case7 (All good) state.
			foreach (InventoryItemType shopsterItem in shopsterItems)
			{
				IList<ShopsterifyProduct> mappings = productMap.GetMappedItems()
					.Where(p => p.SourceId == Convert.ToInt32(shopsterItem.ItemId)).Select(p=>p).ToList();
				if (mappings.Count>0) //Cases 3,7
				{
					foreach (ShopsterifyProduct mapping in mappings) 
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
		shopifyMap.GetSourcesForDestiation(Convert.ToInt32(shopsterItem.ItemId)).Take(1).First())
	.Select(item => item).First();
									mapping.DestinationId = (int)shopifyItem.Id;
									productMap.AddMapping(mapping); //Add mapping will update if it is already there. 
									return returnCount++;
								}
								else
								{
									//this is a wierd situation, there are mappings, but no items found on shopify. Probably a communication error of sorts
								}


							}
							else
							{

								//upload the product
								
								List<InventoryCategoryType> categories = 
									 (shopsterItem.Categories == null || shopsterItem.Categories.Count<1 ) ? 
									 new List<InventoryCategoryType>(1) : new List<InventoryCategoryType>(shopsterItem.Categories.Count);

								foreach (int categoryId in shopsterItem.Categories)
								{
									categories.Add(shopsterComm.GetCategory(apiContext, categoryId));
								}
		

								ShopifyProduct sP = ShopsterifyConverter.convertItem(shopsterItem, categories);
								var response = shopifyComm.CreateProduct(storeAuth, sP);
								if (response.State == ResponseState.OK)
								{
									metaField_ShopsterProductId.Value = Convert.ToInt32(shopsterItem.ItemId).ToString();
									var responseMetafield = shopifyComm.CreateMetafield(storeAuth, metaField_ShopsterProductId, (int)response.ResponseObject.Id);
									
									mapping.DestinationId = response.ResponseObject.Id != null ? (int)response.ResponseObject.Id : 0;
									productMap.AddMapping(mapping); //Add mapping will update if it is already there. 
									returnCount++;
									
								}
							}
						}
						else //Case7 update and ensure metafield map is correct. 
						{
							//Todo enable the below comparison of Sourcedate and DestinationDate
							//if (mapping.SourceDate > mapping.DestinationDate)

							if(mapping.DestinationDate < DateTime.UtcNow.AddMinutes(-5))
							{ 
								List<InventoryCategoryType> categories =
										 (shopsterItem.Categories == null || shopsterItem.Categories.Count < 1) ?
										 new List<InventoryCategoryType>(1) : new List<InventoryCategoryType>(shopsterItem.Categories.Count);

								foreach (int categoryId in shopsterItem.Categories)
								{

									categories.Add(shopsterComm.GetCategory(apiContext, categoryId));
								}

								ShopifyProduct shopifyProduct = ShopsterifyConverter.convertItem(shopsterItem, categories);

								shopifyProduct.Id = productMap.GetProductTable()[Convert.ToInt32(shopsterItem.ItemId)];
								ShopifyVariant[] shopifyVars = shopifyItems.Where(item => item.Id == shopifyProduct.Id).Select(item => item.Variants).FirstOrDefault();

								foreach (ShopifyVariant sV in shopifyProduct.Variants)
								{
									sV.ProductId = shopifyProduct.Id;
									sV.Id = shopifyVars[0].Id;
								}

								var response = shopifyComm.UpdateProduct(storeAuth, shopifyProduct);
								if (response.State == ResponseState.OK)
								{
									//Todo add debug logging that this item was updated correctly
									returnCount++;
									logger.InfoFormat("ShopsterifyController:: Updated shopify item({0}) successfully.", shopifyProduct.Id);

									database.UpdateShopifyProductTimeStamp((ShopifyProduct)response.ResponseObject, DateTime.UtcNow);

								}
							}
							else
							{
								logger.InfoFormat("ShopsterifyController:: Skipped update due to timestamps.");
							}
							

						}

					}
					
				}
				else //Cases 1, 5 -- Product exists, no mappings found, maybe on shopify
				{
					IList<ShopifyMetafieldMapping> metaFieldMappings = shopifyMap.GetMappedItems()
						.Where(map => map.DestinationId == Convert.ToInt32(shopsterItem.ItemId))
						.Select(map => map).ToList();
					
						if (metaFieldMappings.Count > 0) //There are products on shopify that think they belong to this shopsteritem
						{ //Case 5
							foreach (ShopifyMetafieldMapping metaFieldMapping in metaFieldMappings)
							{
								//rebuild the mapping, this call pushes through to the db.
								productMap.AddMapping( new ShopsterifyProduct(shopsterItem, shopifyItems
									.Where(item=> (item.Id == metaFieldMapping.SourceId) && (metaFieldMapping.DestinationId == Convert.ToInt32(shopsterItem.ItemId)))
									.Select(item=>item).Take(1).ToList()[0]));
								returnCount++;
							}
						}
						else //There aren't any products on shopify that belong to this shopsterItem
						{ //Case 1
							//upload the product
							List<InventoryCategoryType> categories =
									 (shopsterItem.Categories == null || shopsterItem.Categories.Count < 1) ?
									 new List<InventoryCategoryType>(1) : new List<InventoryCategoryType>(shopsterItem.Categories.Count);

							foreach (int categoryId in shopsterItem.Categories)
							{
								categories.Add(shopsterComm.GetCategory(apiContext, categoryId));
							}

							ShopifyProduct sP = ShopsterifyConverter.convertItem(shopsterItem, categories);
							var response = shopifyComm.CreateProduct(storeAuth, sP);
							if (response.State == ResponseState.OK)
							{
								metaField_ShopsterProductId.Value = Convert.ToInt32(shopsterItem.ItemId).ToString() ;
								var responseMetafield = shopifyComm.CreateMetafield(storeAuth, metaField_ShopsterProductId, (int)response.ResponseObject.Id);
								if (responseMetafield.State == ResponseState.OK)
								{ //create the mapping 

									ShopsterifyProduct mapping = new ShopsterifyProduct(shopsterItem, response.ResponseObject);
									productMap.AddMapping(mapping); 
									
								}

								
								returnCount++;
							}


						}

				}

			}

			return returnCount;
		} //End of PushProductToShopify


		/// <summary>
		/// Deals with any orphaned cases with a Mapped remaining. Attempts to clean up any shopify Items in the process.
		/// </summary>
		/// <param name="shopsterIds"></param>
		/// <param name="productMap"></param>
		/// <param name="shopifyMapping"></param>
		/// <param name="shopifyIds"></param>
		/// <param name="shopifyAuth"></param>
		/// <param name="user"></param>
		/// <returns>Number of "items" affected</returns>
		private int FixOrphanedMappings(IList<int> shopsterIds, ShopsterifyProductMap productMap, ShopifyMetafieldMap shopifyMapping, IList<int> shopifyIds,  ShopifyStoreAuth shopifyAuth, ShopsterifyUser user)
		{
			int returnCount = 0;
			
			//Foreach productMap for which there is no corresponding shopsterItem.
			// Delete the map, and if applicable delete the shopifyItem.
			foreach (ShopsterifyProduct shopsterifyProduct in productMap.GetMappedItems()
				.Where(p => !shopsterIds.Contains(p.SourceId)).Select(p => p).ToList())
			{
			
				//case 2
				if(!shopifyIds.Contains(shopsterifyProduct.DestinationId))
				{
					productMap.DeleteMapping(shopsterifyProduct);
					returnCount++;
				}
				else //case 6
				{
					if(DeleteProductAndDeleteMap(shopifyAuth, productMap, shopsterifyProduct))
					{
						shopifyIds.Remove(shopsterifyProduct.DestinationId);
						ShopifyMetafieldMapping toRemove = shopifyMapping.GetMappedItems().Where(map => map.SourceId ==shopsterifyProduct.DestinationId).Select(map => map).FirstOrDefault();
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
		/// <param name="user"></param>
		/// <returns></returns>
		private int DeleteOrphanedShopifyProducts(List<InventoryItemType> shopsterItems, ShopsterifyProductMap productMap, ShopifyMetafieldMap shopifyMapping, ShopifyStoreAuth shopifyAuth, ShopsterifyUser user)
		{
			//DeleteOrphanedShopifyProducts(productMap, shopifyMapping, user, shopifyAuth)
			int returnCount = 0;
			List<ShopifyMetafieldMapping> notMappedProductsList = shopifyMapping.GetMappedItems()
				.Where(item => (!productMap.GetDestinationIds().Contains(item.SourceId))).Select(item => item).ToList<ShopifyMetafieldMapping>();

			foreach (ShopifyMetafieldMapping notMappedProduct in notMappedProductsList)
			{
				//If there doesnt exist a shopster shopsterItem, which this metafield map is pointing to.
				if (shopsterItems.Where(item => Convert.ToInt32(item.ItemId) == notMappedProduct.DestinationId).Count() == 0)
				{
					//Delete product from shopify
					if(shopifyComm.DeleteProduct(shopifyAuth, notMappedProduct.SourceId))
					{
						returnCount++;
					}

				}
			}
			return returnCount;
		}

		
		private int SyncProducts(ShopsterifyUser user)
		{
			int returnCount = 0;

			//Ask the db for credentials
			ShopifyStoreAuth shopifyAuth = database.SelectShopifyUserDetails(user);
			ShopsterUser shopsterUser = database.SelectShopsterUserDetails(user);

			MyApiContext apiContext = new MyApiContext();
			apiContext.AccessToken = shopsterUser.AuthToken;
			apiContext.AccessTokenSecret = shopsterUser.AuthSecret;

			List<InventoryItemType> shopsterList = null;
			ShopsterifyProductMap productMap = null;
			List<ShopifyProduct> shopifyItems = null;
			ShopifyMetafieldMap shopifyMapping = null;

			//Todo: Decide the lowest DetailGroup needed
			try
			{
				shopsterList = shopsterComm.GetAllInventoryItemsForUser(apiContext, "All").Where(item =>item.IsStoreVisible == true).Select(item => item).ToList<InventoryItemType>();
				productMap = new ShopsterifyProductMap(user);
				shopifyItems = shopifyComm.GetAllProducts(shopifyAuth);
				shopifyMapping = new ShopifyMetafieldMap(shopifyComm, shopifyAuth);
			}
			catch (Exception e)
			{
				
				logger.FatalFormat("ShopsterifyController::SyncProducts(): Exception while creating lists. Exception is ({0}).", e.Message);
				return 0;
				//todo : throw our own exception class to the program to indicate the user should be disabled or ignored for a while.
			}



			if (shopsterList == null || productMap == null || shopifyItems == null || shopifyMapping == null)
			{
				//todo : again we need to throw our own exception class to indicate the user should be disabled or ignored for a while.
				return 0;
			}

			//create these lists here to reduce redundancy in proceeding calls.
			List<int> shopsterIds = shopsterList.Select(item => Convert.ToInt32(item.ItemId)).ToList<int>();
			List<int> shopifyIds = shopifyItems.Select(item => (int)item.Id).ToList<int>();
			database.UpdateShopsterProductTimeStamps(shopsterList, DateTime.UtcNow);
			

#if DEBUG //Some debugging info 
			foreach (ShopsterifyProduct mappedItem in productMap.GetMappedItems())
			{
				logger.DebugFormat("ShopsterifyController::CreateListsAndBegin(): ShopsterifyDB has ShopsterId({0}, Date({1:yyyy-MM-dd HH:mm:ss}) \tMapped to \t ShopifyId({2}, Date({3:yyyy-MM-dd HH:mm:ss})", mappedItem.SourceId, mappedItem.SourceDate, mappedItem.DestinationId, mappedItem.DestinationDate);
			}

			foreach (ShopifyProduct item in shopifyItems)
			{
				logger.DebugFormat("ShopsterifyController::CreateListsAndBegin(): Shopify has shopifyItem({0}, Date:{1: yyyy-MM-dd HH:mm:ss})", item.Id, ((DateTime)item.Variants[0].UpdatedAt));
			}
			
			foreach (ShopifyMetafieldMapping mapping in shopifyMapping.GetMappedItems())
			{
			  logger.DebugFormat("Shopify has Item({0}) --> ShopsterId({1})", mapping.SourceId, mapping.DestinationId);
			}
#endif

			//Now do the actual syncronizing actions
		
			//This will ensure that all shopster products are in the db and on shopify
			returnCount += PushProductToShopify(shopsterList, productMap, shopifyMapping, shopifyIds, shopifyItems, shopifyAuth, apiContext, user);

			//This will clean up all the broken Maps
			returnCount += FixOrphanedMappings(shopsterIds, productMap, shopifyMapping, shopifyIds, shopifyAuth, user);	
			
			//This will delete all orphaned shopify items (not mapped at all, but tagged as shopster items).
			return (returnCount + DeleteOrphanedShopifyProducts(shopsterList, productMap, shopifyMapping, shopifyAuth, user));
			
		}

		private bool DeleteProductAndDeleteMap(ShopifyStoreAuth shopifyAuth, ShopsterifyProductMap productMap, ShopsterifyProduct item)
		{
			//Attempt to delete the shopsterItem from shopify
			if(!shopifyComm.DeleteProduct(shopifyAuth, item.DestinationId))
			{
				//maybe a communication error, try next time around
				//todo: error, logs
				return false;
			}

			//Delete the Mapping
			return(productMap.DeleteMapping(item));
		}

		//TODO : Make shopsterify user contain apiContext and shopifyAuth objects, then code that takes both a ApiContext and ShopifyStoreAuth can just take a shopsterifyUser
		private bool CreateProductAndMap(ApiContext apiContext, ShopifyStoreAuth shopifyAuth, InventoryItemType shopsterItem, ShopifyMetafieldMap shopifyMapping)
		{
			//Steps are: Upload Item to Shopify, Create metafield, Update map according to new productId

			//Upload to shopify
			List<InventoryCategoryType> categories =
									 (shopsterItem.Categories == null || shopsterItem.Categories.Count < 1) ?
									 new List<InventoryCategoryType>(1) : new List<InventoryCategoryType>(shopsterItem.Categories.Count);

			//get the categories for this item
			foreach (int categoryId in shopsterItem.Categories)
			{
				categories.Add(shopsterComm.GetCategory(apiContext, categoryId));
			}
						
			var response = shopifyComm.CreateProduct(shopifyAuth, ShopsterifyConverter.convertItem(shopsterItem, categories));
			if (response.State == ResponseState.OK)
			{
				metaField_ShopsterProductId.Value = shopsterItem.ItemId;
				ShopifyMetafieldMapping newMap = ShopifyMetafieldMapping.CreateMetafieldMapping((int)response.ResponseObject.Id, (DateTime)response.ResponseObject.Created, metaField_ShopsterProductId);
			
				//Update map (add mapping will create/update the ShopifyMetafield on shopify)
				if (shopifyMapping.AddMapping(newMap))
				{
#if DEBUG
					logger.DebugFormat("ShopsterifyController::CreateProductAndMap(): CreatedProduct({0}) and mapping on Shopify.",response.ResponseObject.Id);
#endif
					return true;
				}
			}
#if DEBUG
			logger.DebugFormat("ShopsterifyController::CreateProductAndMap(): attempted to create mapping and failed. ShopsterToken({0}), ShopifySubdomain({1}, shopsterItemId({2})", apiContext.AccessToken, shopifyAuth.StoreSubDomain, shopsterItem.ItemId);
#endif			
			return false;
	}	

		internal List<ShopsterifyUser> getAllUsers()
		{
			return database.SelectAllUsers();
		}

	
		
		private int SyncOrders(ShopsterifyUser user)
		{

			int returnCount = 0;
			
			//Ask the db for credentials
			ShopifyStoreAuth shopifyAuth = database.SelectShopifyUserDetails(user);
			ShopsterUser shopsterUser = database.SelectShopsterUserDetails(user);

			MyApiContext apiContext = new MyApiContext();
			apiContext.AccessToken = shopsterUser.AuthToken;
			apiContext.AccessTokenSecret = shopsterUser.AuthSecret;

			List<ShopifyOrder> shopifyOrders = shopifyComm.GetListAllOrders(shopifyAuth);
			
			//Think of this dictionary as Dictionary<ShopifyOrderId, ShopsterOrderId>
			Dictionary<int, int> shopsterifyOrderMap = database.SelectOrderMappingsForUser(user);
			List<ShopsterifyProduct> productMappingsList = database.SelectProductForUser(user);
			Dictionary<int, int> productDictionary = new Dictionary<int, int>(productMappingsList.Count);
			foreach (ShopsterifyProduct item in productMappingsList)
			{
				productDictionary.Add(item.SourceId, item.DestinationId);
			}

			if (shopsterifyOrderMap == null || shopifyOrders==null)
			{
				//Todo Logging and fail
				return 0;
			}

			int? newOrder= null;
			
			foreach (ShopifyOrder shopifyOrder in shopifyOrders)
			{

				if (shopsterifyOrderMap.Keys.Contains(shopifyOrder.Id))
				{
					continue; //Skip this order, it already has been processed by shopsterify
				}

				if ((newOrder = shopsterComm.PlaceOrder(apiContext, shopifyOrder, productDictionary)) != null)
				{
					returnCount++;
					if (database.CreateOrderMapping(user, shopifyOrder,(int) newOrder))
					{

					}

				}

			}


			return returnCount;

		}
	}
}
