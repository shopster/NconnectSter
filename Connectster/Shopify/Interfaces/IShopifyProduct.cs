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

namespace Connectster.Shopify.Interfaces
{
	public interface IShopifyProduct
	{
		string Body{ get; set; }
		string BodyHtml{ get; set; }
		DateTime? Created{ get; set; }
		string Handle{ get; set; }
		int? Id{ get; set; }
		string ProductType { get; set; }
		DateTime? PublishedAt { get; set; }
		string TemplateSuffix { get; set; }
		string Title { get; set; }
		DateTime? UpdatedAt { get; set; }
		string Vendor { get; set; }
		string Tags { get; set; }
		ShopifyVariant[] Variants { get; set; }
		ShopifyImage[] Images { get; set; }
		ShopifyProduct.ShopifyOption[] Options { get; set; }
	}
}
