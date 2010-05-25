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
using Shopster.API.Service.SDK.Core.Soap;
using System.Text;

namespace Connectster.Shopster
{
	public class ShopsterProductComparer : IEqualityComparer<InventoryItemType>
	{

		#region IEqualityComparer<InventoryItemType> Members
		public bool Equals(InventoryItemType x, InventoryItemType y)
		{
			//if everything is equal return true. otherwise return false. 
			if (x.ItemId == y.ItemId)
				if (x.CanPurchase == y.CanPurchase)
					if (x.IsStoreVisible == y.IsStoreVisible)
						if (x.LongDescription == y.LongDescription)
							if (x.Name == y.Name)
								if (x.Quantity == y.Quantity)
									if (x.ShortDescription == y.ShortDescription)
										if (x.Sku == y.Sku)
											if (x.StoreUrl == y.StoreUrl)
												if (x.SupplierId == y.SupplierId)
													if (x.WarehouseId == y.WarehouseId)
														if (x.Categories == y.Categories)
															if (x.Images == y.Images)
																if (x.Pricing == y.Pricing)
																	if (x.Weight == y.Weight)
																		return true;
			
			return false;
		}

		public int GetHashCode(InventoryItemType obj)
		{
			StringBuilder allFields = new StringBuilder();
			
			allFields.Append(obj.ItemId);
			allFields.Append(obj.IsStoreVisible.ToString());
			allFields.Append(obj.LongDescription.ToString());
			allFields.Append(obj.Quantity.ToString());
			allFields.Append(obj.ShortDescription);
			allFields.Append(obj.Sku);
			allFields.Append(obj.StoreUrl);
			allFields.Append(obj.SupplierId);
			allFields.Append(obj.WarehouseId);
			allFields.Append(obj.Categories.ToString());
			allFields.Append(obj.Images.ToString());
			allFields.Append(obj.Pricing.ToString());
			allFields.Append(obj.Weight.ToString());

			//TODO: make this into an int by trimming the string to 8 chars and converting hex to int.
			//return (HashString(allFields.ToString()));

			return (0);
		}


		private static string HashString(string Value)
		{
			System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
			byte[] data = System.Text.Encoding.ASCII.GetBytes(Value);
			data = x.ComputeHash(data);
			string ret = "";
			for (int i = 0; i < data.Length; i++)
				ret += data[i].ToString("x2").ToLower();
			return ret;
		}
		#endregion
	}


}