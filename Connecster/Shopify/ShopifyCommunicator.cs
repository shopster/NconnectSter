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
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using log4net;
using Shopsterify.Shopify;
using System.Web;
using EasyConfig;
using Shopsterify.Shopsterify;

namespace Shopsterify
{
	public class ShopifyCommunicator :IShopifyObject
	{
		// Give them some default values.
		private string domain = ".myshopify.com";
		private string protocol = "https://";
		
		private static ShopifyCommunicator instance;
		private static ShopifyAppAuth appAuth;
		
		private static ILog apiLogger = log4net.LogManager.GetLogger("ShopifyAPICommunications");
		private static ILog logger = log4net.LogManager.GetLogger("ShopifyCommunicator");
		private static ConfigurationManager configs;
		static ShopifyCommunicator()
		{
			configs = ConfigurationManager.Instance();
			instance = new ShopifyCommunicator();

		}

		private ShopifyCommunicator()
		{
			
				//Todo: move this into some form of protected file or something.
				SettingsGroup settings = configs.getSettings("ShopifyCommunicator");
				appAuth = new ShopifyAppAuth(settings.Settings["appAuth_APIKey"].GetValueAsString(), settings.Settings["appAuth_APISharedSecret"].GetValueAsString());
				domain = settings.Settings["domain"].GetValueAsString();
				protocol = settings.Settings["protocol"].GetValueAsString();
			
			
				
		}

		public static ShopifyCommunicator Instance()
		{
			return instance;
		}

		
		

		#region shopifyQueries
		public ShopifyResponse<ShopifyCollection> CreateCollection(ShopifyStoreAuth storeAuth, ShopifyCollection collection)
		{
			if (storeAuth == null || collection == null)
			{
		
				logger.Error("ShopifyCommunicator::CreateCollection(): storeAuth and collection cannot be null.");
				return null;
			}

			XmlSerializer xS = new XmlSerializer(typeof(ShopifyCollection));
			XmlAttributeOverrides xO = new XmlAttributeOverrides();

			MemoryStream memStream = new MemoryStream();
			XmlDocument xDoc = new XmlDocument();

			xS.Serialize(memStream, collection);
			memStream.Seek(0, System.IO.SeekOrigin.Begin);
			xDoc.Load(memStream);
			memStream.Close();
			
			
			return new ShopifyResponse<ShopifyCollection>(ShopifyPutPost((protocol + storeAuth.StoreSubDomain + domain + "/admin/custom_collections.xml"),
				HashString(appAuth.Secret + storeAuth.StoreAuthToken), xDoc, "POST"));
		}

		public ShopifyResponse<ShopifyCollection> GetCollectionByName(ShopifyStoreAuth storeAuth, string name)
		{
			if (storeAuth == null || name == null || name==string.Empty)
			{
				logger.Error("ShopifyCommunicator::GetCollectionByName(): storeAuth and name cannot be null. name cannot be empty");
				return null;
			}

			return new ShopifyResponse<ShopifyCollection>(ShopifyGet((protocol + storeAuth.StoreSubDomain + domain + "/admin/custom_collections.xml?name=" + HttpUtility.UrlEncode(name)),
				HashString(appAuth.Secret + storeAuth.StoreAuthToken))); 

		}

