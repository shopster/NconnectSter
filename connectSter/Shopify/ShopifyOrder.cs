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
using Shopsterify.Shopify.Interfaces;
using System.Security.Policy;
using System.Net;
using System.Xml.Serialization;
using Shopsterify.Shopsterify.Interfaces;

namespace Shopsterify.Shopify
{
	[XmlRootAttribute(ElementName = "order", IsNullable = false)] 
	public class ShopifyOrder : IShopifyOrder, IShopifyObject, IOrder
	{



#region PrivateFields
		//private bool buyerAcceptsMarketing;
		//private DateTime? closedAt;
		//private DateTime? createdAt;
		//private CurrencyCode currency;
		//private string email;
		//private FinancialState financialStatus;
		//private string fulfillmentStatus;
		//private string gateway;
		//private int id;
		//private string landingSite;
		//private string name;
		//private String note;
		//private int number;
		//private string referringSite;
		//private decimal subtotalPrice;
		//private bool taxesIncluded;
		//private string token;
		//private decimal totalDiscounts;
		//private decimal totalLineItemsPrice;
		//private decimal totalTax;
		//private int totalWeight; //In grams
		//private DateTime updatedAt;
		//private string browserIp;
		//private string landingSiteRef;
		//private int orderNumber;
		//private ShopifyAddress billingAddress;
		//private ShopifyAddress shippingAddress;
		//private ShopifyLineItem[] lineItems;
		//private ShopifyShippingLine[] shippingLines;
		//private ShopifyPaymentDetails paymentDetails;
		//private ShopifyNoteAttribute[] noteAttributes;
#endregion

		public ShopifyOrder()
		{


		}

#region PublicProperties


		[XmlElement(ElementName = "buyer-accepts-marketing", IsNullable = false)]
		public bool BuyerAcceptsMarketing{ get; set ;} //If the user wants to receive emails

		[XmlElement(ElementName = "closed-at", IsNullable = true)]
		public DateTime? ClosedAt{ get; set ;} //

		[XmlElement(ElementName = "created-at", IsNullable = true)]
		public DateTime? CreatedAt{ get; set ;}

		[XmlElement(ElementName = "currency", IsNullable = false)]
		public CurrencyCode Currency{ get; set ;} //USD or CAD 

		[XmlElement(ElementName = "email", IsNullable = false)]
		public string Email{ get; set ;}

		[XmlElement(ElementName = "financial-status", IsNullable = false)]
		public FinancialState FinancialStatus{ get; set ;}

		[XmlElement(ElementName = "fulfillment-status", IsNullable = true)]
		public string FulfillmentStatus{ get; set ;}

		[XmlElement(ElementName = "gateway", IsNullable = false)]
		public string Gateway{ get; set ;}

		[XmlElement(ElementName = "id", IsNullable = false)]
		public int Id{ get; set ;}

		[XmlElement(ElementName = "landing-site", IsNullable = true)]
		public string LandingSite{ get; set ;}

		[XmlElement(ElementName = "name", IsNullable = false)]
		public string Name{ get; set ;}

		[XmlElement(ElementName = "note", IsNullable = true)]
		public String Note{ get; set ;}

		[XmlElement(ElementName = "number", IsNullable = false)]
		public int Number{ get; set ;}

		[XmlElement(ElementName = "referring-site", IsNullable = true)]
		public string ReferringSite{ get; set ;}

		[XmlElement(ElementName = "subtotal-price", IsNullable = false)]
		public decimal SubtotalPrice{ get; set ;}

		[XmlElement(ElementName = "taxes-included", IsNullable = false)]
		public bool TaxesIncluded{ get; set ;}

		[XmlElement(ElementName = "token", IsNullable = false)]
		public string Token{ get; set ;}

		[XmlElement(ElementName = "total-discounts", IsNullable = false)]
		public decimal TotalDiscounts{ get; set ;}

		[XmlElement(ElementName = "total-line-items-price", IsNullable = false)]
		public decimal TotalLineItemsPrice{ get; set ;}

		[XmlElement(ElementName = "total-tax", IsNullable = false)]
		public decimal TotalTax{ get; set ;}

		[XmlElement(ElementName = "total-weight", IsNullable = false)]
		public int TotalWeight{ get; set ;} //In grams

		[XmlElement(ElementName = "updated-at", IsNullable = false)]
		public DateTime UpdatedAt{ get; set ;}

		[XmlElement(ElementName = "browser-ip", IsNullable = true)]
		public string BrowserIp{ get; set ;}

		[XmlElement(ElementName = "landing-site-ref", IsNullable = false)]
		public string LandingSiteRef{ get; set ;}

		[XmlElement(ElementName = "order-number", IsNullable = false)]
		public int OrderNumber{ get; set ;}

		[XmlElement(ElementName = "billing-address", IsNullable = false)]
		public ShopifyAddress BillingAddress{ get; set ;}

		[XmlElement(ElementName = "shipping-address", IsNullable = false)]
		public ShopifyAddress ShippingAddress{ get; set ;}

		[XmlArray(ElementName = "line-items", IsNullable = false)]
		[XmlArrayItem(ElementName = "line-item")]
		public ShopifyLineItem[] LineItems{ get; set ;}

		[XmlArray(ElementName = "shipping-lines", IsNullable = false)]
		[XmlArrayItem(ElementName = "shipping-line")]
		public ShopifyShippingLine[] ShippingLines{ get; set ;}

		[XmlArray(ElementName = "tax-lines", IsNullable = false)]
		[XmlArrayItem(ElementName = "tax-line")]
		public ShopifyTaxLine[] TaxLines{ get; set; }
		

		//public ShopifyPaymentDetails paymentDetails{ get; set ;}
		//public ShopifyNoteAttribute[] noteAttributes{ get; set ;}
#endregion





		#region IOrder Members

		IAddress IOrder.BillingAddress
		{
			get
			{
				return (IAddress) this.BillingAddress;
			}
			set
			{
				this.BillingAddress = new ShopifyAddress(value);

			}
		}

		IAddress IOrder.ShippingAddress
		{
			get
			{
				return (IAddress)this.ShippingAddress;
			}
			set
			{
				this.ShippingAddress = new ShopifyAddress(value);
			}
		}

		List<ILineItem> IOrder.LineItems
		{
			get {

				return this.LineItems.Select(lineItem => (ILineItem)lineItem).ToList<ILineItem>();
			}
		}

		#endregion

		#region IOrder Members


		public string ShippingMethod
		{
			get
			{

				return "Standard";

				//TODO clean this up, clearly this field is far to restricted.
				//If we have shipping lines, this is the best quality info.
				if (ShippingLines != null && ShippingLines.Count() != 0)
				{
					StringBuilder shippingString = new StringBuilder();

					foreach (ShopifyShippingLine shippingLine in ShippingLines)
					{
						shippingString.Append(shippingLine.Title + ", ");
					}

					return shippingString.ToString().Trim().TrimEnd(',');
				}
				else //If no shipping lines, then try to guess the status from the individual items
				{
					foreach (ShopifyLineItem lineItem in LineItems)
					{
						if (lineItem.RequiresShipping)
						{
							return "Shipping required. Method not specified.";
						}

					}
					return "No shipping required.";
				}


			}

			set
			{
				//Todo : make this work, but we should never need it.
				throw new NotSupportedException("Specify the shipping method through the ShopifyShippingLines or ShopifyLineItem.RequireShipping Property.");

			}
		}

		#endregion
	}
}

