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
using System.Xml.Serialization;
using Shopsterify.Shopify.Interfaces;

namespace Shopsterify.Shopify
{
	[XmlRootAttribute(ElementName = "shop", IsNullable = false)] 
	public class ShopifyShop : IShopifyObject, IShopifyShop
	{

		#region public Properties
		[XmlElement(ElementName = "address1", IsNullable = false)]
		public string Address1 { get; set; }
		
		[XmlElement(ElementName = "city", IsNullable = false)]
		public string City { get; set; }

		[XmlElement(ElementName = "country", IsNullable = false)]
		public string Country { get; set; }
		
		[XmlElement(ElementName = "created-at", IsNullable = false)]
		public DateTime CreatedAt { get; set; }

		[XmlElement(ElementName = "domain", IsNullable = false)]
		public string Domain { get; set; }

		[XmlElement(ElementName = "email", IsNullable = false)]
		public string Email { get; set; }

		[XmlElement(ElementName = "id", IsNullable = false)]
		public int Id { get; set; }

		[XmlElement(ElementName = "name", IsNullable = false)]
		public string Name { get; set; }
		
		[XmlElement(ElementName = "phone", IsNullable = false)]
		public string Phone { get; set; }

		[XmlElement(ElementName = "province", IsNullable = false)]
		public string Province { get; set; }

		[XmlElement(ElementName = "public", IsNullable = false)]
		public bool Public { get; set; }

		[XmlElement(ElementName = "source", IsNullable = false)]
		public string Source { get; set; }

		[XmlElement(ElementName = "zip", IsNullable = false)]
		public string Zip { get; set; }

		[XmlElement(ElementName = "currency", IsNullable = false)]
		public CurrencyCode Currency { get; set; }

		[XmlElement(ElementName = "timezone", IsNullable = false)]
		public string Timezone { get; set; }

		[XmlElement(ElementName = "shop-owner", IsNullable = false)]
		public string ShopOwner { get; set; }

		[XmlElement(ElementName = "money-format", IsNullable = false)]
		public string MoneyFormat { get; set; }

		[XmlElement(ElementName = "money-with-currency-format", IsNullable = false)]
		public string MoneyWithCurrencyFormat { get; set; }

		[XmlElement(ElementName = "taxes-included", IsNullable = false)]
		public bool TaxesIncluded { get; set; }

		[XmlElement(ElementName = "tax-shipping", IsNullable = false)]
		public string TaxShipping { get; set; }

		[XmlElement(ElementName = "plan-name", IsNullable = false)]
		public AccountType PlanName { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// DO NOT USE. Required by XmlSerialization.
		/// </summary>
		public ShopifyShop()
		{
		}

		public ShopifyShop(string address1, string city, string country, DateTime createdAt,
				string domain, string email, int id, string name, string phone, string province,
				bool Public, string source, string zip, CurrencyCode currency,
				string timezone, string shopOwner, string moneyFormat, bool taxesIncluded,
				string taxShipping, AccountType planName)
		{
			this.Address1 = address1;
			this.City = city;
			this.Country = country;
			this.CreatedAt = createdAt;
			this.Domain = domain;
			this.Email = email;
			this.Id = id;
			this.Name = name;
			this.Phone = phone;
			this.Province = province;
			this.Public = Public;
			this.Source = source;
			this.Zip = zip;
			this.Currency = currency;
			this.Timezone = timezone;
			this.ShopOwner = shopOwner;
			this.MoneyFormat = moneyFormat;
			this.TaxesIncluded = taxesIncluded;
			this.TaxShipping = taxShipping;
			this.PlanName = planName;
		}
		#endregion

	}
}