		public ShopifyResponse<ShopifyCollection> GetCollection(ShopifyStoreAuth storeAuth, int collectionId)
		{
			if (storeAuth == null || collectionId <= 0)
			{
				logger.Error("ShopifyCommunicator::GetCollection(): storeAuth cannot be null and collectionId must be >0");
				return null;
			}

			return new ShopifyResponse<ShopifyCollection>(ShopifyGet((protocol + storeAuth.StoreSubDomain + domain + "/admin/custom_collections/" + collectionId.ToString() + ".xml"),
				HashString(appAuth.Secret + storeAuth.StoreAuthToken)));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="storeAuth"></param>
		/// <param name="limit"></param>
		/// <param name="createdMin"></param>
		/// <param name="createdMax"></param>
		/// <param name="updatedMin"></param>
		/// <param name="updatedMax"></param>
		/// <param name="fieldNamespace"></param>
		/// <param name="key"></param>
		/// <param name="valueType"></param>
		/// <param name="productId">If productId is >0 get all metafields for that product, if null get metafields tagged to the shop. </param>
		/// <returns></returns>
		public List<ShopifyMetafield> GetAllMetafields(ShopifyStoreAuth storeAuth, int? limit, 
			DateTime? createdMin, DateTime? createdMax, DateTime? updatedMin, DateTime? updatedMax, 
			string fieldNamespace, string key, string valueType, int? productId)
		{ //Todo: Should we make this take a "dictionary" and then simply do "foreach key in dictionary"
		
	
		//Verify various bits. 
			if (!(valueType == "string" || valueType == "integer"))
			{
				throw new ArgumentException("valueType can only be 'string' or 'integer'");
			}
			if (limit > 250 || limit < 0)
			{
				throw new ArgumentException("limit must be between 1 and 250 inclusive");
			}
			if (storeAuth == null)
			{
				throw new ArgumentNullException("storeAuth cannot be null");
			}
			if (productId==null && productId < 1)
			{
				throw new ArgumentException("productId must be >0");
			}

			List<ShopifyMetafield> returnList = limit != null ? new List<ShopifyMetafield>((int)limit) : new List<ShopifyMetafield>();

			StringBuilder sbUrl = new StringBuilder();
			sbUrl.Append(protocol + storeAuth.StoreSubDomain + domain + "/admin/");
			
			if (productId != null)
			{
				sbUrl.Append("products/" + productId.ToString() + "/");
			}
			sbUrl.Append("metafields.xml?");
			//Append each of the filter terms.
			if (limit != null)
			{
				sbUrl.Append("limit="+limit.ToString()); 
			}
			if (createdMin != null)
			{
				sbUrl.Append("&created_at_min="+HttpUtility.UrlEncode(DateTimeToShopifyString((DateTime)createdMin)));
			}
			if (createdMax != null)
			{
				sbUrl.Append("&created_at_max=" + HttpUtility.UrlEncode(DateTimeToShopifyString((DateTime)createdMax)));
			}
			if (updatedMin!= null)
			{
				sbUrl.Append("&updated_at_min=" + HttpUtility.UrlEncode(DateTimeToShopifyString((DateTime)updatedMin)));
			}
			if (updatedMax != null)
			{
				sbUrl.Append("&updated_at_max=" + HttpUtility.UrlEncode(DateTimeToShopifyString((DateTime)updatedMax)));
			}
			if (!(key == null || key == string.Empty))
			{
				sbUrl.Append("&key=" + HttpUtility.UrlEncode(key));
			}
			if (!(fieldNamespace == null || fieldNamespace == string.Empty))
			{
				sbUrl.Append("&namespace=" + HttpUtility.UrlEncode(fieldNamespace));
			}
			if (!(valueType == null || valueType == string.Empty))
			{
				sbUrl.Append("&value_type=" + HttpUtility.UrlEncode(valueType));
			}

			XmlDocument xDoc = ShopifyGet(sbUrl.ToString().Replace("?&","?"),
				HashString(appAuth.Secret + storeAuth.StoreAuthToken));

			if (xDoc == null)
			{
				//ShopifyGet should be logging/reporting the error
				return null;
			}
			XmlNodeList metaFields = xDoc.SelectNodes("//metafields/metafield");

			foreach (XmlNode metaField in metaFields)
			{
				XmlDocument metaFieldDoc = new XmlDocument();
				metaFieldDoc.AppendChild(metaFieldDoc.ImportNode(metaField, true));
				ShopifyResponse<ShopifyMetafield> serializedResponse = new ShopifyResponse<ShopifyMetafield>(metaFieldDoc);

				
				if (serializedResponse.State == ResponseState.OK)
				{
					returnList.Add(serializedResponse.ResponseObject);
				}
				else
				{
					//Log this error, ignore this metafield and continue with call so we can return the properly formatted metafields
					XmlNodeList idNode = metaField.SelectNodes("//id");
					string metafieldId = idNode.Count > 0 ? idNode[0].InnerText : "null";
					logger.ErrorFormat("ShopifyCommunicator()::GetAllMetafields(): Couldn't parse metafield ({0}) returned in page of metafields.", metafieldId);

				}

			}
			return returnList;

		}

		public ShopifyResponse<ShopifyMetafield> CreateMetafield(ShopifyStoreAuth storeAuth, ShopifyMetafield metaField, int? productId)
		{
			if (storeAuth == null || metaField == null)
			{
				logger.WarnFormat("ShopifyCommunicator::CreateMetafield():storeAuth and metaField cannot be null");
				return null;
			}
			if (productId != null && productId < 1) //Product ID can be null if the metafield is tagged to the store itself (vs. tagged to a product).
			{
				
				logger.ErrorFormat("ShopifyCommunicator::CreateMetafield(): productId must be >=1 if non-null.");
				return null;
				
			}

			//Build our url depending on if this metafield will apply to e product or e shop
			StringBuilder url = new StringBuilder();
			url.Append(protocol + storeAuth.StoreSubDomain + domain + "/admin/");
			
			if (productId != null)
			{
				url.Append("products/"+productId.ToString() + "/");
			}
			url.Append("metafields.xml");

			XmlSerializer xS = new XmlSerializer(typeof(ShopifyMetafield));
			
			MemoryStream memStream = new MemoryStream();
			XmlDocument xDoc = new XmlDocument();

			xS.Serialize(memStream, metaField);
			memStream.Seek(0, System.IO.SeekOrigin.Begin);
			xDoc.Load(memStream);
			memStream.Close();


			return new ShopifyResponse<ShopifyMetafield>(ShopifyPutPost(url.ToString(),
				HashString(appAuth.Secret + storeAuth.StoreAuthToken), xDoc,"POST"));

}


		

		public ShopifyResponse<ShopifyProduct> GetProduct(ShopifyStoreAuth storeAuth, int productId)
		{

			if(productId < 0)
			{
				logger.ErrorFormat("ShopifyCommunicator::GetProduct() was passed a negative productId ({0}), returning null.",productId);
				return null;
			}
			
			XmlDocument xDoc = ShopifyGet((protocol + storeAuth.StoreSubDomain + domain + "/admin/products/" + productId.ToString() + ".xml"),
				HashString(appAuth.Secret + storeAuth.StoreAuthToken));
						
			return new ShopifyResponse<ShopifyProduct>(xDoc);
			
			
		}


		public List<ShopifyProduct> GetAllProducts(ShopifyStoreAuth storeAuth)
		{

			List<ShopifyProduct> returnList = new List<ShopifyProduct>(250);
			int pageNumber=1; 

			while (true)
			{
				List<ShopifyProduct> tempList = GetPageOfProducts(storeAuth, 250, pageNumber++);
				if (tempList != null)
				{

					returnList = returnList.Union<ShopifyProduct>(tempList).ToList<ShopifyProduct>();
					if (tempList.Count < 250)
					{
						break;
					}
				}
			}

			return returnList;
		}

		
		public List<ShopifyProduct> GetPageOfProducts(ShopifyStoreAuth storeAuth, int numWanted, int pageNumber)
		{
			if (storeAuth == null)
			{
				logger.Warn("ShopifyCommunicator::GetPageOfProducts: storeAuth cannot be null");
				return null;
			}

			if (!(numWanted > 0 && numWanted < 251 && pageNumber > 0))
			{
				logger.WarnFormat("ShopifyCommunicator::GetPageOfProducts(): numWanted({0}) must be between 0 and 251, pageNumber({1}) must be >0.", numWanted, pageNumber);
				return null;
			}

			StringBuilder url = new StringBuilder();
			url.Append(protocol);
			url.Append(storeAuth.StoreSubDomain);
			url.Append(domain);
			url.Append("/admin/products.xml?");

			url.Append("limit=" + numWanted.ToString());
			url.Append("&page=" + pageNumber.ToString());

			XmlDocument xDoc = ShopifyGet(url.ToString(), HashString(appAuth.Secret + storeAuth.StoreAuthToken));
			//Now the fun part of parsing this xml. 
			if (xDoc == null)
			{
				return null;
			}

			XmlNodeList productNodeList = xDoc.SelectNodes("//products/product");

			List<ShopifyProduct> returnList = new List<ShopifyProduct>();
//			Foreach product found, serialize and add to return list
			foreach(XmlNode productNode in productNodeList)
			{
				XmlDocument productDoc = new XmlDocument();
				productDoc.AppendChild(productDoc.ImportNode(productNode, true));
				ShopifyResponse<ShopifyProduct> serializedResponse = new ShopifyResponse<ShopifyProduct>(productDoc);
				
				
				if (serializedResponse.State == ResponseState.OK)
				{
					returnList.Add(serializedResponse.ResponseObject);
				}
				else
				{
					XmlNodeList idNode = productNode.SelectNodes("//id");
					string productId = idNode.Count > 0 ? idNode[0].InnerText : "null";
					logger.ErrorFormat("ShopifyCommunicator()::GetPageOfProducts(): Couldn't parse product ({0}) returned in page of product.", productId);
				}
				

			}
			
			return returnList;
		}
		
		public ShopifyResponse<ShopifyProduct> CreateProduct(ShopifyStoreAuth storeAuth, IShopifyProduct sP)
		{
			//We serialize the ShopifyProduct into and XMLDocument, then POST it to shopify.
			XmlSerializer xS = new XmlSerializer(typeof(ShopifyProduct));
			XmlAttributeOverrides xO = new XmlAttributeOverrides();
			
			MemoryStream memStream = new MemoryStream();
			XmlDocument xDoc = new XmlDocument();

			xS.Serialize(memStream, sP);
			memStream.Seek(0, System.IO.SeekOrigin.Begin);
			xDoc.Load(memStream);
			memStream.Close();
			return new ShopifyResponse<ShopifyProduct>(ShopifyPutPost((protocol + storeAuth.StoreSubDomain + domain + "/admin/products.xml"),
				HashString(appAuth.Secret + storeAuth.StoreAuthToken), xDoc,"POST"));
		}


		public bool DeleteProduct(ShopifyStoreAuth storeAuth, int? productId)
		{

			if (productId == null)
			{
				logger.WarnFormat("ShopifyCommunicator::DeleteProduct(): Argument productId is null.");
				return true;
			}

			XmlDocument errorDocument = ShopifyDelete(protocol+storeAuth.StoreSubDomain+domain+"/admin/products/"+productId.ToString()+".xml",
				HashString(appAuth.Secret + storeAuth.StoreAuthToken));
			if (errorDocument == null) // null == No Errors
			{

				return true; }
			else
			{  
				apiLogger.ErrorFormat("ShopifyCommunicator::DeleteProduct(): Shopify returned errors: {0}", errorDocument.InnerText);
				return false;
			}
		}
	
		public ShopifyResponse<ShopifyOrder> GetOrder(ShopifyStoreAuth storeAuth, int orderId)
		{
			//Ensure the orderID makes sense
			if (orderId <0 )
			{
				logger.Warn("ShopifyCommunicator::GetOrder(): Was passed orderId<0, returning null");
				return null;
			}
			
			//Retrieve the order from shopify
			XmlDocument retrievedOrder = ShopifyGet( (protocol+storeAuth.StoreSubDomain+domain+"/admin/orders/" +orderId.ToString() + ".xml"),
				HashString(appAuth.Secret + storeAuth.StoreAuthToken));
				
		
			//Create e responseObject out of the response. 
			return new ShopifyResponse<ShopifyOrder>(retrievedOrder);
		}

		public List<ShopifyOrder> GetListAllOrders(ShopifyStoreAuth storeAuth)
		{
			List<ShopifyOrder> returnList = null;
			XmlDocument responseDoc =  ShopifyGet( (protocol + storeAuth.StoreSubDomain + domain + "/admin/orders.xml"),
				HashString(appAuth.Secret + storeAuth.StoreAuthToken) );


			if (responseDoc != null)
			{
				XmlNodeList errors = responseDoc.SelectNodes("//errors/error");
				if (errors.Count != 0)
				{

					StringWriter sw = new StringWriter();
					XmlTextWriter xw = new XmlTextWriter(sw);
					responseDoc.WriteTo(xw);


					logger.ErrorFormat("ShopifyCommunicator::GetAllOrders(): Shopify returned errors: {0}", sw.ToString());
					//for now we fall through and take whatever orders we can. 
				}

				XmlNodeList orders = responseDoc.SelectNodes("//orders/order");

				returnList = new List<ShopifyOrder>(orders.Count);
				
				foreach (XmlNode order in orders)
				{
					XmlDocument orderDoc = new XmlDocument();
					orderDoc.AppendChild(orderDoc.ImportNode(order, true));
					ShopifyResponse<ShopifyOrder> serializedResponse = new ShopifyResponse<ShopifyOrder>(orderDoc);

					if (serializedResponse.State == ResponseState.OK)
					{
						returnList.Add(serializedResponse.ResponseObject);
					}
					else
					{
						XmlNodeList orderId = responseDoc.SelectNodes("//order/id");
						string id = orderId.Count > 0 ? orderId[0].InnerText : "unknown";
						logger.ErrorFormat("ShopifyCommunicator()::GetListAllOrders(): Error deserializing order id={0}.",id);
					}

				}


			}
			return returnList;
		}
	
		#endregion

		public ShopifyResponse<ShopifyShop> GetShopInformation(ShopifyStoreAuth storeAuth)
		{
			XmlDocument retrievedShop = ShopifyGet((protocol + storeAuth.StoreSubDomain + domain + "/admin/shop.xml"),
					HashString(appAuth.Secret + storeAuth.StoreAuthToken));

			return new ShopifyResponse<ShopifyShop>(retrievedShop);
		}

		public ShopifyResponse<ShopifyProduct> UpdateProduct(ShopifyStoreAuth storeAuth, ShopifyProduct shopifyProduct)
		{
			if(shopifyProduct ==null || shopifyProduct.Id == null )
			{
				throw new ArgumentNullException ("Cannot update w/ e null shopifyProduct, or with e produc that has e null ID");
			}

			XmlSerializer xS = new XmlSerializer(typeof(ShopifyProduct));
			XmlAttributeOverrides xO = new XmlAttributeOverrides();

			MemoryStream memStream = new MemoryStream();
			XmlDocument xDoc = new XmlDocument();

			xS.Serialize(memStream, shopifyProduct);
			memStream.Seek(0, System.IO.SeekOrigin.Begin);
			xDoc.Load(memStream);
			memStream.Close();

			
			XmlDocument productResponse = ShopifyPutPost((protocol + storeAuth.StoreSubDomain + domain +"/admin/products/"+shopifyProduct.Id+".xml"),
				HashString(appAuth.Secret + storeAuth.StoreAuthToken), xDoc, "PUT");



			return new ShopifyResponse<ShopifyProduct>(productResponse);

		}



		#region utilityFunctions

		/// <summary>
		/// encode a datetime as a string for filtering products, metafields etc. As per shopify's specification.
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public string DateTimeToShopifyString(DateTime time)
		{
			if (time == null)
			{
				throw new ArgumentNullException("time cannot be null");
			}
			
			return String.Format("{0:yyyy-MM-dd HH:mm:ss}", time);
		}



		/// <summary>
		/// PUT/POSTs the given XMLDocument to the path w/ the storePassword. 
		/// </summary>
		/// <param name="storePassword">HashString(shopsterSecret + storeAuthToken)</param>
		/// <param name="path">Full path to post this XMLDocument to.</param>
		/// <param name="xDocPost">XMLDocument to post to the server</param>
		/// <param name="method"> 'PUT' or 'POST' to determine the http method</param>
		/// <returns>Sever response as XMLDocument.</returns>
		private static XmlDocument ShopifyPutPost(string path, string storePassword, XmlDocument xDocPost, string method)
		{
			switch (method)
			{
				case "POST":
				case "PUT":
					break;
				default:
					throw new ArgumentException("method must be 'PUT' or 'POST'");
			}

			
		
			XmlDocument cleanedXDoc = CleanupXmlForShopifySubmission(xDocPost);
			
			HttpWebResponse response = null;
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path);

			
			request.ServicePoint.Expect100Continue = false;
			request.KeepAlive = false;

			//add credentials to the POST
			CredentialCache cCache = new CredentialCache();
			cCache.Add(new Uri(path), "Basic", new NetworkCredential(appAuth.User, storePassword));
			request.Credentials = cCache;
			
			request.Method = method;
			request.ContentType = "application/xml";

			StringWriter sw = new StringWriter();
			XmlTextWriter xw = new XmlTextWriter(sw);
			cleanedXDoc.WriteTo(xw);

			
			//Insert the xmlString into the byte buffer.
			byte[] postBuffer = System.Text.Encoding.UTF8.GetBytes(sw.ToString());
			request.ContentLength = postBuffer.Length;

			//todo learn how to do this compiler directive [if DEBUG]
			string xmlString = sw.ToString();
			apiLogger.DebugFormat("ShopifyPutPost():: HTTP {0} URL={1}", method, path);
			//[endif]
			

			try
			{ //Actually do the POST
				Stream postStream = request.GetRequestStream();
				postStream.Write(postBuffer, 0, postBuffer.Length);
				postStream.Close();

				response = (HttpWebResponse)request.GetResponse();
				
				if (response != null)
				{
					// Open the stream using a StreamReader for easy access.
					StreamReader reader = new StreamReader(response.GetResponseStream());
					XmlDocument xD = new XmlDocument();
					xD.Load(reader);
					
					// Cleanup the streams and the response.
					reader.Close();
					response.Close();
					XmlDocument returnedDoc = CleanupXmlForDeserialization(xD);
					returnedDoc.WriteTo(xw);
					
					//todo learn how to do this compiler directive [if DEBUG]
					xmlString = sw.ToString();
					apiLogger.DebugFormat("ShopifyPutPost():: Shopify's Response is HTTP {0}", response.StatusCode);
					//[endif]
					
					return returnedDoc;
				}
				else
				{
			
					throw new WebException("ShopifyPutPost Expects a Response");
				}


			}
			catch (WebException web)
			{

				//TODO: refactor all this error handling code from Post,Get,Delete into a single method. 
				//Todo: add ShopifyError object so responses can be handled nicely. Especially 422, 503, 402
				//Todo: Consider creating own form of exception. 
				logger.ErrorFormat("ShopifyCommunicator::ShopifyPutPost() web exception: {0} , {1} ", web.Message, web.Response);
				if (web.Response != null)
				{
					if(web.Response.Headers.AllKeys.Contains("Status"))
					{
						
						if (web.Response.Headers["Status"] == "422")
						{
							cleanedXDoc.WriteTo(xw);	
							//Todo: If Debug, next line
							apiLogger.DebugFormat("ShopifyCommunicator::ShopifyPutPost(): Shopify responded with HTTP 422 (Unprocessable Entity), xml was \n {0}", sw.ToString());
							logger.ErrorFormat("ShopifyCommunicator::ShopifyPutPost(): Shopify responded with HTTP 422 (Unprocessable Entity), path = {0}", path);
						}
						else if (web.Response.Headers["Status"] == "402")
						{
							logger.WarnFormat("ShopifyCommunicator::ShopifyPutPost(): Shopify responded with HTTP 402 (Payment Required). User is out of skus or some other limit.");
						}

					}

					var wResponse = web.Response.GetResponseStream();
					StreamReader reader = new StreamReader(wResponse);
					string responseMessage = reader.ReadToEnd();
					reader.Close();
					
					apiLogger.DebugFormat("ShopifyPutPost():: Shopify's Response is HTTP {0}, Content={1}", web.Status, responseMessage);
					return IsValidXML(responseMessage); //returns XmlDocument if valid error, or null. 
				}


			}
			catch (Exception e)
			{

				logger.Error("ShopifyCommunicator:: ShopifyPutPost() caught exception" + e.ToString());
			}
			finally
			{
				if (response != null)
				{
					response.Close();
				}
			}


			logger.Warn("ShopifyCommunicator:: ShopifyPutPost() is returning null XmlDocument");
			return null;			
		}
		
		
		/// <summary>
		/// loads and returns the XML document found at path , using the storePassword.
		/// </summary>
		/// <param name="path">path to the XML Document, must be of form http://someSubDomain.Domain.ext</param>
		/// <param name="storePassword">password for store specified in path. Must be 32 char hexidecimal encoded value</param>
		/// <returns></returns>
		private static XmlDocument ShopifyGet(string path, string storePassword)
		{
			DateTime start = DateTime.Now;

			if (path == null || storePassword == null)
			{
				logger.Warn("ShopifyCommunicator::ShopifyGet(): Was passed a null path or storePassword, failing.");
				return null;
			}

			//create XmlUrlResover so we can use authentication
			XmlUrlResolver urlResolver = new XmlUrlResolver();
			CredentialCache cCache = new CredentialCache();
			//path = path.Replace("http://", "http://" + appAuth.User + ":" + storePassword + "@");
			cCache.Add(new Uri(path), "Basic", new NetworkCredential(appAuth.User, storePassword));
			urlResolver.Credentials = cCache;
								

			//Settings for the XmlReader
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.XmlResolver = urlResolver;
			
			XmlDocument xDoc = new XmlDocument();

			//Try to create the connection, create our Reader
			try
			{
				apiLogger.DebugFormat("ShopifyCommunicator:: ShopifyGet(): Attempting HTTP GET path={0}", path);
				XmlReader xRead = XmlReader.Create(path, settings);
				xDoc.Load(xRead);
				xRead.Close();
				apiLogger.DebugFormat("ShopifyCommunicator:: ShopifyGet(): Success: HTTP GET path={0} .", path);

				return CleanupXmlForDeserialization(xDoc);
			}
			catch (WebException web)
			{
				//Log the exception
				logger.Error("ShopifyCommunicator::ShopifyGet() web exception: " + web.Message + " " + web.Response);
				if (web.Response != null)
				{
					if (web.Response.Headers.AllKeys.Contains("Status"))
					{
						if (web.Response.Headers["Status"] == "401")
						{
							logger.ErrorFormat("ShopifyCommunicator::ShopifyGet(): Invalid authenitcation tokens for path ={0}", path);
						}
						else if (web.Response.Headers["Status"] == "404")
						{
							logger.ErrorFormat("ShopifyCommunicator::ShopifyGet(): Couldnt GET from path ={0}", path);
						}
					}

					var wResponse = web.Response.GetResponseStream();
					StreamReader reader = new StreamReader(wResponse);
					string responseMessage = reader.ReadToEnd();
					reader.Close();

				logger.ErrorFormat("ShopifyCommunicator::ShopifyGet() web exception, Shopify's Response is: {0} ", responseMessage);
				return IsValidXML(responseMessage); //returns XmlDocument if valid error, or null. 
				}
			}

			logger.Warn("ShopifyCommunicator:: ShopifyGet() is returning null XmlDocument");
			return null; 
		}

