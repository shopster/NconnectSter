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
using System.Data;
using System.Data.Odbc;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;
using System.Xml;
using Shopsterify;

namespace ShopsterifyAuthentication
{
    public partial class _Default : Page
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

            if (nextIsOAuth())
            {
                //Todo: verify that signature is valid
                //Currently we just check to see that the token is usable. If so, pretty good chance its from shopify.
                if (testToken())
                {
                    doShopsterOAuth();
                }

                //Todo loging and errors
            }
            else if (nextIsBackToShopify())
            {
                ApiContext.FinishUserAuthorization();

                if (storeUserInDatabase())
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
                            Request.QueryString["uid"], Request.QueryString["oauth_verifier"],
                            Request.QueryString["oauth_token"]);
                }
            }
        }

        private bool storeUserInDatabase()
        {
            const string databaseIP = "127.0.0.1";
            const string databasePort = "3306";
            const string databaseName = "shopsterify";
            const string databaseUser = "root";
            const string databasePw = "99Mon.keys";
            const int FLAG_BIG_PACKETS = 8;
            const int FLAG_NO_PROMPT = 16;

            string conString = "DRIVER={MySQL ODBC 5.1 Driver};"
                               + "Server=" + databaseIP + ";"
                               + "PORT=" + databasePort + ";"
                               + "DATABASE=" + databaseName + ";"
                               + "UID=" + databaseUser + ";"
                               + "PWD=" + databasePw + ";"
                               + "OPTION=" + (FLAG_BIG_PACKETS + FLAG_NO_PROMPT) + ";";

            var dbConn = new OdbcConnection(conString);

            try
            {
                if (dbConn.State == ConnectionState.Closed)
                {
                    dbConn.Open();
                    if (dbConn.State == ConnectionState.Open)
                    {
                        //(in shopsterAuthToken CHAR(28), in shopsterAuthSecret CHAR(28), in shopsterAccountType INT(10),
                        //subdomain Varchar(255), in userStatus INT(4), in replicationLevel Int(4), in shopifyAuthToken Char(32), in shopifyAccountType INT(2))

                        string query =
                            String.Format(
                                "CALL InsertShopsterifyUser({0},'{1}','{2}',{3}, '{4}', {5},{6}, '{7}', {8});",
                                Request.QueryString["uid"], //{0} shopsterUserId
                                ApiContext.AccessToken, //{1} shopsterAuthToken
                                ApiContext.AccessTokenSecret, //{2} shopsterAuthSecret
                                1, //{3} shopsterAccountType
                                Request.QueryString["shop"].Replace(".myshopify.com", string.Empty), //{4} subdomain
                                1, //{5} userStatus
                                1, //{6} replicationLevel
                                Request.QueryString["t"], //{7} shopifyAuthToken
                                1 //{8} shopifyAccountType
                                );

                        var sqlCommand = new OdbcCommand(query, dbConn);
                        OdbcDataReader sqlResult = sqlCommand.ExecuteReader();
                        var res = sqlResult.Read();
                        if (res)
                        {
                            //So long as there wasnt an exception we're good. 
                            return true;
                        }
                    }
                }
            }
            catch (OdbcException dbEx)
            {
                throw;
            }

            return false;
        }

        protected void doShopsterOAuth()
        {
            var callback = new UriBuilder(Request.Url);
            callback.Query = String.Format("shop={0}&timestamp={1}&signature={2}&t={3}"
                                           , Request.QueryString["shop"], Request.QueryString["timestamp"],
                                           Request.QueryString["signature"], Request.QueryString["t"]);


            ApiContext = new MyApiContext();
            ApiContext.StartUserAuthorization(callback.Uri, null);
        }

        private bool nextIsOAuth()
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
        private bool testToken()
        {
            if (!nextIsOAuth()) //this may be redundant, but it helps avoid exceptions.
            {
                return false;
            }

            //Todo: add easyconfig and use the keys found in connectSter.conf 
            const string shopifyPublicKey = "6b40c288a08dcbdc3a2a336b60e054c9";
            const string shopifySecretKey = "12fdc51e8f5ab504cd7381ffb76999c7";
            //create XmlUrlResover so we can use authentication
            var urlResolver = new XmlUrlResolver();
            var cCache = new CredentialCache();
            string storePassword = HashString(shopifySecretKey + Request.QueryString["t"]);
            string path = "https://" + Request.QueryString["shop"] + "/admin/shop.xml";

            cCache.Add(new Uri(path), "Basic", new NetworkCredential(shopifyPublicKey, storePassword));
            urlResolver.Credentials = cCache;

            //Settings for the XmlReader
            var settings = new XmlReaderSettings();
            settings.XmlResolver = urlResolver;

            var xDoc = new XmlDocument();

            //Try to create the connection, create our Reader
//            try
//            {
                XmlReader xRead = XmlReader.Create(path, settings);
                xDoc.Load(xRead);

                xRead.Close();
                if (xDoc.DocumentElement.Name == "shop")
                    return true;
//            }
//            catch (Exception e)
//            {
//                throw;
                //Log the exception
                //logger.Error("ShopifyCommunicator::ShopifyGet() web exception: " + web.Message + " " + web.Response);
//            }


            return false;
        }


        /// <summary>
        /// Hashes(MD5) the given value. Typically used as "HashString(shopsterSecret + storeAuthToken)"
        /// </summary>
        /// <param name="Value">String to be hashed</param>
        /// <returns>Returns hex encoded password</returns>
        private string HashString(string Value)
        {
            var x = new MD5CryptoServiceProvider();
            byte[] data = Encoding.ASCII.GetBytes(Value);
            data = x.ComputeHash(data);
            string ret = "";
            for (int i = 0; i < data.Length; i++)
                ret += data[i].ToString("x2").ToLower();
            return ret;
        }
    }
}