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
using Shopsterify.Shopsterify;
using EasyConfig;

namespace Shopsterify
{
	public class MyApiContext : ApiContext
	{
		static ConfigurationManager configManager = ConfigurationManager.Instance();
		public MyApiContext()
			: base()
		{
			SettingsGroup config = configManager.getSettings("MyApiContext");
			
			ConsumerKey = config.Settings["ConsumerKey"].GetValueAsString();
			ConsumerSecret = config.Settings["ConsumerSecret"].GetValueAsString();
			EndpointConfigurationName = config.Settings["EndpointConfigurationName"].GetValueAsString();

			Environment = ApiEnvironmentType.Sandbox;
			
			//Environment = ApiEnvironmentType.Custom;
			//OAuthEndpoint = new Uri("http://localhost/OAuth.ashx");
			//ServiceEndpoint = new Uri("http://localhost:2333/Main.svc");
		}
	}
}
