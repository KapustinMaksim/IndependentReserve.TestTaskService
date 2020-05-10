using System;
using System.Collections.Generic;
using System.Linq;
using IndependentReserve.DataContracts.OrderBook.Enums;
using IndependentReserve.Worker.Worker.ConfigurationsModels;
using IndependentReserve.Worker.Worker.Containers;
using IndependentReserve.Worker.Worker.WsChannel;

namespace IndependentReserve.Worker.Worker
{
    public class OrderBookWorker
    {

        private readonly Lazy<List<IFetchService>> _orderBookContainers;
        private readonly Lazy<WsChannelMonitor> _wsChannelMonitor;

        public OrderBookWorker(ProviderConfigModel configurations)
        {
            _orderBookContainers = new Lazy<List<IFetchService>>(() =>
                (from primaryCurrencyCode in Enum.GetValues(typeof(PrimaryCurrencyCode)).Cast<PrimaryCurrencyCode>()
                 from secondaryCurrencyCode in Enum.GetValues(typeof(SecondaryCurrencyCode)).Cast<SecondaryCurrencyCode>()
                 where primaryCurrencyCode != PrimaryCurrencyCode.undefined && secondaryCurrencyCode != SecondaryCurrencyCode.undefined
                 select (IFetchService)new OrderBookDataContainer(configurations, primaryCurrencyCode, secondaryCurrencyCode)).ToList());

            _wsChannelMonitor = new Lazy<WsChannelMonitor>(() => new WsChannelMonitor(configurations, _orderBookContainers.Value));
        }

        public void Start()
        {
            _wsChannelMonitor.Value.InstallBackgroundMonitor();
        }

        public void ShutDown()
        {
            _wsChannelMonitor.Value.ShutDown();
        }

        public List<IContainerDataProvider> GetContainers() => _orderBookContainers.Value.Cast<IContainerDataProvider>().ToList();

    }
}
