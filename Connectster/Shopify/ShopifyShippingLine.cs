using System;
using System.Xml.Serialization;

namespace Connectster.Shopify
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
