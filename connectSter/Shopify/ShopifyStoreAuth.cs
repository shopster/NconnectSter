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

namespace Shopsterify
{
	public class ShopifyStoreAuth
	{
		//Test store tokens
		//private string storeAuthToken = "e6c6bc7978e4eb5f3b6bd7eafe813d04";
		//private string storeSubDomain = "cremin-bayer-and-carter5962";
		

		private string storeAuthToken;
		private string storeSubDomain;
		
		public ShopifyStoreAuth(string storeAuthToken, string storeSubDomain)
		{
			if (storeAuthToken == null || storeSubDomain == null)
			{
				throw new ArgumentNullException("ShopifyStoreAuth fields cannot be null");
			}

			this.storeAuthToken = storeAuthToken;
			this.storeSubDomain = storeSubDomain;
		}

		public string StoreAuthToken
		{
			get { return storeAuthToken;}
			set {
				if (value == null)
				{
					throw new ArgumentNullException("ShopifyStoreAuth::storeAuthToken cannot be null");
				}
				storeAuthToken = value;}
		}

		public string StoreSubDomain
		{
			get { return storeSubDomain; }
			set {
				if (value == null)
				{
					throw new ArgumentNullException("ShopifyStoreAuth::storeSubDomain cannot be null");
				}
			
				storeSubDomain = value; 
			
			}
		}
	}
}
