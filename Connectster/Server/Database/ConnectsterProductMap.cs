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
	public class ConnectsterProductMap: ProductMap<ConnectsterProduct>
	{
		private readonly ConnectsterUser _myUser;
		private ConnectsterDatabase _databaseConn;

		public ConnectsterProductMap(ConnectsterUser user)
		{
			if(user == null)
			{ 
				throw new ArgumentNullException("user");
			}
			
			_myUser = user;
			RefreshProductMap();
		}
            
		
		public override sealed bool RefreshProductMap()
		{
			if(_databaseConn==null)
			{
                _databaseConn = new ConnectsterDatabase();
			}

			productMap = _databaseConn.SelectProductForUser(_myUser);
			return (productMap != null);
		}

		public override bool AddMappingToDataSource(ConnectsterProduct itemToAdd)
		{
			//return databaseConn.CreateProductMapping(itemToAdd.SourceId, itemToAdd.DestinationId);
			if (!(itemToAdd.shopifyItem != null && itemToAdd.shopsterItem != null))
			{
				return _databaseConn.InsertProductForUser(_myUser, itemToAdd.SourceId, itemToAdd.SourceDate.ToUniversalTime(), itemToAdd.DestinationId, itemToAdd.DestinationDate.ToUniversalTime());
			}
			return _databaseConn.InsertProductForUser(_myUser, itemToAdd.shopsterItem, itemToAdd.shopifyItem);
		}

		public override bool DeleteMappingFromDataSource(ConnectsterProduct itemToDelete)
		{
			return _databaseConn.DeleteConnectsterProductMapping(itemToDelete.SourceId, itemToDelete.DestinationId);
		}
	}
}
