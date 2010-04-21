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
using System.Xml.Serialization;
using Shopsterify.Shopify;
using System.Xml;

namespace Shopsterify
{
	[Serializable]
	[XmlRootAttribute(ElementName = "image", IsNullable = true)]
	public class ShopifyImage : IShopifyObject
	{
		private DateTime? createdAt;
		private int? id;
		private int? position;
		private int? productId;
		private DateTime? updatedAt;
		private string source; //URL of image

		public ShopifyImage()
		{

		}

		[XmlElement(ElementName = "created-at", IsNullable = true)]
		public DateTime? CreatedAt
		{
			get { return createdAt; }
			set { createdAt = value; }
		}

		[XmlElement(ElementName = "id", IsNullable = true)]
		public int? Id
		{
			get { return id; }
			set { id = value; }
		}

		[XmlElement(ElementName = "position", IsNullable = true)]
		public int? Position
		{
			get { return position; }
			set { position = value; }
		}

		[XmlElement(ElementName = "product-id", IsNullable = true)]
		public int? ProductId
		{
			get { return productId; }
			set { productId = value; }
		}

		[XmlElement(ElementName = "updated-at", IsNullable = true)]
		public DateTime? UpdatedAt
		{
			get { return updatedAt; }
			set { updatedAt = value; }
		}

		[XmlElement(ElementName = "src", IsNullable = false)]
		public string Source
		{
			get { return source; }
			set { source = value; }
		}

		public IShopifyObject createObject(XmlDocument inDoc)
		{
			return null;
		}

	}
}
