namespace IndependentReserve.DataContracts.OrderBook.Models.ModelExtensions
{
    public class ExtraOrderBookDto : OrderBookDto
    {
        public int LastNonce { get; set; }
    }
}