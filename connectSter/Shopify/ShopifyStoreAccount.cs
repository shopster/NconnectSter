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

namespace Shopsterify
{
	class ShopifyStoreAccount
	{
		private string storeSubdomain;
		private string authToken;

		public ShopifyStoreAccount(string storeSubdomain, string authToken)
		{
			this.storeSubdomain = storeSubdomain;
			this.authToken = authToken;

		}

		public string StoreSubdomain
		{

			
			get { return storeSubdomain; }
			set {
				//blindly removes disallowed characters (':', '.', '/', '\')
				Regex reg = new Regex("[:./\\]*");
				string fixedValue = reg.Replace(value, "");
				storeSubdomain = fixedValue; }
		}

		public string AuthToken
		{
			get { return authToken; }
			set
			{
				VerifyStorePassword(value);
				authToken = value;
			}

		}


		#region methods

		/// <summary>
		/// Throws exceptions related to these common argument checks.
		/// </summary>
		/// <param name="storePassword"></param>
		/// <returns></returns>
		private static void VerifyStorePassword(string storePassword)
		{
			if (storePassword == null)
			{
				throw new ArgumentNullException("storePassword cannot be null");
			}

			//This section ensures that it is lowercase hex for the whole array.
			bool is_hex_char;
			int length = 0;
			foreach (char current_char in storePassword.ToCharArray())
			{
				length++;
				is_hex_char = (current_char >= '0' && current_char <= '9') ||
				   (current_char >= 'a' && current_char <= 'f');

				if (!is_hex_char)
				{
					throw new ArgumentException("storePassword must be 32 lowercase char hexadecimal encoded value");
				}

			}

			if (length < 32)
			{
				throw new ArgumentException("storePassword must be 32 char hexadecimal encoded value");
			}

			return;
		}

		#endregion
	}
}
