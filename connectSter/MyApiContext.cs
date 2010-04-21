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
using System.Web;
using Shopster.API.Service.SDK.Core;

namespace Shopsterify
{
	public class MyApiContext : ApiContext
	{
		public MyApiContext()
			: base()
		{
			ConsumerKey = "38BB2224-A128-4785-87D3-6AE256EE3053";
			ConsumerSecret = "6513A808-5B6A-4A71-89DA-85A46EFB92D3";
			EndpointConfigurationName = "MainBinding_IMain";
			

			Environment = ApiEnvironmentType.Sandbox;
			//Environment = ApiEnvironmentType.Custom;
			//OAuthEndpoint = new Uri("http://localhost/OAuth.ashx");
			//ServiceEndpoint = new Uri("http://localhost:2333/Main.svc");
		}
	}
}
