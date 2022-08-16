
namespace HCoroutines {
    public interface ICoroutineStopListener {
        void OnChildStop(CoroutineBase coroutine);
    }
}
