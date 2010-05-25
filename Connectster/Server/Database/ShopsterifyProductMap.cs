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

namespace Connectster.Server.Database
{
	public class ShopsterifyProductMap: ProductMap<ShopsterifyProduct>
	{
		private ShopsterifyUser myUser;
		private ShopsterifyDatabase databaseConn;

		public ShopsterifyProductMap(ShopsterifyUser aUser)
		{
			if(aUser == null)
			{ 
				throw new ArgumentNullException("User cannot be null");
			}
			
			myUser = aUser;
			RefreshProductMap();
		}

		
		public override bool RefreshProductMap()
		{
			if(databaseConn==null)
			{
				databaseConn = new ShopsterifyDatabase();
				
				//If we still failed to get the connection.
				if( databaseConn==null)
				{
					return false;
				}
			}

			productMap = databaseConn.SelectProductForUser(myUser);
			return (productMap != null);
		}

		public override bool AddMappingToDataSource(ShopsterifyProduct itemToAdd)
		{
			//return databaseConn.CreateProductMapping(itemToAdd.SourceId, itemToAdd.DestinationId);
			if (!(itemToAdd.shopifyItem != null && itemToAdd.shopsterItem != null))
			{
				return databaseConn.InsertProductForUser(myUser, itemToAdd.SourceId, itemToAdd.SourceDate.ToUniversalTime(), itemToAdd.DestinationId, itemToAdd.DestinationDate.ToUniversalTime());
			}
			return databaseConn.InsertProductForUser(myUser, itemToAdd.shopsterItem, itemToAdd.shopifyItem);
		}

		public override bool DeleteMappingFromDataSource(ShopsterifyProduct itemToDelete)
		{
			return databaseConn.DeleteShopsterifyProductMapping(itemToDelete.SourceId, itemToDelete.DestinationId);
		}
	}
}
