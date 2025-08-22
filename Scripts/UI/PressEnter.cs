using Godot;
using System;

public partial class PressEnter : CanvasLayer
{
    PackedScene titleScreenMenu = (PackedScene)GD.Load("res://Objects/UI_Components/title_screen_menu.tscn");

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("ui_accept"))
        {
            Node instance = titleScreenMenu.Instantiate();
            GetParent().AddChild(instance);
            QueueFree();
        }
    }
}
