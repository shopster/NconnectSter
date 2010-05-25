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
using Connectster.Shopify;

namespace Connectster.Server
{
	//TODO: can this class and ShopsterifyProduct map inherit alot of actions from e parent class?
	public class ShopifyMetafieldMap : ProductMap<ShopifyMetafieldMapping>
	{

		static int maxRefreshInMinutes = 5;
		private ShopifyCommunicator shopifyConn;
		private ShopifyStoreAuth storeAuth;

		public ShopifyMetafieldMap(ShopifyCommunicator connection, ShopifyStoreAuth storeAuth)
		{
			shopifyConn = connection;
			this.storeAuth = storeAuth;
			RefreshProductMap();
		}

		/// <summary>
		/// This will contact shopify and get all of the metafields. Use sparingly.
		/// </summary>
		/// <returns></returns>
		public override bool RefreshProductMap()
		{
			
			//Refuse to update faster than every maxRefreshInMinutes minutes
			if (CacheDateStamp != DateTime.MinValue && ((DateTime.Now-CacheDateStamp).Minutes < maxRefreshInMinutes))
			{
				return true;
			}

			//GET ALL PRODUCT IDS from shopify
			//Todo: Make this getpageofproduct work on many pages

			List<ShopifyProduct> shopifyItems = shopifyConn.GetAllProducts(storeAuth);
			List<ShopifyMetafieldMapping> newMap = new List<ShopifyMetafieldMapping>(shopifyItems.Count);
			List<ShopifyMetafield> tempList = new List<ShopifyMetafield>();

			//GET METAFIELD TAGS for each product. 
			foreach (ShopifyProduct product in shopifyItems)
			{
				tempList = shopifyConn.GetAllMetafields(storeAuth, null, null, null, null, null, "Shopsterify", "ShopsterProductId", "integer", product.Id);
				if (tempList != null && tempList.Count > 0 &&product.Id!=null)
				{
					newMap.Add(ShopifyMetafieldMapping.CreateMetafieldMapping((int) product.Id, ((DateTime) product.Variants[0].UpdatedAt).ToUniversalTime(), tempList[0]));
				} //Else, ignore it an move on. ToDo: should we log this?
			}
			productMap = newMap;
			return (productMap!=null);

		}

		public override bool AddMappingToDataSource(ShopifyMetafieldMapping itemToAdd)
		{
			ShopifyMetafield newMetafield = new ShopifyMetafield();
			newMetafield.Namespace = "Shopsterify";
			newMetafield.Key = "ShopsterProductId";
			newMetafield.Value = itemToAdd.DestinationId.ToString();
			newMetafield.ValueType = "integer";

			if (shopifyConn.CreateMetafield(storeAuth, newMetafield, itemToAdd.SourceId).State == ResponseState.OK)
			{
				return true;
			}
			else
			{
				return false;
			}

		}

		public override bool DeleteMappingFromDataSource(ShopifyMetafieldMapping itemToDelete)
		{

			//TODO: add delete metafield to shopifyConn
			throw new NotImplementedException();
		}
	}

		
}
