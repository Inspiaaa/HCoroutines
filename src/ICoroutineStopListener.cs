namespace HCoroutines;

public interface ICoroutineStopListener
{
    void OnChildStopped(CoroutineBase coroutine);
}