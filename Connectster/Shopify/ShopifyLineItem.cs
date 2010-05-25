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
using System.Xml.Serialization;
using Connectster.Server.Interfaces;
using Connectster.Shopify.Interfaces;

namespace Connectster.Shopify
{
	[Serializable]
	public class ShopifyLineItem: ILineItem, IShopifyObject

	{
		

		[XmlElement(ElementName = "fulfillment-service", IsNullable = false)]
		public string FulfillmentService { get; set; }

		[XmlElement(ElementName = "fulfillment-status", IsNullable = true)]
		public string FulfillmentStatus{ get; set; }

		[XmlElement(ElementName = "grams", IsNullable = true)]
		public int? Grams{ get; set; }

		[XmlElement(ElementName = "id", IsNullable = false)]
		public int Id{ get; set; }

		[XmlElement(ElementName = "price", IsNullable = false)]
		public Decimal Price{ get; set; }

		//Todo : when this is null, we need to do our best to hook it up with an existing product in their Rshop
		[XmlElement(ElementName = "product-id", IsNullable = true)]
		public int? ProductId{ get; set; }

		[XmlElement(ElementName = "quantity", IsNullable = false)]
		public int Quantity{ get; set; }

		[XmlElement(ElementName = "requires-shipping", IsNullable = false)]
		public bool RequiresShipping{ get; set; }

		[XmlElement(ElementName = "sku", IsNullable = true)]
		public string Sku{ get; set; }

		[XmlElement(ElementName = "title", IsNullable = false)]
		public string Title{ get; set; }

		[XmlElement(ElementName = "variant-id", IsNullable = true)]
		public int? VariantId{ get; set; }

		[XmlElement(ElementName = "variant-title", IsNullable = true)]
		public string VariantTitle{ get; set; }

		[XmlElement(ElementName = "vendor", IsNullable = true)]
		public string Vendor{ get; set; }
		
		[XmlElement(ElementName = "name", IsNullable = false)]
		public string Name{ get; set; }

		/// <summary>
		/// Required by XMLSerialization, Do Not Use.
		/// </summary>
		public ShopifyLineItem()
		{
		}

		public ShopifyLineItem(ShopifyProduct thisProduct, ShopifyVariant thisVariant)
		{

			Id = 0;
			FulfillmentService = thisVariant.FulfillmentService;
			FulfillmentStatus = null;
			Grams = thisVariant.Grams;
			Price = thisVariant.Price;
			ProductId = thisProduct.Id == null ? 0 : (int)thisProduct.Id;
			Quantity = thisVariant.InventoryQuantity == null ?  0 : (int) thisVariant.InventoryQuantity;
			RequiresShipping = thisVariant.RequiresShipping;
			Sku = thisVariant.Sku;
			Title = thisProduct.Title;
			VariantId = thisVariant.Id == null ? 0 : (int)thisVariant.Id;
			VariantTitle = thisVariant.Title;
			Vendor = thisProduct.Vendor;
			Name = thisVariant.Title;
		}

		#region ILineItem Members

		public int LineItemId
		{
			get
			{
				return Id;
			}
			set
			{
				Id = value;
			}
		}

		#endregion


		#region ILineItem Members


		int ILineItem.ProductId
		{
			get //todo remove this hack
			{

				return ProductId != null ? (int) ProductId : 0;
			}
			set
			{
				this.ProductId = value;
			}
		}

		#endregion
	}

}
