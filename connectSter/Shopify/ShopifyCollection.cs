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
using System.Xml;
using System.Xml.Serialization;
using Shopsterify.Shopify.Interfaces;

namespace Shopsterify.Shopify
{
	[XmlRootAttribute(ElementName = "custom-collection", IsNullable = false)] 
	public class ShopifyCollection: IShopifyCollection, IShopifyObject
	{
		[XmlElement(ElementName = "body-html", IsNullable = false)]
		public string BodyHtml { get; set; }
		[XmlElement(ElementName = "handle", IsNullable = false)]
		public string Handle { get; set; }
		[XmlElement(ElementName = "id", IsNullable = false)] 
		public int Id { get; set; }
		[XmlElement(ElementName = "published-at", IsNullable = false)] 
		public DateTime PublishedAt { get; set; }
		[XmlElement(ElementName = "sort-order", IsNullable = false)] 
		public string SortOrder { get; set; }
		[XmlElement(ElementName = "template-suffix", IsNullable = false)] 
		public string TemplateSuffix { get; set; }
		[XmlElement(ElementName = "title", IsNullable = false)] 
		public string Title { get; set; }
		[XmlElement(ElementName = "updated-at", IsNullable = false)] 
		public DateTime UpdatedAt{ get; set; }
		[XmlElement(ElementName = "body", IsNullable = false)] 
		public string Body { get; set; }
		[XmlElement(ElementName = "published", IsNullable = true)] 
		public bool? Published { get; set; } 


		/// <summary>
		/// Required by serializiation, do not use.
		/// </summary>
		public ShopifyCollection()
		{
		}

	}
}
