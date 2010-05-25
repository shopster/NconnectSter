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
using System.Linq;
using System.Xml.Serialization;
using Connectster.Server.Interfaces;
using Connectster.Shopify.Interfaces;

namespace Connectster.Shopify
{
	[Serializable]
	public class ShopifyAddress :IAddress , IShopifyObject
	{
		
		#region Ctors 
		/// <summary>
		/// DO NOT USE. Required for XML Serialization.
		/// </summary>
		public ShopifyAddress()
		{ }

		public ShopifyAddress(IAddress inAddress)
		{
			this.Address1 = inAddress.Street1;
			this.Address2 = inAddress.Street2;
			this.City = inAddress.City;
			this.Company = inAddress.Name;
			this.Country = Enumerable.FirstOrDefault<string>(CountryCodes.Instance().countryToCode.Where(Item => Item.Value == inAddress.CountryCode).Select(Item => Item.Key));
			this.CountryCode = inAddress.CountryCode;
			string[] names =  inAddress.Name.Split(' ');
			if(names!=null && names.Count()>0)
			{	
				this.FirstName = names[0];
				//the remainder of the string is the last name.
				foreach (string name in names.Where(item => item!= FirstName).Select(item=>item).ToArray<string>())
				{
					this.LastName += (name + " ");
				}
				LastName.TrimEnd(' '); //remove the trailing ' '
			}
			else
			{
				this.FirstName = "Unnamed";
				this.LastName = "Unnamed";
			}

			this.Name = inAddress.Name;

			this.PhoneNumber = "";
			this.Province = inAddress.Region;
			this.ProvinceCode = inAddress.Region;
			this.Zip = inAddress.PostalCode;
		}


		/// <summary>
		/// Create a ShopifyAddress w/ all available parameters
		/// </summary>
		/// <param name="address1"></param>
		/// <param name="address2"></param>
		/// <param name="city"></param>
		/// <param name="company"></param>
		/// <param name="country"></param>
		/// <param name="firstName"></param>
		/// <param name="lastName"></param>
		/// <param name="phoneNumber"></param>
		/// <param name="province"></param>
		/// <param name="zip"></param>
		/// <param name="name"></param>
		/// <param name="countryCode"></param>
		/// <param name="provinceCode"></param>
		public ShopifyAddress(string address1, string address2, string city, string company,
			string country, string firstName, string lastName, string phoneNumber, string province,
			string zip, string name, string countryCode, string provinceCode)
		{
			this.Address1 = address1;
			this.Address2 = address2;
			this.City = city;
			this.Company = company;
			this.Country = country;
			this.FirstName = firstName;
			this.LastName = lastName;
			this.PhoneNumber = phoneNumber;
			this.Province = province;
			this.Zip = zip;
			this.Name = name;
			this.CountryCode = countryCode;
			this.ProvinceCode = provinceCode;
		}
		#endregion


		#region IShopifyAddress Members
		[XmlElement(ElementName = "address1", IsNullable = false)]
		public string Address1{ get; set ;}

		[XmlElement(ElementName = "address2", IsNullable = true)]
		public string Address2{ get; set ;}

		[XmlElement(ElementName = "city", IsNullable = false)]
		public string City{ get; set ;}

		[XmlElement(ElementName = "company", IsNullable = true)]
		public string Company{ get; set ;}
		
		[XmlElement(ElementName = "country", IsNullable = false)]
		public string Country{ get; set ;}

		[XmlElement(ElementName = "first-name", IsNullable = false)]
		public string FirstName{ get; set ;}

		[XmlElement(ElementName = "last-name", IsNullable = false)]
		public string LastName{ get; set ;}

		[XmlElement(ElementName = "phone", IsNullable = false)]
		public string PhoneNumber{ get; set ;}

		[XmlElement(ElementName = "province", IsNullable = false)]
		public string Province{ get; set ;}

		[XmlElement(ElementName = "zip", IsNullable = false)]
		public string Zip{ get; set ;}

		[XmlElement(ElementName = "name", IsNullable = false)]
		public string Name{ get; set ;}

		[XmlElement(ElementName = "country-code", IsNullable = false)]
		public string CountryCode{ get; set ;}

		[XmlElement(ElementName = "province-code", IsNullable = false)]
		public string ProvinceCode { get; set; }
		#endregion





		#region IAddress Members
		public string Street1
		{
			get
			{
				return Address1;
			}
			set
			{
				Address1 = value;
			}
		}

		public string Street2
		{
			get
			{
				return (Address2!=null && Address2.Length>0)? Address2 : null;
			}
			set
			{
				Address2 = value;
			}
		}

		public string Region
		{
			get
			{
				return ProvinceCode;
			}
			set
			{
				ProvinceCode = value;
			}
		}

		public string PostalCode
		{
			get
			{
				return Zip;
			}
			set
			{
				Zip = value;
			}
		}

		#endregion
	}
}
