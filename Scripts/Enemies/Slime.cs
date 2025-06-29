using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class Slime : CharacterBody2D
{
    // Components
    private AnimatedSprite2D animatedSprite;
    private AnimationPlayer animationPlayer;
    private CollisionShape2D collision;

    // Movement
    [Export] public float leftEdge = 5f;
    [Export] public float rightEdge = 5f;
    [Export] public float speed = 1f;
    private bool isFacingRight = true;
    private Vector2 initialPosition;
    private Timer damageTimer;

    // State
    private enum States { Idle, Patrolling, Dying, TakenDamage };
    private States currentState = States.Patrolling;
    private bool isDeath = false;

    // Death Physics
    private float deathVerticalVelocity = -150f; // Initial pop-up speed
    private float deathGravity = 500f;           // Pull-down force
    private bool isHitDirectionRight = true; // Direction of the hit

    // Health
    public int Health = 3;

    // Melee hit flag
    private bool hitByMelee = false;

    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        collision = GetNode<CollisionShape2D>("CollisionShape2D");
        damageTimer = GetNode<Timer>("DamageTimer");
        initialPosition = Position;

        // Connect animation finished for cleanup
        animationPlayer.AnimationFinished += OnAnimationFinished;

        AddToGroup("mobs");
    }

    public override void _Process(double delta)
    {
        if (!isDeath)
        {
            animatedSprite.Play("Idle", customSpeed: 0.5f);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        //Gravity
        if (!this.IsOnFloor())
        {
            velocity += GetGravity() * (float)delta;
        }

        //Patrolling
        if (currentState is States.Patrolling)
        {
            //Going Right
            if (isFacingRight)
            {
                if (this.Position.X <= initialPosition.X + rightEdge)
                {
                    velocity.X = speed * (float)delta;
                }
                else
                {
                    isFacingRight = !isFacingRight;
                    this.Scale = new Vector2(-this.Scale.X, this.Scale.Y);
                }
            }
            else
            {
                if (this.Position.X >= initialPosition.X - leftEdge)
                {
                    velocity.X = -speed * (float)delta;
                }
                else
                {
                    isFacingRight = !isFacingRight;
                    this.Scale = new Vector2(-this.Scale.X, this.Scale.Y);
                }
            }
        }
        else if (currentState == States.Dying)
        {
            // Play animation once
            if (!animationPlayer.IsPlaying())
                animationPlayer.Play("Death");

            // Disable collision
            collision.Disabled = true;

            // Apply vertical "bounce" motion
            Position += new Vector2(10f * (float)delta, deathVerticalVelocity * (float)delta);
            deathVerticalVelocity += deathGravity * (float)delta;

            Rotation += (float)delta * 10f; // Rotate slowly while dying

            // Stop velocity
            velocity = Vector2.Zero;
        }
        else if (currentState == States.TakenDamage)
        {
            //Enemy Took Damage
            if (damageTimer.IsStopped())
            {
                damageTimer.Start(0.5f); // Reset the timer for the next damage state
                velocity = Vector2.Zero; // Stop movement during damage animation
                velocity.Y = -100f; // Small jump effect
            }
            velocity.X = isHitDirectionRight ? -speed * 5 * (float)delta : speed * 5 * (float)delta;
        }

        Velocity = isDeath ? Vector2.Zero : velocity;
        MoveAndSlide();
    }

    public void OnAreaBodyEntered(Node2D body)
    {
        if (body.IsInGroup("Player") && !isDeath)
        {
            (body as PlayerController)?.TakeDamage(1);
        }
    }

    public async void OnHitboxBodyEntered(Area2D area)
    {
        // Melee damage
        if (area.IsInGroup("melee"))
        {
            var player = area.GetParent() as PlayerController;
            if (player != null && player.IsMeleeAttacking() && !hitByMelee)
            {
                hitByMelee = true;
                await TakeDamage(1);
            }
        }
        // Bullet damage
        if (area.IsInGroup("bullets"))
        {
            this.isHitDirectionRight = area.Position.X > Position.X;
            await TakeDamage(1);
            area.QueueFree();
        }
    }

    public void ResetMeleeHit()
    {
        hitByMelee = false;
    }

    public async Task TakeDamage(int amount)
    {
        Health -= amount;
        GD.Print($"Slime took {amount} damage! Health now: {Health}");

        animationPlayer.Play("Damage", customSpeed: 2f);

        if (Health <= 0)
        {
            isDeath = true;
            currentState = States.Dying;
            deathVerticalVelocity = -150f; // Give it the pop-up effect
            animationPlayer.Play("Death"); ;
        }
        else
        {
            currentState = States.TakenDamage;
        }
    }

    private void OnAnimationFinished(StringName animName)
    {
        if (animName == "Death")
        {
            QueueFree(); // Clean up enemy
        }
    }

    private void onTimerTimout()
    {
        // Reset the state after damage animation
        GD.Print("Damage animation finished, resetting state.");
        damageTimer.Stop();
        if (currentState == States.TakenDamage)
        {
            currentState = States.Patrolling;
        }
    }
}
