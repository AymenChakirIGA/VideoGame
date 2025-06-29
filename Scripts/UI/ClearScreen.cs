using Godot;
using System;

public partial class ClearScreen : CanvasLayer
{
    private AnimationPlayer animationPlayer;

    public override void _Ready()
    {
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animationPlayer.Play("Idle");
    }

    public void Clear()
    {
        // Play the clear screen animation
        animationPlayer.Play("Clear");
    }
}
