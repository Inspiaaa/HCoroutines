namespace HCoroutines;

/// <summary>
/// The ProcessMode determines when the Update() method of a coroutine is called.
/// </summary>
public enum CoProcessMode {
    /// <summary>
    /// The ProcessMode is inherited from the parent coroutine. If there is no parent, then the Normal mode is used.
    /// </summary>
    Inherit,

    /// <summary>
    /// The Update() method is called during each process frame (like the _Process() method).
    /// </summary>
    Normal,

    /// <summary>
    /// The Update() method is called during each physics frame (like the _PhysicsProcess() method).
    /// </summary>
    Physics
}