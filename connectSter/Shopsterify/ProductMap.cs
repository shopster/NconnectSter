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
using System.Text;
using Shopsterify.Shopsterify.Database.Interfaces;
using Shopsterify.Shopsterify.Interfaces;
using Shopsterify.Database;

namespace Shopsterify.Shopsterify
{
	 abstract public class ProductMap<T> : IProductMap<T> 
		where T : IMappedProduct
	{

		#region IProductMap<T> Members
		protected List<T> productMap;
		protected DateTime cacheDate; //this is changed by baseclass implementations. 
		
		public DateTime CacheDateStamp { 
			get{ return cacheDate; } 
		
		}

		public IList<int> GetSourceIds()
		{
			List<int> returnList = new List<int>(productMap.Count);
			foreach (IMappedProduct product in productMap)
			{
				returnList.Add(product.SourceId);
			}
			return returnList;
		}

		public IList<int> GetDestinationIds()
		{
			return productMap.Select(p => p.DestinationId).ToList();
		}

		//TODO: change this to Dictionary<int,List<int>> to match the 1:N mapping
		public IDictionary<int, int> GetProductTable()
		{

			Dictionary<int,int> returnList = new Dictionary<int,int>(productMap.Count);
			foreach (IMappedProduct product in productMap)
			{
				returnList.Add(product.SourceId, product.DestinationId);

			}
			return returnList;
		
		}
		
		 		public IList<int> GetSourcesForDestiation(int destination)
		{

			return productMap.Where(p => p.DestinationId==destination)
				.Select(p => p.SourceId)
				.Distinct( ).ToList();
				
		}

		public IList<int> GetDestiationsForSource(int source)
		{
			return 	productMap.Where(product => product.SourceId == source).Select(product => product.DestinationId).ToList<int>();
		}

		public IList<T> GetMappedItems()
		{
			if (productMap == null)
			{
				RefreshProductMap();
			}
			return productMap;
		}

		public bool AddMapping(T itemToAdd)
		{
			//Error check.
			if (productMap == null)
			{
				if (!RefreshProductMap())
				{
					
					return false;
				}
			}

			//Updates the timestamp if it already exists, otherwise adds the mapping. 
			foreach (T productMapping in productMap)
			{
				if (productMapping.DestinationId == itemToAdd.DestinationId && productMapping.SourceId == itemToAdd.SourceId)
				{
					if (itemToAdd.SourceDate > productMapping.SourceDate)
					{
						productMapping.SourceDate = itemToAdd.SourceDate;
						
					}
					return (AddMappingToDataSource(productMapping));				}
			}
			
			productMap.Add(itemToAdd);
			return AddMappingToDataSource(itemToAdd);
		}

		public bool DeleteMapping(T itemToDelete)
		{
			foreach (T productMapping in productMap)
			{
				if (productMapping.DestinationId == itemToDelete.DestinationId && productMapping.SourceId == itemToDelete.SourceId)
				{
					productMap.Remove(itemToDelete);

					return (DeleteMappingFromDataSource(itemToDelete));
				}
			}

			return false;
		}

		abstract public bool RefreshProductMap();
		abstract public bool AddMappingToDataSource(T itemToAdd);
		abstract public bool DeleteMappingFromDataSource(T itemToDelete);
		#endregion
	}
}
