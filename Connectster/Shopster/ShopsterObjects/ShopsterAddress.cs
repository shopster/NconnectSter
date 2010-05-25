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

using System.Linq;
using Connectster.Server.Interfaces;
using Shopster.API.Service.SDK.Core.Soap;

namespace Connectster.Shopster.ShopsterObjects
{
	class ShopsterAddress : IAddress
	{
		internal AddressType address;
		
		//These match the AddressType
		public string Title
		{
			get
			{
				return address.Title ;
			}
			set
			{
				if (value != null && value.Length > 0)
				{
					value = value.Trim();
					address.Title = value.Length > 5 ? value.Substring(0, 5) : value;
				}
				else //todo refactor to allow empty titles
				{
					address.Title = "Dr.";
				}

				
			} //Mr, Mrs, max length = 5
		}
		public string Country
		{
			get
			{
				return address.Country;
			}
			set
			{
				value = value.Trim();
				address.Country = value.Length > 2 ? value.Substring(0, 2) : value;
				
			}
		}

		public ShopsterAddress()
		{
			address = new AddressType();
		}

		public ShopsterAddress(IAddress inAddress)
		{
			this.City = inAddress.City;
			//Lookup the country related to this code.
			this.Country = Enumerable.FirstOrDefault<string>(CountryCodes.Instance().countryToCode.Where(country => country.Value == inAddress.CountryCode).Select(country => country.Key));
			this.CountryCode = inAddress.CountryCode;
			this.Name = inAddress.Name;
			this.PostalCode = inAddress.PostalCode;
			this.Region = inAddress.Region;
			this.Street1 = inAddress.Street1;
			this.Street2 = inAddress.Street2;
			this.Title = "Mr." ;
		}


		#region IAddress Members
		public string Name
		{
			get
			{
				return address.Name;
			}
			set
			{
				value = value.Trim();
				address.Name = value.Length > 50 ? value.Substring(0, 50) : value;
			}
		}

		public string Street1
		{
			get
			{
				return address.Street1;
			}
			set
			{
				value = value.Trim();
				address.Street1 = value.Length > 50 ? value.Substring(0, 50) : value;
			}
		}

		public string Street2
		{
			get
			{
				return address.Street2;
			}
			set
			{
				if (value != null)
				{
					value = value.Trim();
					address.Street2 = value.Length > 50 ? value.Substring(0, 50) : value;
				}
				else
				{
					address.Street2 = value;
				}



				
			}
		}

		public string City
		{
			get
			{
				return address.City;
			}
			set
			{
				value = value.Trim();
				address.City = value.Length > 50 ? value.Substring(0, 50) : value;
			}
		}

		public string Region
		{
			get
			{
				return address.Region;
			}
			set
			{
				value = value.Trim();
				address.Region = value.Length > 50 ? value.Substring(0, 50) : value;
			}
		}
		public string PostalCode
		{
			get
			{
				return address.PostalCode;
			}
			set
			{
				value = value.Trim();
				address.PostalCode = value.Length > 10 ? value.Substring(0, 10) : value;
			}
		}

		public string CountryCode
		{
			get
			{
				return address.Country;
			}
			set
			{
				value = value.Trim();
				address.Country = value.Length > 2 ? value.Substring(0, 2) : value;
			}
		}
		#endregion
	
	}
}
