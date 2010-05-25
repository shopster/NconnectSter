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
using System.Xml.Serialization;
using Shopsterify.Shopify.Interfaces;

namespace Shopsterify.Shopify
{

	[XmlRootAttribute(ElementName = "metafield", IsNullable = false)] 
	public class ShopifyMetafield : IShopifyMetafield, IShopifyObject
	{
		[XmlElement(ElementName = "created-at", IsNullable = false)]
		public DateTime CreatedAt { get; set; }
		
		[XmlElement(ElementName = "description", IsNullable = false)]
		public string Description { get; set; }
		
		[XmlElement(ElementName = "id", IsNullable = false)]
		public int Id { get; set; }
		
		[XmlElement(ElementName = "key", IsNullable = false)]
		public string Key { get; set; }
		
		[XmlElement(ElementName = "namespace", IsNullable = false)]
		public string Namespace { get; set; }
		
		[XmlElement(ElementName = "updated-at", IsNullable = false)]
		public DateTime UpdatedAt { get; set; }
		
		[XmlElement(ElementName = "value", IsNullable = false)]
		public string Value { get; set; }
		
		[XmlElement(ElementName = "value-type", IsNullable = false)]
		public string ValueType { get; set; } //can be int or string

		public ShopifyMetafield()
		{

		}


	}	
}
