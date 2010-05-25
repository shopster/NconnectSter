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
using Shopsterify.Shopify;

namespace Shopsterify.Shopsterify
{

	/// <summary>
	/// Shopify Metafield Tagging of product. Source = shopify, Destination = shopster
	/// </summary>
	public class ShopifyMetafieldMapping : IMappedProduct
	{

		private ShopifyMetafield metafield;
		public static ShopifyMetafieldMapping CreateMetafieldMapping(int sourceId, DateTime creationDate, ShopifyMetafield thisField)
		{
			if (thisField.ValueType != "integer" || thisField.Namespace != "Shopsterify" || thisField.Key != "ShopsterProductId")
			{
				//ToDo: Throw exception?
				return null; //caller is responsible to check if creation worked.
			}
			
			return new ShopifyMetafieldMapping(sourceId, creationDate, thisField);
		}

		private ShopifyMetafieldMapping(int sourceId, DateTime creationDate, ShopifyMetafield thisField)
		{
			SourceId = sourceId;
			DestinationId = Convert.ToInt32(thisField.Value);

			SourceDate = creationDate;
			DestinationDate = creationDate;
			metafield = thisField;
		}

		#region IMappedProduct Members
		public int SourceId { get; set; }
		public int DestinationId { get; set; }
		public DateTime SourceDate { get; set; }
		public DateTime DestinationDate { get; set; }
		#endregion
	}
}
