using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IndependentReserve.DataContracts.OrderBook.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SecondaryCurrencyCode
    {
        undefined,
        aud,
        usd
    }
}