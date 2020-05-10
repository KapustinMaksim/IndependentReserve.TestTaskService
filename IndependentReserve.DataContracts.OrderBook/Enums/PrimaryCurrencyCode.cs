using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IndependentReserve.DataContracts.OrderBook.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PrimaryCurrencyCode
    {
        undefined,
        eth,
        xbt
    }
}