using Godot;
using System;

public partial class Mushroom : CharacterBody2D
{
    public const float Speed = 300.0f;
    private Vector2 velocity = new Vector2(Speed, 0);
	private RayCast2D groundRayCast;
    public override void _Ready()
    {
        Velocity = velocity;
		 groundRayCast = GetNode<RayCast2D>("GroundRayCast");
    }

    public override void _PhysicsProcess(double delta)
    {
		if (!groundRayCast.IsColliding())
        {
            ReverseDirection();
        }

        if (!IsOnFloor())
        {
            Velocity += GetGravity() * (float)delta;
        }
        KinematicCollision2D collision = MoveAndCollide(Velocity * (float)delta);
		GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("walk");
        if (collision != null && collision.GetNormal().X != 0)
        {
            ReverseDirection();
        }
        Velocity = velocity;
    }

    private void ReverseDirection()
    {
        velocity.X = -velocity.X;
		  var animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animatedSprite.FlipH = !animatedSprite.FlipH;
         groundRayCast.TargetPosition = new Vector2(-groundRayCast.TargetPosition.X, groundRayCast.TargetPosition.Y);
    
    }
}
