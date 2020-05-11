using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IndependentReserve.Worker.Worker.ConfigurationsModels;
using IndependentReserve.Worker.Worker.Containers;

namespace IndependentReserve.Worker.Worker.WsChannel
{

    internal class WsChannelMonitor
    {
        private readonly WsMessageProcessor _wsMessageProcessor;
        private readonly ProviderConfigModel _configurations;
        private readonly List<IFetchService> _containers;

        private readonly ManualResetEvent _monitorLaunchEvent;
        private volatile bool _apiIsAlive = true;

        public WsChannelMonitor(ProviderConfigModel configurations, List<IFetchService> containers)
        {
            _configurations = configurations;
            _containers = containers;
            _wsMessageProcessor = new WsMessageProcessor(containers.Cast<IRefreshBufferQueue>().ToList());
            _monitorLaunchEvent = new ManualResetEvent(false);
        }

        delegate void OnRejectEventHandler();
        event OnRejectEventHandler OnReject;

        delegate void OnShutDownEventHandler();
        event OnShutDownEventHandler OnOnShutDown;

        public void ShutDown()
        {
            _apiIsAlive = false;
            OnOnShutDown?.Invoke();
            _monitorLaunchEvent.Dispose();
        }

        public void InstallBackgroundMonitor()
        {
            Task.Factory.StartNew(async () =>
            {
                await InstallWsConnection();
            });

            _monitorLaunchEvent.WaitOne();
            InstallContainers();
        }

        private void InstallContainers()
        {
            foreach (var container in _containers)
            {
                OnReject += container.ReloadOrderBook;
                OnOnShutDown += container.ShutDown;
                Task.Factory.StartNew(async ()=> await container.InstallContainer());
            }
        }

        private async Task InstallWsConnection()
        {
            while (_apiIsAlive)
            {
                try
                {
                    using var ws = new ClientWebSocket();
                    await ws.ConnectAsync(_configurations.WebSocketChannelUri, CancellationToken.None);

                    _monitorLaunchEvent.Set();
                        
                    while (ws.State == WebSocketState.Open)
                    {
                        var receivedMessage = await Receive(ws);
                        _wsMessageProcessor.ProcessMessage(receivedMessage);
                    }
                }
                catch (Exception ex)
                {
                    ApplicationLogger.Logger.Error(ex);
                    OnReject?.Invoke();
                }
                finally
                {
                    await Task.Delay(new TimeSpan(0, 0, 1));
                }
            }

        }

        private async Task<string> Receive(ClientWebSocket socket)
        {
            var buffer = new ArraySegment<byte>(new byte[2048]);
            WebSocketReceiveResult result;
            using var ms = new MemoryStream();
            do
            {
                result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                ms.Write(buffer.Array ?? throw new InvalidOperationException("Received buffer is empty."), buffer.Offset, result.Count);
            } while (!result.EndOfMessage);

            ms.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(ms, Encoding.UTF8);
            return await reader.ReadToEndAsync();
        }

    }
}