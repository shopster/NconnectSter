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

using System.Collections.Generic;
using System.Linq;
using Connectster.Server.Interfaces;
using Connectster.Shopster.ShopsterObjects;

namespace Connectster.Shopster.Converters
{
	class ShopsterOrderConverter
	{
		public static ShopsterOrder ToShopsterOrder(IOrder inOrder, Dictionary<int, int> productMap)
		{

			ShopsterOrder returnOrder = new ShopsterOrder();


			returnOrder.LineItems = inOrder.LineItems;
			foreach (ILineItem item in returnOrder.LineItems)
			{
				if (productMap.Values.Contains(item.ProductId))
				{
					//Change the id to the source mapped item. (ie, item.ProductId== ShopifyId, p.Key == ShopsterId)
					item.ProductId = productMap.Where(p => p.Value == item.ProductId).Select(p => p.Key).FirstOrDefault() ;
				}
				else
				{
					returnOrder.LineItems = returnOrder.LineItems.Where(i => i.ProductId != item.ProductId).Select(i => i).ToList<ILineItem>();

					//Todo logging and message the user to tell them the items not there

				}
				

			}


			returnOrder.BillingAddress = ShopsterAddressConverter.ToShopsterAddress(inOrder.BillingAddress);
			returnOrder.ShippingAddress = inOrder.ShippingAddress;
			returnOrder.orderCart.ShippingMethod = inOrder.ShippingMethod;
			return returnOrder;
		}
	}
}
