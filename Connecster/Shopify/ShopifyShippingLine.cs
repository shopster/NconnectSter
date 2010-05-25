using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;


namespace Shopsterify.Shopify
{
	[Serializable]
	public class ShopifyShippingLine
	{
		[XmlElement(ElementName = "code", IsNullable = false)]
		public string Code { get; set; }

		[XmlElement(ElementName = "price", IsNullable = false)]
		public decimal Price { get; set; }

		[XmlElement(ElementName = "title", IsNullable = false)]
		public string Title { get; set; }

	}
}
