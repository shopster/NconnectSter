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
using Shopsterify.Shopify;

namespace Shopsterify
{
	[XmlRootAttribute(ElementName="product", IsNullable=false)] 
	public class ShopifyProduct : IShopifyProduct, IShopifyObject
	{
		//private fields backing their respective Properties
		private string body;
		private string bodyHtml;
		private DateTime? created;
		private string handle;
		private int? id;
		private string productType;
		private DateTime? publishedAt;
		private string templateSuffix;
		private string title;
		private DateTime? updatedAt;
		private string vendor;
		private string tags;
		private ShopifyVariant[] variants;
		private ShopifyImage[] images;
		private ShopifyOption[] options;
	
		
		
		//minimum set required to create e shopify product
		public ShopifyProduct(string productType, string vendor, string title)
		{
			this.productType = productType;
			this.vendor = vendor;
			this.title = title;
		}

		//All possible options
		public ShopifyProduct(string body, string bodyHtml, DateTime? created, string handle,
			int id, string productType, DateTime? publishedAt, string templateSuffix,
			string title, DateTime? updatedAt, string vendor, string tags, ShopifyVariant[] variants,
			ShopifyImage[] images, ShopifyOption[] options)
		{
			this.body = body;
			this.bodyHtml = bodyHtml;
			this.created = created;
			this.handle = handle;
			this.id = id;
			this.productType = productType;
			this.publishedAt = publishedAt;
			this.templateSuffix = templateSuffix;
			this.title = title;
			this.updatedAt = updatedAt;
			this.vendor = vendor;
			this.tags = tags;
			this.variants = variants;
			this.images = images;
			this.options = options;

		}

		/// <summary>
		/// Required for XMLSerializer. Do Not Use.
		/// </summary>
		public ShopifyProduct()
		{}

		#region getters/setters
		[XmlElement(ElementName="body", IsNullable=false)]
		public string Body
		{
			get { return body; }
			set { body = value; }
		}

		[XmlElement(ElementName = "body-html", IsNullable = false)]
		public string BodyHtml
		{
			get { return bodyHtml; }
			set { bodyHtml = value; } //TODO , set body == BodyHTML - HTML Tags
		}

		
		[XmlElement(ElementName = "created-at", IsNullable=true)]
		public DateTime? Created
		{
			get { return created; }
			set { created = value == null ? value : ((DateTime)value).ToUniversalTime(); }
		}

		

		[XmlElement(ElementName = "handle", IsNullable = false)]
		public string Handle
		{
			get { return handle; }
			set	{ handle = value; }
		}

		[XmlElement(ElementName = "id", IsNullable = true)]
		public int? Id
		{
			get { return id; }
			set	{ id = value;}
		}
		
		public bool ShouldSerializeId()
		{
			return (id !=null);
		}

		[XmlElement(ElementName = "product-type", IsNullable = false)]
		public string ProductType
		{
			get { return productType; }
			set	{ productType = value; }
		}

		[XmlElement(ElementName = "published-at", IsNullable = true)]
		public DateTime? PublishedAt
		{
			get { return publishedAt; }
			set { publishedAt = value == null ? value : ((DateTime)value).ToUniversalTime(); }
		}
		public bool ShouldSerializePublishedAt()
		{
			return (publishedAt!= DateTime.MinValue);
		}

		[XmlElement(ElementName = "tempate-suffix", IsNullable = false)]
		public string TemplateSuffix
		{
			get { return templateSuffix; }
			set { templateSuffix = value; }
		}

		[XmlElement(ElementName = "title", IsNullable = false)]
		public string Title
		{
			get { return title; }
			set	{ title = value; }
		}

		[XmlElement(ElementName = "updated-at", IsNullable = true)]
		public DateTime? UpdatedAt
		{
			get { return updatedAt; }
			set { updatedAt = value == null ? value : ((DateTime)value).ToUniversalTime(); }
		}
		public bool ShouldSerializeUpdatedAt()
		{
			return (updatedAt!= DateTime.MinValue);
		}

		[XmlElement(ElementName = "vendor", IsNullable = false)]
		public string Vendor
		{
			get { return vendor; }
			set { vendor = value; }
		}

		[XmlElement(ElementName = "tags", IsNullable = false)]
		public string Tags
		{
			get { return tags; }
			set	{ tags = value; }
		} 

	
		[XmlArray(ElementName = "variants", IsNullable = true)]
		[XmlArrayItem(ElementName = "variant")]
		public ShopifyVariant[] Variants
		{
			get { return variants; }
			set	{variants= value; }
		}


		public bool ShouldSerializeVariants()
		{
			return (variants != null && variants.Length > 0);
		}


		[XmlArray(ElementName = "images", IsNullable = true)]
		[XmlArrayItem(ElementName = "image")]
		public ShopifyImage[] Images
		{
		    get { return images; }
		    set { images= value; }
		}
		
		//Check if we should include <images> in the xml
		public bool ShouldSerializeImages()
		{
			return ( images != null && images.Length >0);
		}
		
		[XmlArray(ElementName = "options", IsNullable = true)]
		[XmlArrayItem(ElementName = "option")]
		public ShopifyOption[] Options
		{
			get { return options; }
			set { options = value; }
		}
		public bool ShouldSerializeOptions()
		{
			return (options != null && options.Length >0);
		}
#endregion

		#region methods

	
		#endregion

		#region ShopifyOption Class Definition
		public class ShopifyOption
		{
			private string name;
			//required for XMLSerializer
			public ShopifyOption()
			{ }

			public ShopifyOption(string name)
			{
				this.name = name;
			}

			[XmlElement(ElementName = "name", IsNullable = true)]
			public string Name
			{
				get { return name; }
				set { name = value; }
			}
		}
		
		#endregion
	}
}
