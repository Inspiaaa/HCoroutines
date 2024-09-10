using Godot;

namespace  HCoroutines.Demo;

public partial class IconSpawner : Node2D
{
    [Export] private PackedScene icon;

    public override void _Input(InputEvent e)
    {
        if (e is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
            {
                Node2D node = icon.Instantiate<Node2D>();
                node.Position = GetLocalMousePosition();
                AddChild(node);
            }
        }
    }

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
    }
}
