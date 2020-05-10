using System.Linq;
using IndependentReserve.DataContracts.OrderBook.Enums;
using IndependentReserve.DataContracts.OrderBook.Models.ModelExtensions;
using IndependentReserve.Worker.Worker;

namespace IndependentReserve.DataProviders
{
    public class OrderBookProvider
    {
        private readonly OrderBookWorker _orderBookWorker;

        public OrderBookProvider(OrderBookWorker orderBookWorker)
        {
            _orderBookWorker = orderBookWorker;
        }

        public (ExtraOrderBookDto, string) GetAllOrders(PrimaryCurrencyCode primaryCurrencyCode, SecondaryCurrencyCode secondaryCurrencyCode)
        {

            if (primaryCurrencyCode == PrimaryCurrencyCode.undefined)
                return (null, "Invalid Primary Currency Code");
            
            if (secondaryCurrencyCode == SecondaryCurrencyCode.undefined)
                return (null, "Invalid Secondary Currency Code");

            var container = _orderBookWorker.GetContainers().FirstOrDefault(c=>c.PrimaryCurrencyCode == primaryCurrencyCode && c.SecondaryCurrencyCode == secondaryCurrencyCode);

            if (container == null)
                return (null, $"Invalid pair {primaryCurrencyCode}-{secondaryCurrencyCode}");

            return (container.GetAllOrders(), null);
        }

    }
}