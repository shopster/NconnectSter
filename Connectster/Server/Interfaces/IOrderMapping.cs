namespace Connectster.Server.Interfaces
{
	interface IOrderMapping
	{
		IOrder SourceOrder { get; set; }
		IOrder DestinationOrder { get; set; }

	}
}
