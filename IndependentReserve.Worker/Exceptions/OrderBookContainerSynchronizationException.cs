using System;

namespace IndependentReserve.Worker.Exceptions
{
    public class OrderBookContainerSynchronizationException :InvalidOperationException
    {
        public OrderBookContainerSynchronizationException(string synchronizationErrorMsg): base(synchronizationErrorMsg) { }
    }
}