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
//	limitations under the License.

namespace Connectster.Shopify.Interfaces
{
	interface IShopifyAddress 
	{
		string Address1 { get; set; }
		string Address2 { get; set; }
		string City { get; set; }
		string Company { get; set; }
		string Country { get; set; }
		string FirstName { get; set; }
		string LastName { get; set; }
		string PhoneNumber { get; set; }
		string Province { get; set; }
		string Zip { get; set; }
		string Name { get; set; } 
		string CountryCode { get; set; }
		string ProvinceCode { get; set; }
	}
}
