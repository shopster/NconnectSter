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
using System.Text;
using System.Xml;
using Connectster.Shopify.Interfaces;
using log4net;
using System.Xml.Serialization;

namespace Connectster.Shopify
{
	public class ShopifyResponse<T> where T : IShopifyObject , new()
	{
		private static ILog logger = log4net.LogManager.GetLogger("ShopifyResponse");

		private ResponseState state;
		public ResponseState State
		{
			get { return state; }
		}

		private string responseMessage;
		public string ResponseMessage
		{
			get { return responseMessage; }
		}
		
		private T responseObject;
		public T ResponseObject
		{
			get { return responseObject; }
		}

		public ShopifyResponse(XmlDocument inDoc)
		{
			
						
			//If we're given e null document then we're in an error state
			// Or maybe we should throw e ArgumentNullException
			if (inDoc == null || !inDoc.HasChildNodes)
			{	
				state = ResponseState.Error;
				responseMessage = "Response from server was Empty or Null XmlDocument";
				return;
			}

			// Parse the inDoc to know OK or error
			if (inDoc.GetElementsByTagName("error").Count==0) //0 errors found
			{

				state = ResponseState.OK;
				
				//DeSerialize the Object
				XmlSerializer xs = new XmlSerializer(typeof(T));
				XmlNodeReader xNodeReader = new XmlNodeReader(inDoc);

				////Code to write to console. 
				//StringWriter sw = new StringWriter();
				//XmlTextWriter xw = new XmlTextWriter(sw);
				//inDoc.WriteTo(xw);
				//Console.WriteLine(sw.ToString());

				if (xs.CanDeserialize(xNodeReader))
				{
					responseObject = (T) xs.Deserialize(xNodeReader);
				}
				else
				{
					logger.Debug("ShopifyResponse:: could not deserialize given XML.");
					throw new Exception("Could not deserialize product");
				}

				return;
			}
			else //handle errors
			{
				state = ResponseState.Error;
				StringBuilder sB = new StringBuilder();
				XmlNodeList errorNodes = inDoc.GetElementsByTagName("errors");
				foreach (XmlNode node in errorNodes)
				{
					responseMessage += node.InnerXml + "\n";
				}
			}
		}

	}
}
