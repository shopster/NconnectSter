using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Shopsterify.Shopsterify.Interfaces;

namespace Shopsterify.Shopify.Interfaces
{
	interface IShopifyOrder
	{
		bool BuyerAcceptsMarketing { get; set; } //If the user wants to receive emails

		DateTime? ClosedAt { get; set; } //

		DateTime? CreatedAt { get; set; }

		CurrencyCode Currency { get; set; } //USD or CAD 

		string Email { get; set; }

		FinancialState FinancialStatus { get; set; }

		string FulfillmentStatus { get; set; }

		string Gateway { get; set; }

		int Id { get; set; }

		string LandingSite { get; set; }

		string Name { get; set; }

		String Note { get; set; }

		int Number { get; set; }

		string ReferringSite { get; set; }

		decimal SubtotalPrice { get; set; }

		bool TaxesIncluded { get; set; }

		string Token { get; set; }

		decimal TotalDiscounts { get; set; }

		decimal TotalLineItemsPrice { get; set; }

		decimal TotalTax { get; set; }

		int TotalWeight { get; set; } //In grams

		DateTime UpdatedAt { get; set; }

		string BrowserIp { get; set; }

		string LandingSiteRef { get; set; }

		int OrderNumber { get; set; }

		ShopifyAddress BillingAddress { get; set; }

		ShopifyAddress ShippingAddress { get; set; }

		ShopifyLineItem[] LineItems { get; set; }

		ShopifyShippingLine[] ShippingLines { get; set; }

		ShopifyTaxLine[] TaxLines { get; set; }

	}
}
