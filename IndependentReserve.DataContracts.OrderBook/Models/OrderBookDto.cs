using System;
using System.Collections.Generic;
using IndependentReserve.DataContracts.OrderBook.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IndependentReserve.DataContracts.OrderBook.Models
{
    public class OrderBookDto
    {
        public List<OrderDto> BuyOrders { get; set; }
        public DateTime CreatedTimestampUtc { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public PrimaryCurrencyCode PrimaryCurrencyCode { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public SecondaryCurrencyCode SecondaryCurrencyCode { get; set; }
    }
}