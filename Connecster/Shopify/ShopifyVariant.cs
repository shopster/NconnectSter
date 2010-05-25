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

namespace Shopsterify
{
	[Serializable]
	[XmlRootAttribute(ElementName = "variants", IsNullable = false)]
	public class ShopifyVariant
	{
		private decimal? compareAtPrice; //compare-at-price is optional
		private DateTime? createdAt;
		private string fulfillmentService;
		private int? grams; //UI asks for lbs.
		private int? id; //can be absenf it we're uploading
		private string inventoryManagement;
		private string inventoryPolicy;
		private int? inventoryQuantity; // if absent then infinite.
		private string title;
		private string option1;
		private string option2;
		private string option3;
		private int? position;
		private decimal price;
		private int? productId; // ID of parent.
		private bool requiresShipping;
		private string sku;
		private bool taxable;
		private DateTime? updatedAt;

		

		public ShopifyVariant(decimal? compareAtPrice, DateTime? createdAt, string fulfillmentService,
				int? grams,  int? id, string inventoryManagement, string inventoryPolicy, int? inventoryQuantity, 
				string title, string option1, string option2, string option3, int position, 
				decimal price, int productId, bool requiresShipping, string sku, bool taxable, 
				DateTime? updatedAt)
	{	
	
		this.compareAtPrice=compareAtPrice;
		this.createdAt=createdAt;
		this.fulfillmentService=fulfillmentService;
		this.grams=grams; //weight of a product 
		this.id=id;
		this.inventoryManagement=inventoryManagement;
		this.inventoryPolicy=inventoryPolicy;
		this.inventoryQuantity=inventoryQuantity;
		this.title=title; // these are the titles of the options, separated with " / "
		this.option1=option1;
		this.option2=option2;
		this.option3=option3;
		this.position=position;
		this.price=price;
		this.productId =productId;
		this.requiresShipping=requiresShipping;
		this.sku=sku;
		this.taxable=taxable;
		this.title=title;
		this.updatedAt=updatedAt;

			
		}

		
		/// <summary>
		/// DO NOT USE. Required for XML Serialization. 
		/// </summary>
		public ShopifyVariant()
		{ }

		[XmlElement(ElementName = "compare-at-price", IsNullable=true)]
		public decimal? CompareAtPrice
		{
			get { return compareAtPrice; }
			set { compareAtPrice = value;}
		}
		
		[XmlElement(ElementName = "created-at")]
		public DateTime? CreatedAt
		{
			get { return createdAt; }
			set { createdAt = value; }
		}
		
		[XmlElement(ElementName = "fulfillment-service", IsNullable = false)]
		public string FulfillmentService
		{
			get { return fulfillmentService; }
			set { fulfillmentService = value;}
		}

		[XmlElement(ElementName = "grams", IsNullable=true)]
		public int? Grams
		{
			get { return grams; }
			set { grams = value; }
		}
		
		[XmlElement(ElementName = "id", IsNullable = true)]
		public int? Id
		{
			get { return id; }
			set { id = value; }
		}
		
		[XmlElement(ElementName = "inventory-management")]
		public string InventoryManagement
		{
			get { return inventoryManagement; }
			set { inventoryManagement = value; }
		}
		public bool ShouldSerializeInventoryManagement()
		{
			return (inventoryManagement != string.Empty);
		}


		[XmlElement(ElementName = "inventory-policy", IsNullable = false)]
		public string InventoryPolicy
		{
			get { return inventoryPolicy; }
			set { inventoryPolicy = value; }
		}

		[XmlElement(ElementName = "inventory-quantity", IsNullable = true)]
		public int? InventoryQuantity
		{
			get { return inventoryQuantity; }
			set { inventoryQuantity = value; }
		}
		//public bool ShouldSerializeInventoryQuantity()
		//{
		//    return (inventoryQuantity != null);
		//}

		[XmlElement(ElementName = "title")]
		public string Title
		{
			get { return title; }
			set { title = value; }
		}

		[XmlElement(ElementName = "option1")]
		public string Option1
		{
			get { return option1; }
			set { option1= value; }
		}

		[XmlElement(ElementName = "option2", IsNullable=false)]
		public string Option2
		{
			get { return option2; }
			set { option2 = value; }
		}
		public bool ShouldSerializeOption2()
		{
			return (option2 != string.Empty);
		}

		[XmlElement(ElementName = "option3")]
		public string Option3
		{
			get { return option3; }
			set { option3 = value; }
		}
		public bool ShouldSerializeOption3()
		{
			return (option3 != string.Empty);
		}


		[XmlElement(ElementName = "position", IsNullable=true)]
		public int? Position
		{
			get { return position; }
			set { position = value; }
		}

		[XmlElement(ElementName = "price")]
		public decimal Price
		{
			get { return price; }
			set { price = value; }
		}

		[XmlElement(ElementName = "product-id", IsNullable=true)]
		public int? ProductId
		{
			get { return productId; }
			set { productId = value; }
		}

		[XmlElement(ElementName = "requires-shipping")]
		public bool RequiresShipping
		{
			get { return requiresShipping; }
			set { requiresShipping = value; }
		}

		[XmlElement(ElementName = "sku")]
		public string Sku
		{
			get { return sku; }
			set { sku = value; }
		}
		public bool ShouldSerializeSku()
		{
			return (sku != string.Empty);
		}


		[XmlElement(ElementName = "taxable", IsNullable = false)]
		public bool Taxable
		{
			get { return taxable; }
			set { taxable = value; }
		}

		[XmlElement(ElementName = "updated-at", IsNullable = true)]
		public DateTime? UpdatedAt
		{
			get { return updatedAt; }
			set { updatedAt = value == null ? (DateTime?)null : ((DateTime)value).ToUniversalTime(); }
		}
		//public bool ShouldSerializeUpdatedAt()
		//{
		//    return (updatedAt ==DateTime.MinValue);
		//}

	}
}
