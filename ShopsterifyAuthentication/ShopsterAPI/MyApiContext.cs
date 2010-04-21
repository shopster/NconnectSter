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
