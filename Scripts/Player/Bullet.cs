using Godot;
using System;

public partial class Bullet : Area2D
{
    [Export]
    public float Speed = 750f;
    public Vector2 Direction = Vector2.Right; 
    
    public override void _Ready()
    {
    }
    
    public override void _PhysicsProcess(double delta)
    {
        Position += Direction * Speed * (float)delta;
    }

    private void _on_Bullet_body_entered(Node body)
    {
        if (body.IsInGroup("mobs"))
        {
            body.QueueFree();
        }
        QueueFree();
    }
}
