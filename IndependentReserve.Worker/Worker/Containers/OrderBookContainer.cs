using System;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IndependentReserve.Common.Tools;
using IndependentReserve.DataContracts.OrderBook.Enums;
using IndependentReserve.DataContracts.OrderBook.Models;
using IndependentReserve.DataContracts.OrderBook.Models.ModelExtensions;
using IndependentReserve.Worker.Exceptions;
using IndependentReserve.Worker.Worker.ConfigurationsModels;
using Newtonsoft.Json;

namespace IndependentReserve.Worker.Worker.Containers
{

    internal class OrderBookDataContainer : IContainerDataProvider, IFetchService
    {

        private readonly ProviderConfigModel _configurations;
        private readonly RefreshBufferFactory _refreshBufferFactory;

        private readonly ManualResetEvent _signalEvent;
        private readonly ConcurrentDictionary<Guid, ExtraOrderDto> _orderBookBuffer;
        private readonly ConcurrentQueue<OrderChangeDto> _refreshBufferQueue;
        private volatile bool _listenBufferQueue;
        private volatile bool _apiIsAlive = true;

        public OrderBookDataContainer(
            ProviderConfigModel configurations, 
            PrimaryCurrencyCode primaryCurrencyCode,
            SecondaryCurrencyCode secondaryCurrencyCode)
        {
            _configurations = configurations;
            PrimaryCurrencyCode = primaryCurrencyCode;
            SecondaryCurrencyCode = secondaryCurrencyCode;
            _signalEvent = new ManualResetEvent(false);
            _orderBookBuffer = new ConcurrentDictionary<Guid, ExtraOrderDto>();
            _refreshBufferQueue = new ConcurrentQueue<OrderChangeDto>();
            _refreshBufferFactory = new RefreshBufferFactory(_orderBookBuffer);
        }

        public PrimaryCurrencyCode PrimaryCurrencyCode { get; }
        public SecondaryCurrencyCode SecondaryCurrencyCode { get; }

        public void PushToQueue(OrderChangeDto orderChange)
        {
            _refreshBufferQueue.Enqueue(orderChange);
        }

        public ExtraOrderBookDto GetAllOrders()
        {
            _signalEvent.WaitOne();
            var result = _orderBookBuffer.Select(o => o.Value).ToList();
            var lastOrder = result.OrderByDescending(o => o.UpdateDateTime).FirstOrDefault(o=>o.Nonce.HasValue);

            if (lastOrder?.Nonce == null)
                throw new OrderBookContainerSynchronizationException("The last order does not have a Nonce");

            return new ExtraOrderBookDto
            {
                LastNonce = lastOrder.Nonce.Value,
                BuyOrders = result.Cast<OrderDto>().ToList(),
                CreatedTimestampUtc = DateTime.Now,
                PrimaryCurrencyCode = PrimaryCurrencyCode,
                SecondaryCurrencyCode = SecondaryCurrencyCode,
            };
        }

        public void ShutDown()
        {
            _apiIsAlive = false;
        }

        public void ReloadOrderBook()
        {
            _listenBufferQueue = false;
        }

        public async Task InstallContainer()
        {
            while (_apiIsAlive)
            {
                if (_listenBufferQueue)
                {
                    ProcessOrderBookBuffer();
                }
                else
                {
                    _signalEvent.Reset();
                    await FetchOrderBook();
                    _listenBufferQueue = true;
                }
                await Task.Delay(new TimeSpan(100));
            }
        }

        private void ProcessOrderBookBuffer()
        {
            if (!_refreshBufferQueue.TryDequeue(out var orderChangeDto)) 
                return;

            _refreshBufferFactory.ProcessChange(orderChangeDto);
            
            if (orderChangeDto.Event == OrderChangeEvent.NewOrder ||
                orderChangeDto.Event == OrderChangeEvent.OrderChanged)
                _signalEvent.Set();
        }

        private async Task FetchOrderBook()
        {
            var orderBook = JsonConvert.DeserializeObject<OrderBookDto>(await new HttpClient().GetAsync(new Uri(_configurations.IndependentReserveApiUri, "/Public/GetAllOrders"), new NameValueCollection
            {
                ["primaryCurrencyCode"] = PrimaryCurrencyCode.ToString(),
                ["secondaryCurrencyCode"] = SecondaryCurrencyCode.ToString()
            }));

            if (orderBook == null)
                return;

            _orderBookBuffer.Clear();

            foreach (var order in orderBook.BuyOrders)
            {
                _orderBookBuffer.TryAdd(order.Guid, new ExtraOrderDto
                {
                    Guid = order.Guid,
                    Price = order.Price,
                    Volume = order.Volume,
                    UpdateDateTime = DateTime.Now
                });
            }

        }

    }
}