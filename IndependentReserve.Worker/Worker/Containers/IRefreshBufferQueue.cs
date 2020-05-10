using IndependentReserve.DataContracts.OrderBook.Models;

namespace IndependentReserve.Worker.Worker.Containers
{
    public interface IRefreshBufferQueue : IOrderBook
    {
        void PushToQueue(OrderChangeDto orderChange);
        void ReloadOrderBook();
    }
}