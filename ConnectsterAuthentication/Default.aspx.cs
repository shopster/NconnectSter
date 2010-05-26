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
using System.Configuration;
using System.Data;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;
using System.Xml;
using MySql.Data.MySqlClient;

namespace ConnectsterAuthentication
{
    public partial class Default : Page
    {
        protected MyApiContext ApiContext
        {
            get { return (MyApiContext) Session["WcfApiContext"]; }
            set { Session["WcfApiContext"] = value; }
        }

        protected void OnPreRender(object sender, EventArgs e)
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.QueryString.HasKeys()) return;

            if (NextIsOAuth())
            {
                //Todo: verify that signature is valid
                //Currently we just check to see that the token is usable. If so, pretty good chance its from shopify.
                if (TestToken())
                {
                    DoShopsterOAuth();
                }

                //Todo loging and errors
            }
            else if (NextIsBackToShopify())
            {
                ApiContext.FinishUserAuthorization();

                if (StoreUserInDatabase())
                {
                    Response.Redirect("https://" + Request.QueryString["shop"] + "/admin/applications");
                }
                else
                {
                    //Todo logging and error message, and to pretty this page. 
                    myDiv.InnerHtml =
                        String.Format(
                            @"An Error Occured. Please email techsupport@site.com and copy the following into the email body.<br>
						<p><ul>
						<li>shopifyDomain={0}</li>
						<li>t={1}</li>
						<li>uid={2}</li>
						<li>oauth_verifier={3}</li>
						<li>oatuh_token={4}</li>
						</ul></p>
						",
                            Request.QueryString["shop"], Request.QueryString["t"], Request.QueryString["uid"],
                            Request.QueryString["uid"], Request.QueryString["oauth_verifier"]);
                }
            }
        }

        private bool StoreUserInDatabase()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["connectster"].ConnectionString;
            var dbConn = new MySqlConnection(connectionString);

            if (dbConn.State == ConnectionState.Closed)
            {
                dbConn.Open();
                if (dbConn.State == ConnectionState.Open)
                {
                    //(in shopsterAuthToken CHAR(28), in shopsterAuthSecret CHAR(28), in shopsterAccountType INT(10),
                    //subdomain Varchar(255), in userStatus INT(4), in replicationLevel Int(4), in shopifyAuthToken Char(32), in shopifyAccountType INT(2))
                    var shopifyDomain = ConfigurationManager.AppSettings["ShopifyDomain"];

                    var query =
                        String.Format(
                            "CALL InsertConnectsterUser({0},'{1}','{2}',{3}, '{4}', {5},{6}, '{7}', {8});",
                            Request.QueryString["uid"], //{0} shopsterUserId
                            ApiContext.AccessToken, //{1} shopsterAuthToken
                            ApiContext.AccessTokenSecret, //{2} shopsterAuthSecret
                            1, //{3} shopsterAccountType
                            Request.QueryString["shop"].Replace(shopifyDomain, string.Empty), //{4} subdomain
                            1, //{5} userStatus
                            1, //{6} replicationLevel
                            Request.QueryString["t"], //{7} shopifyAuthToken
                            1 //{8} shopifyAccountType
                            );

                    var sqlCommand = new MySqlCommand(query, dbConn);
                    var sqlResult = sqlCommand.ExecuteReader();

                    if (sqlResult.Read())
                    {
                        //So long as there wasnt an exception we're good. 
                        return true;
                    }
                }
            }


            return false;
        }

        protected void DoShopsterOAuth()
        {
            var callback = new UriBuilder(Request.Url)
                               {
                                   Query = String.Format("shop={0}&timestamp={1}&signature={2}&t={3}"
                                                         , Request.QueryString["shop"], Request.QueryString["timestamp"],
                                                         Request.QueryString["signature"], Request.QueryString["t"])
                               };


            ApiContext = new MyApiContext();
            ApiContext.StartUserAuthorization(callback.Uri, null);
        }

        private bool NextIsOAuth()
        {
            if (Request.QueryString["shop"] != null
                && Request.QueryString["timestamp"] != null
                && Request.QueryString["signature"] != null
                && Request.QueryString["t"] != null
                && Request.QueryString["t"].Length == 32
                && Request.QueryString.Keys.Count == 4
                )
            {
                return true;
            }

            return false;
        }

        private bool NextIsBackToShopify()
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
                && Request.QueryString["t"].Length == 32
                )

            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tests that "t" is a valid key by making an API call. 
        /// </summary>
        /// <returns></returns>
        private bool TestToken()
        {
            if (!NextIsOAuth()) //this may be redundant, but it helps avoid exceptions.
            {
                return false;
            }

            var shopifyPublicKey = ConfigurationManager.AppSettings["ShopifyAppAuthApiKey"];
            var shopifySecretKey = ConfigurationManager.AppSettings["ShopifyAppAuthApiSharedSecret"];
            var shopifyProtocol = ConfigurationManager.AppSettings["ShopifyProtocol"];

            //create XmlUrlResover so we can use authentication
            var urlResolver = new XmlUrlResolver();
            var cCache = new CredentialCache();
            var storePassword = HashString(shopifySecretKey + Request.QueryString["t"]);
            var path = String.Format("{0}{1}/admin/shop.xml", shopifyProtocol, Request.QueryString["shop"]) ;

            cCache.Add(new Uri(path), "Basic", new NetworkCredential(shopifyPublicKey, storePassword));
            urlResolver.Credentials = cCache;

            //Settings for the XmlReader
            var settings = new XmlReaderSettings();
            settings.XmlResolver = urlResolver;

            var xDoc = new XmlDocument();

            //Try to create the connection, create our Reader
            XmlReader xRead = XmlReader.Create(path, settings);
            xDoc.Load(xRead);

            xRead.Close();
            return xDoc.DocumentElement != null && xDoc.DocumentElement.Name == "shop";
        }


        /// <summary>
        /// Hashes(MD5) the given value. Typically used as "HashString(shopsterSecret + storeAuthToken)"
        /// </summary>
        /// <param name="value">String to be hashed</param>
        /// <returns>Returns hex encoded password</returns>
        private static string HashString(string value)
        {
            var x = new MD5CryptoServiceProvider();
            byte[] data = Encoding.ASCII.GetBytes(value);
            data = x.ComputeHash(data);
            string ret = "";
            for (int i = 0; i < data.Length; i++)
                ret += data[i].ToString("x2").ToLower();
            return ret;
        }
    }
}