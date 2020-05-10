using System;
using System.Collections.Generic;
using IndependentReserve.DataContracts.OrderBook.Enums;
using IndependentReserve.DataContracts.OrderBook.Models;
using IndependentReserve.Worker.Worker.Containers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IndependentReserve.Worker.Worker.WsChannel
{
    public class WsMessageProcessor
    {
        private readonly List<IRefreshBufferQueue> _containers;
        private readonly Dictionary<OrderChangeEvent, Action<string>> _processors;

        public WsMessageProcessor(List<IRefreshBufferQueue> containers)
        {
            _containers = containers;
            _processors = new Dictionary<OrderChangeEvent, Action<string>>
            {
                {OrderChangeEvent.NewOrder,  ProcessOrderChangeEvent},
                {OrderChangeEvent.OrderCanceled,  ProcessOrderChangeEvent},
                {OrderChangeEvent.OrderChanged,  ProcessOrderChangeEvent},
                {OrderChangeEvent.Subscriptions,  (message)=> {ApplicationLogger.Logger.Trace(message);}}
            };
        }

        public void ProcessMessage(string message)
        {
            var messageNode = JObject.Parse(message)["Event"]?.Value<string>();

            if (Enum.TryParse(messageNode, out OrderChangeEvent messageEvent) && _processors.ContainsKey(messageEvent))
            {
                _processors[messageEvent](message);
                return;
            }

            throw new InvalidOperationException($"Not supported message '{message}'");
        }

        private void ProcessOrderChangeEvent(string message)
        {

            var orderChange = JsonConvert.DeserializeObject<OrderChangeDto>(message, new Newtonsoft.Json.Converters.StringEnumConverter());

            foreach (var container in _containers)
            {
                if (orderChange.Data == null)
                    return;

                var (primaryCurrencyCode, secondaryCurrencyCode) = orderChange.Data.ParsePairValues();

                if (container.PrimaryCurrencyCode == primaryCurrencyCode &&
                    container.SecondaryCurrencyCode == secondaryCurrencyCode)
                {
                    container.PushToQueue(orderChange);
                    return;
                }
            }
        }

    }
}