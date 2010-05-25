﻿//	Copyright 2010 Shopster E-Commerce Inc.
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
	public class ShopifyAppAuth
	{

		private string user; //Shopify API Key
		private string secret; //Shopify Secret Key

		public ShopifyAppAuth(string user, string secret)
		{
			if (user == null || secret == null)
			{
				throw new ArgumentNullException("ShopifyAppAuth fields cannot be null");
			}

			this.user = user;
			this.secret = secret;
		}

		public string User
		{
			get { return user; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("ShopifyAppAuth::user cannot be null");
				}
				user = value; 
			}
		}

		public string Secret
		{
			get { return secret; }
			set {
				if (value == null)
				{
					throw new ArgumentNullException("ShopifyAppAuth::secret cannot be null");
				}
				secret = value; 
			}
		}

	}
}