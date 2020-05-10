using IndependentReserve.DataContracts.OrderBook.Enums;

namespace IndependentReserve.Worker.Worker.Containers
{
    public interface IOrderBook
    {
        PrimaryCurrencyCode PrimaryCurrencyCode { get; }
        SecondaryCurrencyCode SecondaryCurrencyCode { get; }
    }
}