		private static XmlDocument ShopifyDelete(string path, string storePassword)
		{
			HttpWebRequest deleteRequest = (HttpWebRequest) HttpWebRequest.Create(path);
			deleteRequest.Method = "DELETE";
			
			CredentialCache cCache = new CredentialCache();
			cCache.Add(new Uri(path), "Basic", new NetworkCredential(appAuth.User, storePassword));
			deleteRequest.Credentials = cCache;
			deleteRequest.KeepAlive = false;

			try
			{

				apiLogger.DebugFormat("ShopifyCommunicator::ShopifyDelete(): attempting DELETE request to ({0})", path);
				HttpWebResponse deleteResponse = (HttpWebResponse)deleteRequest.GetResponse();
				apiLogger.DebugFormat("ShopifyCommunicator::ShopifyDelete(): Received HttpStatusCode ({0})", deleteResponse.StatusCode);
		
				
				if (deleteResponse != null)
				{
					if (deleteResponse.StatusCode == HttpStatusCode.OK)
					{
						// return null indicates success
						deleteResponse.Close();
						return null;
					}
				}
			}
			catch ( WebException we )
			{
							
	

				HttpWebResponse deleteResponse = (HttpWebResponse) we.Response;
				if (deleteResponse != null)
				{
					if(deleteResponse.Headers.AllKeys.Contains("Status"))
					{
						if ( deleteResponse.Headers["Status"] == "404")
						{
						apiLogger.WarnFormat("ShopifyCommunicator::ShopifyDelete():: WebException Thrown: Http 404. Path is ({0})", path);
						return null; //If its not there, good!
						}
						else if (deleteResponse.Headers["Status"] == "401")
						{
							apiLogger.WarnFormat("ShopifyCommunicator::ShopifyDelete():: WebException Thrown: Http 401. Invalid Authentication. Path is ({0})", path);
						}
				
					}

					apiLogger.ErrorFormat("ShopifyCommunicator::ShopifyDelete(): Error HTTP DELETE url={0} . Server Response={1}", path, deleteResponse.StatusCode.ToString());

					StreamReader reader = new StreamReader(deleteResponse.GetResponseStream());
					string responseMessage = reader.ReadToEnd();
					// Cleanup the streams and the response.
					reader.Close();
					we.Response.Close();

					XmlDocument errorDoc = IsValidXML(responseMessage); //returns XmlDocument if valid error, or null. 
					if (errorDoc == null)
					{
						errorDoc = IsValidXML("<error>" + responseMessage+ "\n</error>");
					}
					return errorDoc;
				}


			}

			logger.WarnFormat("ShopifyCommunicator::ShopifyDelete(): returning null XmlDocument.");
			return null;

		}



