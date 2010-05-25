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

namespace Shopsterify.Shopster.ShopsterObjects
{
	class ShopsterCartItem : ILineItem
	{

		internal OrderCartItemType cartItem;


		public ShopsterCartItem(OrderCartItemType inItem)
		{
			cartItem = inItem;
		}
	
		public ShopsterCartItem(ILineItem inItem)
		{
			cartItem = new OrderCartItemType();
			cartItem.Quantity = inItem.Quantity;
			cartItem.ItemId = inItem.ProductId.ToString() ;
		}


		#region ILineItem Members

		public int LineItemId
		{
			get
			{ //This looks really complex, but it just returns the int representation of the first 4 characters of the hashcode... hack
				return System.BitConverter.ToInt32(System.Text.Encoding.UTF8.GetBytes(cartItem.GetHashCode().ToString().ToCharArray(), 0, 4), 0) ;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public int ProductId
		{
			get
			{
				return Convert.ToInt32(cartItem.ItemId);
			}
			set
			{
				cartItem.ItemId = value.ToString();
			}
		}

		public int Quantity
		{
			get
			{
				return cartItem.Quantity == null ? 0 : (int)cartItem.Quantity;
			}
			set
			{
				cartItem.Quantity = value;
			}
		}

		#endregion
	}
}
