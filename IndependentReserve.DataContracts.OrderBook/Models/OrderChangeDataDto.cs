using System;
using System.IO;
using IndependentReserve.DataContracts.OrderBook.Enums;

namespace IndependentReserve.DataContracts.OrderBook.Models
{
    public class OrderChangeDataDto
    {
        public Guid OrderGuid { get; set; }
        public string OrderType { get; set; }
        public string Pair { get; set; }
        public decimal Price { get; set; }
        public decimal Volume { get; set; }

        public (PrimaryCurrencyCode, SecondaryCurrencyCode) ParsePairValues()
        {
            if (string.IsNullOrEmpty(Pair))
                throw new InvalidDataException($"The field '{nameof(OrderChangeDataDto)}.{nameof(Pair)}' is empty.");

            var currencies = Pair.Split('-');

            if (currencies.Length != 2)
                throw new InvalidDataException($"The field '{nameof(OrderChangeDataDto)}.{nameof(Pair)}' has incorrect format '{Pair}'.");

            return ((PrimaryCurrencyCode)Enum.Parse(typeof(PrimaryCurrencyCode), currencies[0]), (SecondaryCurrencyCode)Enum.Parse(typeof(SecondaryCurrencyCode),currencies[1]));
        }
    }
}