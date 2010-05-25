using Connectster.Server.Interfaces;

namespace Connectster.Server
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
