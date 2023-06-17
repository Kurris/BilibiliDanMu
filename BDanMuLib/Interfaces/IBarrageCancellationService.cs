using System.Threading;

namespace LiveCore.Interfaces
{
    public interface IBarrageCancellationService
    {
        bool Cancel(string connectionId);

        void SetConnectionIdWithCancellationToken(string connectionId);

        CancellationTokenSource Get(string connectionId);

        bool ExistsCancelToken(string connectionId);

        int ConnectionCount();
    }
}
