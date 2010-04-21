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
using Shopster.API.Service.SDK.Core.Soap;

namespace Shopsterify.Shopsterify
{

	/// <summary>
	/// Mapping of ShopsterProduct to ShopifyProduct 
	/// </summary>
	public class ShopsterifyProduct : IMappedProduct
	{
		internal InventoryItemType shopsterItem;
		internal ShopifyProduct shopifyItem;

		public int SourceId { get; set; }
		public int DestinationId { get; set; }
		
		public DateTime SourceDate { get; set; }
		public DateTime DestinationDate { get; set; }
	

		public ShopsterifyProduct(int sourceId, DateTime sourceVersion , int destinationId, DateTime destinationVersion)
		{
			SourceId = sourceId;
			//Todo get source date from shopster
			SourceDate = sourceVersion;
			DestinationId = destinationId;
			DestinationDate = destinationVersion;
			this.shopifyItem = null;
			this.shopsterItem = null;
		}

		public ShopsterifyProduct(InventoryItemType shopsterItem, ShopifyProduct shopifyItem)
		{
			this.shopsterItem = shopsterItem;
			this.shopifyItem = shopifyItem;

			SourceId = Convert.ToInt32(shopsterItem.ItemId);
			SourceDate = DateTime.UtcNow;
			
			DestinationId = (int) shopifyItem.Id;
			DestinationDate = shopifyItem.Variants[0].UpdatedAt== null? DateTime.UtcNow: ((DateTime) shopifyItem.Variants[0].UpdatedAt);
			
		}



	}
}
