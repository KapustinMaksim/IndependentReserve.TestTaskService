using System.Threading.Tasks;

namespace IndependentReserve.Worker.Worker.Containers
{
    public interface IFetchService : IRefreshBufferQueue
    {
        Task InstallContainer();
        void ShutDown();
    }
}