		/// <summary>
		/// Hashes(MD5) the given value. Typically used as "HashString(shopsterSecret + storeAuthToken)"
		/// </summary>
		/// <param name="Value">String to be hashed</param>
		/// <returns>Returns hex encoded password</returns>
		private static string HashString(string Value)
		{
			System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
			byte[] data = System.Text.Encoding.ASCII.GetBytes(Value);
			data = x.ComputeHash(data);
			string ret = "";
			for (int i = 0; i < data.Length; i++)
				ret += data[i].ToString("x2").ToLower();
			return ret;
		}

		private static XmlDocument IsValidXML(string input)
		{
			try
			{
				XmlDocument xD = new XmlDocument();
				xD.LoadXml(input);
				return xD;
			}
			catch (XmlException e)
			{
				logger.WarnFormat("ShopifyCommunicator::IsValidXML(): XmlException while trying to load xml. Message is {0}", e.Message);
				return null;
			}
			
		}
		
		/// <summary>
		/// Remove tags w/ "nil=true" for "type=decimal" or "type=datetime". Do this because XMLSerializer crashes on these cases.
		/// </summary>
		/// <param name="inDoc">The XmlDocument you want cleaned up for deserialization.</param>
		/// <returns>A deserializable XmlDocument</returns>
		public static XmlDocument CleanupXmlForDeserialization(XmlDocument inDoc)
		{
			if (inDoc == null)
			{
				return inDoc;
			}
				
				//<compare-at-price type="decimal" nil="true">     </compare-at-price>
			const string nilDecimal = @"\<[^\>]+ type\=\""(decimal|datetime|integer){1}\"" nil\=\""true\""[^\>]*\>[^\<\>]*\<\/[^\>]+\>";

			StringWriter sw = new StringWriter();
			XmlTextWriter xw = new XmlTextWriter(sw);
			inDoc.WriteTo(xw);
			string xmlString = sw.ToString();
			
			xmlString = Regex.Replace(xmlString, nilDecimal, string.Empty);
			
			XmlDocument returnDoc = new XmlDocument();

			try
			{
				if (xmlString != string.Empty)
				{
					returnDoc.LoadXml(xmlString);
				}
				return returnDoc;
			}
			catch (XmlException e)
			{
				logger.ErrorFormat("ShopifyCommunicator()::CleanupXmlForDeserialization(): Cleanup invalidated XML. Final string was {0} . Exception is ({1}) ", xmlString, e.Message);
				return null;
			}
		}

