using IndependentReserve.DataContracts.OrderBook.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IndependentReserve.DataContracts.OrderBook.Models
{
    public class OrderChangeDto
    {
        public string Channel { get; set; }
        public int Nonce { get; set; }
        public OrderChangeDataDto Data { set; get; }

        [JsonConverter(typeof(StringEnumConverter))]
        public OrderChangeEvent Event { get; set; }
    }
}