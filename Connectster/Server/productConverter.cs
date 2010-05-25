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
using System.Text.RegularExpressions;
using Connectster.Shopify;
using Connectster.Shopster;
using Shopster.API.Service.SDK.Core.Soap;

namespace Connectster.Server
{

	#region Shopsterify Conversion Functions
	public class ShopsterifyConverter
	{
		public static ShopifyProduct convertItem(InventoryItemType shopsterItem, List<InventoryCategoryType> categories)
		{

			//These are in the order of the ShopifyProduct class declaration:
			ShopifyProduct shopifyItem = new ShopifyProduct();
			ShopsterCommunicator shopsterComm = ShopsterCommunicator.Instance();
			
			//ShopifyItem.Body appears as "Desctiption" in their product management screen.
			shopifyItem.Body = Regex.Replace(shopsterItem.LongDescription,@"<[^>]*>",string.Empty, RegexOptions.Multiline);
			
			shopifyItem.BodyHtml = shopsterItem.LongDescription;

			//	shopifyItem.Created = null; //We cant set this anyways.
			shopifyItem.Handle = Regex.Replace(shopsterItem.Name, " ", "-");
			//shopifyItem.Id = null; // We cannot set the Id anyways.

			
			//Todo make this shopster || category.name, include multiple levels 
			shopifyItem.ProductType = categories.Count==0 ? "TestProductType" : categories.OrderBy(category=>category.CategoryId).First().Name; // Todo: Is this the best solution?
		
			//	shopifyItem.PublishedAt = null;
			shopifyItem.TemplateSuffix = null;
			shopifyItem.Title = shopsterItem.Name; //
			shopifyItem.UpdatedAt = DateTime.UtcNow;
			shopifyItem.Vendor = shopsterItem.Name;  //Todo:  retrieveName (shopsterItem.SupplierId.name)
			


			//Tags is a coma separated list of strings
			StringBuilder sb = new StringBuilder();
			
			foreach (InventoryCategoryType category in categories)
			{
				if (category.Name != null)
				{
					sb.Append(category.Name + ",");
				}
			}
			shopifyItem.Tags = sb.ToString().TrimEnd(',');  //remove the trailing ","

			shopifyItem.Variants = new ShopifyVariant[1];
			shopifyItem.Variants[0] = createVariant(shopsterItem);

			shopifyItem.Images = new ShopifyImage[1];
			ShopifyImage image = new ShopifyImage();
			image.Source = shopsterItem.Images[0].LargeUrl.ToString();
			shopifyItem.Images[0] = image;
			//private ShopifyImage[] images;

			//shopifyItem.Options = new ShopifyProduct.ShopifyOption[1];
			//shopifyItem.Options[0] = new ShopifyProduct.ShopifyOption("Option 1");

			return shopifyItem;
		}

		//This is intended to only be called from within convertItem(InventoryItemType shopsterItem)
		private static ShopifyVariant createVariant(InventoryItemType shopsterItem)
		{

			const double gramsPerLb = 453.59237;
			ShopifyVariant variant = new ShopifyVariant();

			variant.CompareAtPrice = null;  //retrieve max price of this shopsterItem/variant

			//Todo better choice for created at?
			//variant.CreatedAt = String.Format("{0:u}", DateTime.Now); DateTime.
			variant.FulfillmentService = "manual";

			//Variant.weight 
			if (shopsterItem.Weight.Unit == WeightUnitType.KG)
			{
				variant.Grams = (int)Math.Floor(shopsterItem.Weight.Value * 1000);
			}
			else if (shopsterItem.Weight.Unit == WeightUnitType.LBS)
			{
				variant.Grams = (int)Math.Floor(((double)shopsterItem.Weight.Value) * gramsPerLb);
			}

			variant.Id = null; //can be absent if we're uploading
			variant.InventoryManagement = "shopify";
			variant.InventoryPolicy = "deny"; //Need to see what this actually should be

			//Variant.quantity
			if (shopsterItem.Quantity >= 0)
			{
				variant.InventoryQuantity = shopsterItem.Quantity;
			}
			else
			{
				variant.InventoryQuantity = 0;
			}

			variant.Title = shopsterItem.Name;
			variant.Option1 = null;
			variant.Option2 = null;
			variant.Option3 = null;

			//private int position; //dont know what this is.

			variant.Price = shopsterItem.Pricing.RetailPrice; //Is this correct?
			//private int productId; // ID of parent, we cant set it anyways
			variant.RequiresShipping = true; //Eventually we may sell electronic stuff
			variant.Sku = shopsterItem.Sku; // how will we handle multiple variants?
			variant.Taxable = false;
			variant.UpdatedAt = null;

			return variant;
		}
	#endregion
	}
}