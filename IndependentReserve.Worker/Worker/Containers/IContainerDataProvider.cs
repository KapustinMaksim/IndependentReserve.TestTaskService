using IndependentReserve.DataContracts.OrderBook.Models.ModelExtensions;

namespace IndependentReserve.Worker.Worker.Containers
{
    public interface IContainerDataProvider : IOrderBook
    {
        ExtraOrderBookDto GetAllOrders();
    }
}