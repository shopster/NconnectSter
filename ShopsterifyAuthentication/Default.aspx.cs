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
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Shopsterify;
using System.IO;
using System.Collections.Generic;
using System.Data.Odbc;

namespace ShopsterifyAuthentication
{
	public partial class _Default : System.Web.UI.Page
	{

		protected MyApiContext ApiContext
		{
			get { return (MyApiContext)Session["WcfApiContext"]; }
			set { Session["WcfApiContext"] = value; }
		}

		protected void OnPreRender(object sender, EventArgs e)
		{

		}
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Request.QueryString.HasKeys())
			{
				
				if ( nextIsBackToShopify())
				{
					ApiContext.FinishUserAuthorization();
					
					if(storeUserInDatabase())
					{

						Response.Redirect("https://" + Request.QueryString["shop"] + "/admin/applications");
					}
					else
					{
						//Todo logging and error message
						myDiv.InnerHtml =String.Format(@"An Error Occured. Please email techsupport@site.com and copy the following into the email body.<br>
						<p><ul>
						<li>shopifyDomain={0}</li>
						<li>t={1}</li>
						<li>uid={2}</li>
						<li>oauth_verifier={3}</li>
						<li>oatuh_token={4}</li>
						</ul></p>
						", Request.QueryString["shop"], Request.QueryString["t"], Request.QueryString["uid"],
						 Request.QueryString["uid"], Request.QueryString["oauth_verifier"], Request.QueryString["oauth_token"]);
						
					}
				}
				else if ( nextIsOAuth() )
				{

					//Todo: verify that signature == our app auth. (stored in config)
	
					this.doShopsterOAuth();
				}
			}

		}

		private bool storeUserInDatabase()
		{
			const string databaseIP = "127.0.0.1";
			const string databasePort = "3306";
			const string databaseName = "shopsterify";
			const string databaseUser = "root";
			const string databasePw = "123.123";
			const int FLAG_BIG_PACKETS = 8;
			const int FLAG_NO_PROMPT = 16;
			
			//Todo more config file strings to add. 
			string conString = "DRIVER={MySQL ODBC 5.1 Driver};"
			+ "Server=" + databaseIP + ";"
			+ "PORT=" + databasePort + ";"
			+ "DATABASE=" + databaseName + ";"
			+ "UID=" + databaseUser + ";"
			+ "PWD=" + databasePw + ";"
			+ "OPTION=" + (FLAG_BIG_PACKETS + FLAG_NO_PROMPT).ToString() + ";";

			OdbcConnection dbConn = new OdbcConnection(conString);

			try
			{
				if (dbConn.State == ConnectionState.Closed)
				{
					dbConn.Open();
					if (dbConn.State == ConnectionState.Open)
					{
						//(in shopsterAuthToken CHAR(28), in shopsterAuthSecret CHAR(28), in shopsterAccountType INT(10),
						//subdomain Varchar(255), in userStatus INT(4), in replicationLevel Int(4), in shopifyAuthToken Char(32), in shopifyAccountType INT(2))

						string query = String.Format("CALL InsertShopsterifyUser({0},'{1}','{2}',{3}, '{4}', {5},{6}, '{7}', {8});",
							Request.QueryString["uid"], //{0} shopsterUserId
							ApiContext.AccessToken,  //{1} shopsterAuthToken
							ApiContext.AccessTokenSecret, //{2} shopsterAuthSecret
							1,  //{3} shopsterAccountType
							Request.QueryString["shop"].Replace(".myshopify.com", string.Empty),  //{4} subdomain
							1, //{5} userStatus
							1, //{6} replicationLevel
							Request.QueryString["t"], //{7} shopifyAuthToken
							1 //{8} shopifyAccountType
							);

						OdbcCommand sqlCommand = new OdbcCommand(query, dbConn);
						OdbcDataReader sqlResult = sqlCommand.ExecuteReader();
						if (sqlResult.Read())
						{
							//So long as there wasnt an exception we're good. 
								return true;
							
						}
					}
				}
			}
			catch (OdbcException dbEx)
			{
				Console.WriteLine(dbEx.Message);
			}

			return false;
		}

		



		protected void doShopsterOAuth()
		{
			var callback = new UriBuilder(Request.Url);
			callback.Query = String.Format("shop={0}&timestamp={1}&signature={2}&t={3}"
				,Request.QueryString["shop"],Request.QueryString["timestamp"],Request.QueryString["signature"], Request.QueryString["t"]);
		

			ApiContext = new MyApiContext();
			ApiContext.StartUserAuthorization(callback.Uri,null);
	
		}
		private bool nextIsOAuth()
		{
			if (Request.QueryString["shop"] != null 
				&& Request.QueryString["timestamp"] != null
				&& Request.QueryString["signature"] != null 
				&& Request.QueryString["t"] != null
				&& Request.QueryString.Keys.Count == 4)
			{
				return true;
			}

			return false;
		}
		private bool nextIsBackToShopify()
		{
			if (
				Request.QueryString["oauth_verifier"] != null
					&& Request.QueryString["oauth_token"] != null
					&& Request.QueryString["shop"] != null
					&& Request.QueryString["timestamp"] != null
					&& Request.QueryString["signature"] != null
					&& Request.QueryString["t"] != null
					&& Request.QueryString["uid"] != null
					&& Request.QueryString.Keys.Count == 7
				)

			{
				return true;
			}
			return false;


		}
	}
}
