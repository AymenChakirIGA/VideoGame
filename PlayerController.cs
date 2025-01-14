using Godot;
using System;

public partial class PlayerController : CharacterBody2D
{
    public const float Speed = 100.0f;
    public const float JumpVelocity = -200.0f;
    public const float DashSpeed = 300.0f;
    private float frinction = .1f;
    private float acceleration = 2.25f;
    private bool isDashing = false;
    private float dashTimer = 0.2f;
    private const float DashDuration = 0.1f; 
    

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;
        if (!IsOnFloor())
        {
            velocity += GetGravity() * (float)delta;
        }
        if (Input.IsActionJustPressed("Jump") && IsOnFloor())
        {
            velocity.Y = JumpVelocity;
        }
        if (Input.IsActionJustPressed("dash") && !isDashing) 
        {
            isDashing = true;
            dashTimer = DashDuration;
        }

        if (isDashing)
        {
            dashTimer -= (float)delta;
            if (dashTimer <= 0)
            {
                isDashing = false;
            }
            else
            {
                Vector2 direction = Input.GetVector("left", "right", "up", "down");
                velocity.X = direction.X * DashSpeed;
            }
        }
        else
        {
            Vector2 direction = Input.GetVector("left", "right", "up", "down");
            if (direction != Vector2.Zero)
            {
                velocity.X = Mathf.MoveToward(velocity.X, direction.X * Speed, acceleration);
            }
            else
            {
                velocity.X = Mathf.Lerp(velocity.X, 0, frinction);
            }
        }

        Velocity = velocity;
        MoveAndSlide();
    }
}
