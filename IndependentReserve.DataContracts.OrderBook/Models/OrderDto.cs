using System;

namespace IndependentReserve.DataContracts.OrderBook.Models
{
    public class OrderDto
    {
        public Guid Guid { get; set; }
        public decimal Price { get; set; }
        public decimal Volume { get; set; }

    }
}