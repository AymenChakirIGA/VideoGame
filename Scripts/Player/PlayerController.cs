using Godot;
using System;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class PlayerController : CharacterBody2D
{
    public float Speed = 100f;
    public const float JumpVelocityY = -350.0f;
    public const float JumpVelocityX = 100f;
    public const float DashSpeed = 400.0f;
    public const float WallSlideFriction = 5f;
    Vector2 dashDirection = Vector2.Zero;
    Vector2 direction = Vector2.Zero;
    private float friction = .1f;
    private float acceleration = 5f;
    private bool isDashing = false;
    private float dashTimer = 0.05f;
    private const float DashDuration = 0.1f;
    private const float DashCooldown = 1.0f;
    private float dashCooldownTimer = 0f;
    private float Stamina = 100.0f;
    private float alphaValue = 0f;
    private float deltaValue = 0f;
    private float gravityController = 1f;
    [Export] private int maxHealth = 3;
    private int blinkCounter = 0;
    private int currentHealth;
    private TextureProgressBar textureProgressBar;
    private AnimatedSprite2D sprite;
    private HBoxContainer heartsContainter;
    private Timer knockedBackTimer;
    private Timer invincibleTimer;
    private Timer blinkTimer;
    private Timer BookShootTimer;
    private AnimatedSprite2D bookSprite;
    private AnimationPlayer bookAnimationPlayer;
    private Vector2 spawnPosition;
    [Export] public PackedScene GameOverScene;
    [Export] PackedScene heartGUI;
    private PlayerCamera playerCamera;
    private bool wasOnFloor = false;
    private bool isWallSliding = false;
    private bool isRunning = false;
    private bool isFacingRight = true;
    private bool isGliding = false;
    private bool canGlide = false;
    private bool isKnockedBack = false;
    private bool isInvincible = false;
    private bool isShooting = false;
    private bool isFatigued = false;
    private Area2D meleeArea;
    private Timer meleeTimer;
    private bool canMelee = true;
    private bool isMeleeAttacking = false;

    Health health;
    [Export]
    public PackedScene BulletScene; // Drag your bullet.tscn here in the editor

    private Node2D muzzle;
    private Vector2 facingDirection = Vector2.Right; // Default facing direction

    public override void _Ready()
    {
        //GET NODE
        health = GetNode<Health>("Health");
        textureProgressBar = GetNode("Stamina").GetNode<TextureProgressBar>("TextureProgressBar");
        textureProgressBar.Value = 100;
        sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        muzzle = GetNode<Node2D>("Muzzle");
        heartsContainter = GetNode<CanvasLayer>("CanvasLayer").GetNode<HBoxContainer>("HeartsContainer");
        knockedBackTimer = GetNode<Timer>("KnockedBackTimer");
        invincibleTimer = GetNode<Timer>("InvincibleTimer");
        blinkTimer = GetNode<Timer>("BlinkTimer");
        bookSprite = GetNode<AnimatedSprite2D>("Book");
        bookAnimationPlayer = bookSprite.GetNode<AnimationPlayer>("AnimationPlayer");
        BookShootTimer = bookSprite.GetNode<Timer>("ShootAnimationTimer");
        playerCamera = GetParent().GetNode<PlayerCamera>("Camera2D");

        //Init Variables
        currentHealth = maxHealth;
        spawnPosition = Position;
        updateHeartUI();
        SetCurrentHealth(4);

        //Checkpoint
        var global = (Global)GetNode("/root/Global");
        if (global.IsCheckpointExists)
        {
            Position = global.CheckpointPosition;
        }

        meleeArea = GetNode<Area2D>("MeleeArea");
        meleeArea.AddToGroup("melee");
        meleeArea.Monitoring = false;
        meleeArea.Visible = false;

        // Add a timer for melee cooldown/duration
        meleeTimer = new Timer();
        meleeTimer.OneShot = true;
        meleeTimer.WaitTime = 0.2f; // Melee active duration
        AddChild(meleeTimer);
        meleeTimer.Timeout += OnMeleeTimerTimeout;
    }
    public override void _Process(double delta)
    {
        DebugPlayer();
        if (Input.IsActionPressed("right"))
            facingDirection = Vector2.Right;
        else if (Input.IsActionPressed("left"))
            facingDirection = Vector2.Left;

        if (Input.IsActionJustPressed("shoot"))
            Shoot();

        //Stamina Process Code
        StaminaRecovery();

        //Handle Fatigue State
        HandleFatigueState();

        //Hide Stamina if no activity
        if (!isRunning && !isGliding && Stamina >= 100.0f) StaminaHide();
        BookAnimation();

        // Melee attack input
        if (Input.IsActionJustPressed("melee") && canMelee)
        {
            MeleeAttack();
        }
    }
    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;
        bool isOnFloor = IsOnFloor();
        deltaValue = (float)delta;

        if (!isOnFloor)
        {
            //Handles Gravity? (it should be handeled directly by the engine)
            velocity += GetGravity() * (float)delta * gravityController;
        }

        //Get the input from the player
        direction = !isDashing ? Input.GetVector("left", "right", "up", "down") : dashDirection;
        //Handle Mouvement: Jumping, Dashing, Wall Jumping

        //Jump and Wall Jump
        velocity = HandleJump(velocity);

        velocity.Y = HandleGlide(velocity);

        //Dash
        dashDirection = HandleDashState(direction, delta);

        //Horizontal Movement
        velocity.X = !isKnockedBack ? (!isDashing ? HorizontalMovement(velocity, direction, Speed) : dashDirection.X * 600f) : velocity.X;
        //Vertical Movement
        velocity.Y = VerticalMovement(velocity);

        velocity.Y = isWallSliding ? VerticalMovement(velocity) : velocity.Y;


        //Dash Timer
        if (isOnFloor && !wasOnFloor)
        {
            dashCooldownTimer = 0f;
        }

        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= (float)delta;
        }

        //Speed Adjustment
        if (isRunning)
        {
            Speed = Mathf.Lerp(Speed, 200f, deltaValue * 5f);
        }
        else if (isGliding)
        {
            Speed = Mathf.Lerp(Speed, 170f, deltaValue * 5f);
        }
        else
        {
            Speed = Mathf.Lerp(Speed, 100f, deltaValue * 50f);
        }

        //Animation Handler:
        AnimationHandler();


        Velocity = velocity;
        MoveAndSlide();

        wasOnFloor = isOnFloor; // Update the floor status
    }

    private void HandleFatigueState()
    {
        if (!isFatigued)
        {
            isFatigued = Stamina <= 0f;
            textureProgressBar.TintOver = new Color(41, 41, 41, alphaValue); //Default Color
            textureProgressBar.TintProgress = new Color(0, 221, 144, alphaValue); //Default Color
            
        }
        else
        {
            // Tint part of the progress bar red for visual indication
            textureProgressBar.TintOver = new Color(255f, 0, 0, alphaValue); 
            textureProgressBar.TintProgress = new Color(255f, 0, 0, alphaValue);

            if (Stamina < 100.0f)
            {
                // If the player is fatigued, reduce speed and disable running
                Speed = Mathf.Lerp(Speed, 50f, deltaValue * 5f);
                isRunning = false;
            }
            else
            {
                isFatigued = false;
            }

        }
    }

    private Vector2 HandleJump(Vector2 velocity)
    {
        //Jumping and wall jumping mechanics
        bool isOnWall = GetNode<RayCast2D>("RayCast2DLeft").IsColliding() || GetNode<RayCast2D>("RayCast2DRight").IsColliding();
        if (Input.IsActionJustPressed("Jump"))
        {
            if (IsOnFloor())
            {
                velocity.Y = JumpVelocityY; //Basic Jump  
                canGlide = false;
            }
            else if (isOnWall && Input.IsActionPressed("right"))
            {
                //Wall jumping right
                velocity.Y = JumpVelocityY;
                velocity.X = -JumpVelocityX;
            }
            else if (isOnWall && Input.IsActionPressed("left"))
            {
                velocity.Y = JumpVelocityY;
                velocity.X = JumpVelocityX;
            }

        }
        return velocity;
    }

    private float HandleGlide(Vector2 velocity)
    {
        if (!IsOnFloor() && Input.IsActionPressed("Jump") && canGlide && Stamina > 0f)
        {
            //Gliding when it's not on floor
            Stamina -= 0.5f;
            textureProgressBar.Value = Stamina;
            StaminaShow();
            isGliding = true;
            if (velocity.Y > 0)
            {
                velocity.Y = Math.Clamp(velocity.Y, 0, 50f);
                gravityController = 0.08f;
            }
        }
        else
        {
            //end of gliding state
            isGliding = false;
            gravityController = 1f;

        }
        if (!canGlide)
        {
            // Handle the case when the charachter just jumped
            canGlide = Input.IsActionJustReleased("Jump");
        }
        return velocity.Y;
    }


    private float HorizontalMovement(Vector2 velocity, Vector2 direction, float Speed)
    {
        //Handles the horizontal movement of the player
        isRunning = Input.IsActionPressed("Run") && Stamina > 0 && direction != Vector2.Zero && !isGliding && !isFatigued;
        if (direction != Vector2.Zero)
        {
            velocity.X += direction.X * acceleration;
            // velocity.Y +=  direction.Y *  acceleration;
            velocity.X = Math.Clamp(velocity.X, -Speed, Speed); // ensure that the speed dosn't exceed it limit
        }
        else
        {
            velocity.X = Mathf.Lerp(velocity.X, 0, friction);
            // velocity.Y = Mathf.Lerp(velocity.Y, 0, friction);
        }

        //Running System
        if (isRunning)
        {
            //Stamina is greater than 0, the player will run
            if (Stamina > 0)
            {
                Stamina -= 0.5f;
                textureProgressBar.Value = Stamina;
            }
            StaminaShow();
        }
        Math.Clamp(Stamina, 0f, 100f);
        return velocity.X;
    }

    private void StaminaRecovery()
    {
        if (Stamina < 100f && !isRunning && !isGliding)
        {
            //Recover Stamina
            Stamina += 0.5f;
            textureProgressBar.Value = Stamina;
            StaminaShow();
        }
    }

    private float VerticalMovement(Vector2 velocity)
    {
        //Check if player is on a wall
        isWallSliding = (GetNode<RayCast2D>("RayCast2DLeft").IsColliding() && Input.IsActionPressed("left")
                        || GetNode<RayCast2D>("RayCast2DRight").IsColliding() && Input.IsActionPressed("right")) && !IsOnFloor();

        if (isWallSliding)
        {
            velocity.Y += WallSlideFriction;
            velocity.Y = Mathf.Min(velocity.Y, WallSlideFriction);
        }
        return velocity.Y;
    }

    private Vector2 HandleDashState(Vector2 LastRecordedDirection, double delta = 0.0)
    {
        //Handles the dash state of the player

        //If the player just pressed the dash key and the cooldown is over and the player is not already dashing
        if (Input.IsActionJustPressed("dash") && dashCooldownTimer <= 0 && !isDashing)
        {
            //User Just Pressed Dash Key
            isDashing = true;
            dashDirection = LastRecordedDirection;
            dashTimer = DashDuration;
            dashCooldownTimer = DashCooldown;
        }

        if (isDashing)
        {
            dashTimer -= (float)delta;
            if (dashTimer <= 0)
            {
                isDashing = false;
            }
        }
        return dashDirection;
    }

    private void StaminaHide()
    {
        //Gradually Hide the stamina bar
        if (alphaValue > 0f)
        {
            alphaValue = Math.Clamp(alphaValue - deltaValue, 0f, 0.5f);
        }
        textureProgressBar.TintUnder = new Color(textureProgressBar.TintUnder.R, textureProgressBar.TintUnder.G, textureProgressBar.TintUnder.B, alphaValue);
        textureProgressBar.TintOver = new Color(textureProgressBar.TintOver.R, textureProgressBar.TintOver.G, textureProgressBar.TintOver.B, alphaValue);
        textureProgressBar.TintProgress = new Color(textureProgressBar.TintProgress.R, textureProgressBar.TintProgress.G, textureProgressBar.TintProgress.B, alphaValue);
    }

    private void StaminaShow()
    {
        //Gradually Show the stamina bar
        if (alphaValue < 0.5f)
        {
            //Gradually Increase the opacity of the stamina bar
            alphaValue = Math.Clamp(alphaValue + deltaValue, 0f, 0.5f);
        }
        textureProgressBar.TintUnder = new Color(textureProgressBar.TintUnder.R, textureProgressBar.TintUnder.G, textureProgressBar.TintUnder.B, alphaValue);
        textureProgressBar.TintOver = new Color(textureProgressBar.TintOver.R, textureProgressBar.TintOver.G, textureProgressBar.TintOver.B, alphaValue);
        textureProgressBar.TintProgress = new Color(textureProgressBar.TintProgress.R, textureProgressBar.TintProgress.G, textureProgressBar.TintProgress.B, alphaValue);
    }

    private void AnimationHandler()
    {
        //Handle Rotation of character
        if (isFacingRight && direction.X < 0)
        {
            isFacingRight = false;
            sprite.FlipH = true;
            // Move muzzle to the left
            muzzle.Position = new Vector2(-Math.Abs(muzzle.Position.X), muzzle.Position.Y);
        }
        else if (!isFacingRight && direction.X > 0)
        {
            isFacingRight = true;
            sprite.FlipH = false;
            // Move muzzle to the right
            muzzle.Position = new Vector2(Math.Abs(muzzle.Position.X), muzzle.Position.Y);
        }

        //Time to Animate
        if (direction.X != 0 && !isRunning && this.IsOnFloor())
        {
            sprite.Play("Walk");
        }
        else if (isRunning && this.IsOnFloor())
        {
            sprite.Play("Run");
        }
        else if (!this.IsOnFloor() && !isGliding)
        {
            sprite.Play("Jump");
        }
        else if (isGliding)
        {
            sprite.Play("Umbrella_2");
        }
        else
        {
            sprite.Play("Idle");
        }
        return;
    }

    public void DebugPlayer()
    {

    }

    private void BookAnimation()
    {
        //Handle Book Movement
        if (BookShootTimer.IsStopped())
        {
            bookSprite.Play("Idle");
            if (isFacingRight)
            {
                bookAnimationPlayer.Play("Idle");
            }
            else
            {
                bookAnimationPlayer.Play("Idle_Reverse");
            }
        }
        else
        {
            bookSprite.Play("Open");
            if (isFacingRight)
            {
                bookAnimationPlayer.Play("Shoot");
            }
            else
            {
                bookAnimationPlayer.Play("Shoot_Reverse");
            }

        }
    }

    private void Shoot()
    {
        if (BulletScene == null || muzzle == null)
        {
            GD.PrintErr("BulletScene or Muzzle not set!");
            return;
        }
        var bullet = (Bullet)BulletScene.Instantiate();
        bullet.Position = muzzle.GlobalPosition;
        bullet.Direction = facingDirection;
        isShooting = true;
        BookShootTimer.Start();

        var sprite = bullet.GetNode<Sprite2D>("Sprite2D");
        if (sprite == null)
            GD.PrintErr("Sprite2D not found on bullet!");
        else
            sprite.FlipH = (facingDirection == Vector2.Left);

        GetTree().CurrentScene.AddChild(bullet);
    }

    private void MeleeAttack()
    {
        // Reset all slimes' melee flags before a new attack
        foreach (var slime in GetTree().GetNodesInGroup("mobs"))
        {
            if (slime is Slime s)
                s.ResetMeleeHit();
        }

        canMelee = false;
        isMeleeAttacking = true;
        meleeArea.Monitoring = true;
        meleeArea.Visible = true;
        meleeTimer.Start();
        sprite.Play("Melee_1");
    }

    private void OnMeleeTimerTimeout()
    {
        meleeArea.Monitoring = false;
        meleeArea.Visible = false;
        canMelee = true;
        isMeleeAttacking = false;
    }

    // For the slime to check if the player is attacking
    public bool IsMeleeAttacking()
    {
        return isMeleeAttacking;
    }

    //Health System
    public int GetHealth()
    {
        return currentHealth;
    }

    public void SetCurrentHealth(int newHealth)
    {
        currentHealth = Math.Clamp(newHealth, 0, maxHealth);
        updateHeartUI();
        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    private void updateHeartUI()
    {
        // Remove all hearts
        foreach (Node heartGUI in heartsContainter.GetChildren())
        {
            heartGUI.QueueFree();
        }
        // Add hearts for current health
        for (int i = 0; i < currentHealth; i++)
        {
            heartsContainter.AddChild(heartGUI.Instantiate());
        }
    }

    //Take Damage from an enemie
    public async Task TakeDamage(int heartDamage)
    {
        if (isInvincible) return;
        SetCurrentHealth(currentHealth - heartDamage); 
        blinkTimer.Start();
        knockedBackTimer.Start();
        invincibleTimer.Start();
        Vector2 velocity = Velocity;
        isKnockedBack = true;
        isInvincible = true;
        velocity.X = 130f * (isFacingRight ? -1 : 1);
        velocity.Y = -100f;
        Velocity = velocity;

        //Update hearts counter
        if (currentHealth <= 0)
        {
            //if health is lower than 0 it's game over
            GameOver();
        }
        var animationPlayer = heartsContainter.GetChild(currentHealth-1).GetNode<AnimationPlayer>("AnimationPlayer");
        animationPlayer.Play("Break");
        await ToSignal(animationPlayer, "animation_finished");
    }

    //Set Spawn Position ex: when the player Collides with a checkpoint
    public void SetSpawnPosition(Vector2 newSpawnPosition)
    {
        spawnPosition = newSpawnPosition;
    }

    //GameOver
    private void GameOver()
    {
        AddChild(GameOverScene.Instantiate());
    }

    //Signals
    private void OnKnockedBackTimerTimeout()
    {
        isKnockedBack = false;

    }

    private void OnInvincibleTimeout()
    {
        isInvincible = false;
        sprite.Visible = true; // in case counter ends with a non mod 2
        blinkCounter = 0;
    }

    //Custom Animation
    private void OnBlinkTimeout()
    {
        //blink one out of two times
        if (blinkCounter % 2 == 0)
            sprite.Visible = false;
        else
            sprite.Visible = true;

        blinkCounter++;
        if (isInvincible) blinkTimer.Start();
        else sprite.Visible = true;
    }

}
