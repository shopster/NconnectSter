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
using Shopsterify.Shopsterify.Interfaces;

namespace Shopsterify.Shopsterify.Database.Interfaces
{
	public interface IProductMap<T>
	{
		DateTime CacheDateStamp { get; }
				
		IList<int> GetSourceIds(); //All SourceIds
		IList<int> GetDestinationIds(); //All DestinationIds
		IDictionary<int, int> GetProductTable(); //A dictionary form of this ProductMap
		IList<int> GetSourcesForDestiation(int destination); //Get all sources for a given destination
		IList<int> GetDestiationsForSource(int source); //Get all desinations for a given source
		IList<T> GetMappedItems(); //Get all of the mapped items

		bool AddMapping(T itemToAdd); 
		bool DeleteMapping(T itemToDelete);
				
		bool RefreshProductMap(); //If dirty, do some work. 
		
		
	}
}