		/// <summary>
		/// Add " type="array" " to elements that require it. Remove xsi:nil="true" elements.
		/// </summary>
		/// <param name="inDoc">XmlDocument to be cleaned before submission to Shopify.</param>
		/// <returns>Cleaned XmlDocument ready for POST/PUT to Shopify.</returns>
		public static XmlDocument CleanupXmlForShopifySubmission(XmlDocument inDoc)
		{
			
			const string withoutArray = @"\<[^/\>]*(options|images|variants){1}[^\>]*\>";
			const string addArray = @"<$1 type=""array"">";
			const string removeNil = @"\<[^\>]* xsi:nil=""(true|false){1}"" [^\<\>]*\>[\s]*";
			const string findDecimalTags = @"<(price){1}[^\>]*>";
			const string addTypeDecimal = @"<$1 type=""decimal"">";

			const string findIntegerTypeTags = @"<(grams|id|inventory-quantity|position|product-id){1}>";
			const string addTypeInteger = @"<$1 type=""integer"">";

			const string findBooleanTypeTags = @"<(taxable|requires-shipping){1}>";
			const string addTypeBoolean = @"<$1 type=""boolean"">";

			const string findDateTimeTypeTags = @"<(updated-at|created-at|published-at){1}>";
			const string addTypeDateTime = @"<$1 type=""datetime"">";

			const string removeXsi = @"xmlns:(xsd|xsi){1}=""[^""]*""";
			
			//Get the xmldocument as e string
			StringWriter sw = new StringWriter();
			XmlTextWriter xw = new XmlTextWriter(sw);
			inDoc.WriteTo(xw);
			
			string xmlString = sw.ToString();
			xmlString = Regex.Replace(xmlString, removeXsi, String.Empty);
			xmlString = Regex.Replace(xmlString, removeNil, string.Empty);

			xmlString = Regex.Replace(xmlString, withoutArray, addArray);
			xmlString = Regex.Replace(xmlString, findDecimalTags, addTypeDecimal);
			xmlString = Regex.Replace(xmlString, findIntegerTypeTags, addTypeInteger);
			xmlString = Regex.Replace(xmlString, findBooleanTypeTags, addTypeBoolean);
			xmlString = Regex.Replace(xmlString, findDateTimeTypeTags, addTypeDateTime);
						
			XmlDocument returnDoc = new XmlDocument();
			try
			{
				returnDoc.LoadXml(xmlString);
				return returnDoc;
			}
			catch
			{
				logger.ErrorFormat("ShopifyCommunicator()::CleanupXmlForShopifySubmission(): Cleanup invalidated XML. Final string was {0}", xmlString);
				return null;
			}

			
		}

		#endregion


	}
}

