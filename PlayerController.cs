using Godot;
using System;

public partial class PlayerController : CharacterBody2D
{
    public const float Speed = 100.0f;
    public const float JumpVelocity = -250.0f;
    public const float DashSpeed = 400.0f;
    private float frinction = .1f;
    private float acceleration = 2.25f;
    private bool isDashing = false;
    private float dashTimer = 0.2f;
    private const float DashDuration = 0.1f; 
    private const float DashCooldown = 1.0f; 
    private float dashCooldownTimer = 0f; 
    private bool wasOnFloor = false; 
    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;
        bool isOnFloor = IsOnFloor();

        if (!isOnFloor)
        {
            velocity += GetGravity() * (float)delta;
        }
        if (Input.IsActionJustPressed("Jump") && isOnFloor)
        {
            velocity.Y = JumpVelocity;
        }

        if (isOnFloor && !wasOnFloor) 
        {
            dashCooldownTimer = 0f; 
        }

        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= (float)delta;
        }

        if (Input.IsActionJustPressed("dash") && dashCooldownTimer <= 0)
        {
            isDashing = true;
            dashTimer = DashDuration;
            dashCooldownTimer = DashCooldown;
        }
        if(Input.IsActionJustPressed("Jump") && GetNode<RayCast2D>("RayCast2DLeft").IsColliding() )
        {
            velocity.Y = JumpVelocity;
            velocity.X = -JumpVelocity;
        }
        else if(Input.IsActionJustPressed("Jump") && GetNode<RayCast2D>("RayCastRight").IsColliding())
        {
           velocity.Y = JumpVelocity;
            velocity.X = JumpVelocity;
        }
        if (isDashing)
        {
            dashTimer -= (float)delta;
            if (dashTimer <= 0)
            {
                isDashing = false;
                velocity = Speed * velocity.Normalized();
            }
            else
            {
                Vector2 direction = Input.GetVector("left", "right", "up", "down");
                velocity.X = direction.X * DashSpeed;
                velocity.Y = direction.Y * DashSpeed;
            }
        }
        else
        {
            Vector2 direction = Input.GetVector("left", "right", "up", "down");
            if (direction != Vector2.Zero)
            {
                velocity.X = Mathf.MoveToward(velocity.X, direction.X * Speed, acceleration);
                velocity.Y = Mathf.MoveToward(velocity.Y, direction.Y * Speed, acceleration);
            }
            else
            {
                velocity.X = Mathf.Lerp(velocity.X, 0, frinction);
                velocity.Y = Mathf.Lerp(velocity.Y, 0, frinction);
            }
        }

        Velocity = velocity;
        MoveAndSlide();

        wasOnFloor = isOnFloor; // Update the floor status
    }
}
