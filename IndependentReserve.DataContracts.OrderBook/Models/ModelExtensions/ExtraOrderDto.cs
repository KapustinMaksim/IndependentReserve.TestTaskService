using System;

namespace IndependentReserve.DataContracts.OrderBook.Models.ModelExtensions
{
    public class ExtraOrderDto : OrderDto
    {
        public int? Nonce { get; set; }
        public DateTime UpdateDateTime { get; set; }
    }
}