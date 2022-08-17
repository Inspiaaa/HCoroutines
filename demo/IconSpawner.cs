using Godot;
using System;

public class IconSpawner : Node2D
{
    [Export] private PackedScene icon;

    public override void _Input(InputEvent e)
    {
        if (e is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.ButtonIndex == (int)ButtonList.Left && mouseEvent.Pressed)
            {
                Node2D node = icon.Instance<Node2D>();
                node.Position = GetLocalMousePosition();
                AddChild(node);
            }
        }
    }
}
