using System;
using System.Xml.Serialization;

namespace Connectster.Shopify
{
	[Serializable]
	public class ShopifyTaxLine
	{
		[XmlElement(ElementName = "price", IsNullable = false)]
		public decimal Price { get; set; }

		[XmlElement(ElementName = "rate", IsNullable = false)]
		public float Rate { get; set; }

		[XmlElement(ElementName = "title", IsNullable = false)]
		public string Title { get; set; }
	}
}
