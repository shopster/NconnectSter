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
using log4net;
using Shopster.API.Service.SDK.Core;

namespace Shopsterify.Shopsterify
{

	public class ShopsterifyUser
	{
		private static ILog logger  = LogManager.GetLogger("Shopsterify");
		
		public ShopsterifyUser(int shopsterUser, string shopifyUser, DateTime sleepUntil)
		{
			this.CommonConstructor(shopsterUser, shopifyUser, sleepUntil);
		}
		
		public ShopsterifyUser(int shopsterUser, string shopifyUser)
		{
		
			this.CommonConstructor(shopsterUser,shopifyUser, DateTime.MinValue);
		}

		private void CommonConstructor(int shopsterUser, string shopifyUser, DateTime sleepUntil)
		{
			if (shopsterUser < 1)
			{

				logger.DebugFormat("shopsterUserId ({0}) must be >0 . ", shopsterUser );
				throw new ArgumentException("shopsterUserId must be >0 . ");
			}
			if (shopifyUser == null || shopifyUser == String.Empty || shopifyUser.Length > 255)
			{
				logger.DebugFormat("shopifyUser({0}) must not be null, cannot be empty and cannot exceed 255 characters", shopifyUser);
				throw new ArgumentException("shopifyUser must not be null, cannot be empty and cannot exceed 255 characters");
			}

			this.sleepUntil = sleepUntil;
			this.ShopsterUser = shopsterUser;
			this.ShopifyUser = shopifyUser;
		}

	
		public DateTime sleepUntil { get; set; }				
		public string ShopifyUser { get; set; }
		public int ShopsterUser { get; set; }
	}
}
