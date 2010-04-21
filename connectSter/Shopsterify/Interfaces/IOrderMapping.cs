using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shopsterify.Shopsterify.Interfaces
{
	interface IOrderMapping
	{
		IOrder SourceOrder { get; set; }
		IOrder DestinationOrder { get; set; }

	}
}
