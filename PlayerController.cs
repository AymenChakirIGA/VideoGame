using Godot;
using System;
using System.Diagnostics;

public partial class PlayerController : CharacterBody2D
{
    public const float Speed = 100f;
    public const float JumpVelocityY = -250.0f;
    public const float JumpVelocityX = 100f;
    public const float DashSpeed = 400.0f;
    public const float WallSlideFriction = 5f; 
    Vector2 dashDirection = Vector2.Zero;
    private float frinction = .1f;
    private float acceleration = 5f;
    private bool isDashing = false;
    private float dashTimer = 0.05f;
    private const float DashDuration = 0.1f; 
    private const float DashCooldown = 1.0f; 
    private float dashCooldownTimer = 0f;
    private bool wasOnFloor = false; 
    private bool isWallSliding = false;
    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;
        bool isOnFloor = IsOnFloor();
        if (!isOnFloor)
        {
            //Handles Gravity? (it should be handeled directly by the engine)
            velocity += GetGravity() * (float)delta;
        }

        //Get the input from the player
        Vector2 direction = !isDashing? Input.GetVector("left", "right", "up", "down"): dashDirection;
        //Handle Mouvement: Jumping, Dashing, Wall Jumping

        //Jump and Wall Jump
        velocity = HandleJump(velocity);

        //Dash
        dashDirection = HandleDashState(direction, delta);

        //Horizontal Movement
        velocity.X = !isDashing?HorizontalMovement(velocity, direction, Speed): dashDirection.X * 600f;
        //Vertical Movement
        velocity.Y = VerticalMovement(velocity);
        
        velocity.Y = isWallSliding? VerticalMovement(velocity): velocity.Y;


        if (isOnFloor && !wasOnFloor) 
        {
            dashCooldownTimer = 0f; 
        }

        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= (float)delta;
        }


        Velocity = velocity;
        MoveAndSlide();

        wasOnFloor = isOnFloor; // Update the floor status
    }

    private Vector2 HandleJump(Vector2 velocity){
        //Jumping and wall jumping mechanics
        bool isOnWall = GetNode<RayCast2D>("RayCast2DLeft").IsColliding() || GetNode<RayCast2D>("RayCast2DRight").IsColliding() ;
        Debug.WriteLine(isOnWall);
        if (Input.IsActionJustPressed("Jump"))
        {
            if(IsOnFloor()) velocity.Y = JumpVelocityY; //Basic Jump

            else if(isOnWall && Input.IsActionPressed("right")){
                //Wall jumping right
                velocity.Y = JumpVelocityY;
                velocity.X = -JumpVelocityX;
            }
            else if(isOnWall && Input.IsActionPressed("left")){
                velocity.Y = JumpVelocityY;
                velocity.X = JumpVelocityX;
            }

        }
        return velocity;
    }


    private float HorizontalMovement(Vector2 velocity, Vector2 direction, float Speed)
    {
        //Handles the horizontal movement of the player
        Debug.Print("Direction: " + direction);
        if (direction != Vector2.Zero)
        {
            velocity.X +=  direction.X * acceleration;
            // velocity.Y +=  direction.Y *  acceleration;
            velocity.X = Math.Clamp(velocity.X, -Speed, Speed); // ensure that the speed dosn't exceed it limit
        }
        else
        {
            velocity.X = Mathf.Lerp(velocity.X, 0, frinction);
            // velocity.Y = Mathf.Lerp(velocity.Y, 0, frinction);
        }
        return velocity.X;
    }

    private float VerticalMovement(Vector2 velocity){
        //Check if player is on a wall
        isWallSliding = (GetNode<RayCast2D>("RayCast2DLeft").IsColliding() && Input.IsActionPressed("left") 
                        || GetNode<RayCast2D>("RayCast2DRight").IsColliding() && Input.IsActionPressed("right"))&&!IsOnFloor();
        
        if(isWallSliding){
            velocity.Y+= WallSlideFriction;
            velocity.Y = Mathf.Min(velocity.Y,WallSlideFriction);
        }
        return velocity.Y;
    }

    private Vector2 HandleDashState( Vector2 LastRecordedDirection, double delta = 0.0){
        //Handles the dash state of the player

        //If the player just pressed the dash key and the cooldown is over and the player is not already dashing
        if (Input.IsActionJustPressed("dash") && dashCooldownTimer <= 0 && !isDashing)
        {
            //User Just Pressed Dash Key
            isDashing = true; 
            dashDirection = LastRecordedDirection;
            dashTimer = DashDuration;
            dashCooldownTimer = DashCooldown;
            Debug.Print("Dash");
        }

        if(isDashing){
            dashTimer -= (float)delta;
            if(dashTimer <= 0){
                isDashing = false;
                Debug.Print("Dash Over");
            }
        }
        return dashDirection;
    }


}
