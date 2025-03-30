namespace HCoroutines;

/// <summary>
/// The RunMode determines when the coroutine is run and whether it can be paused.
/// </summary>
public enum CoRunMode {
    /// <summary>
    /// The RunMode is inherited from the parent coroutine. If there is no parent, Pausable is used.
    /// </summary>
    Inherit,

    /// <summary>
    /// The coroutine is paused when the game is paused.
    /// </summary>
    Pausable,

    /// <summary>
    /// The coroutine only runs when the game is paused.
    /// </summary>
    WhenPaused,

    /// <summary>
    /// The coroutine always runs, regardless of whether the game is paused is or not.
    /// </summary>
    Always
}