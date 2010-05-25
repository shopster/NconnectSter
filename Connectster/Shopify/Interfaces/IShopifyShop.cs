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
	interface IShopifyShop
	{

		
		string Address1 { get; set; }
		string City { get; set; }
		string Country { get; set; }
		DateTime CreatedAt { get; set; }
		string Domain { get; set; }
		string Email { get; set; }
		int Id { get; set; }
		string Name { get; set; }
		string Phone { get; set; }
		string Province { get; set; }
		bool Public { get; set; }
		string Source { get; set; }
		string Zip { get; set; }
		CurrencyCode Currency { get; set; }
		string Timezone { get; set; }
		string ShopOwner { get; set; }
		string MoneyFormat { get; set; }
		string MoneyWithCurrencyFormat { get; set; }
		bool TaxesIncluded { get; set; }
		string TaxShipping { get; set; }
		AccountType PlanName { get; set; }
	}
}
