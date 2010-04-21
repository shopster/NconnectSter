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
using Shopsterify.Shopster.Converters;
using Shopsterify.Shopster.ShopsterObjects;

namespace Shopsterify.Shopster.ShopsterObjects
{
	class ShopsterOrder : IOrder
	{

		internal OrderCartType orderCart;
		private ShopsterAddress billingAddress;
		private ShopsterAddress shippingAddress;

		public ShopsterOrder()
		{

			orderCart = new OrderCartType();
			orderCart.BillingInfo = new BillingInfoType();
		}

		public string MessageId //we can only get it.
		{
			get
			{
				return orderCart.GetHashCode().ToString();
			}
		}
			
		#region IOrder Members
		public IAddress BillingAddress
		{
			get
			{
				return billingAddress;
			}
			set
			{
				
				billingAddress = ShopsterAddressConverter.ToShopsterAddress(value);
				orderCart.BillingInfo.Address = billingAddress.address;
			}
		}

		public IAddress ShippingAddress
		{
			get
			{
				return shippingAddress;
			}
			set
			{
				shippingAddress = ShopsterAddressConverter.ToShopsterAddress(value);
				orderCart.ShippingInfo = shippingAddress.address;
			}
		}

		public List<ILineItem> LineItems
		{
			get
			{
				List<ILineItem> returnList = new List<ILineItem>(orderCart.Items.Count);
				foreach (var item in orderCart.Items)
				{
					returnList.Add(new ShopsterCartItem(item));
				}

				return returnList;
			}
			set
			{
				orderCart.Items = new OrderCartItemListType();
				
				foreach (var item in value)
				{
					orderCart.Items.Add(new ShopsterCartItem(item).cartItem);
				}
			}
		}

		public string ShippingMethod
		{
			get
			{
				return this.orderCart.ShippingMethod;
			}
			set
			{
				this.orderCart.ShippingMethod = value;
			}
		}

		#endregion
	}
}
