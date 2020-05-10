using System;

namespace IndependentReserve.Worker.Worker.ConfigurationsModels
{
    public class ProviderConfigModel
    {
        public Uri IndependentReserveApiUri { get; set; }
        public Uri WebSocketChannelUri { get; set; }
    }
}