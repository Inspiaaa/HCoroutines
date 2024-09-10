using Godot;

namespace HCoroutines.Demo;

public partial class PauseController : Node
{
    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("ui_select"))
        {
            GetTree().Paused = !GetTree().Paused;
            GD.Print("Paused = ", GetTree().Paused);
        }
    }
}