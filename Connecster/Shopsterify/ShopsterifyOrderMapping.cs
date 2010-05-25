using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shopsterify.Shopsterify.Interfaces;

namespace Shopsterify.Shopsterify
{
	public class ShopsterifyOrderMapping: IOrderMapping
	{
		public ShopsterifyOrderMapping(IOrder source, IOrder dest)
		{

			SourceOrder = source;
			DestinationOrder = dest;
		}

		#region IOrderMapping Members
		public IOrder SourceOrder {	get; set; }
		public IOrder DestinationOrder { get; set; }
		#endregion
	}
}
