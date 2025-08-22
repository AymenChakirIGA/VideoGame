using Godot;
using System;

public partial class Collectable : Area2D
{
    [Signal]
    public delegate void CollectedEventHandler();

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node body)
    {
        if (body.Name == "Player") 
        {
            EmitSignal(SignalName.Collected);
            QueueFree();
        }
    }
}
