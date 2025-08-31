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
    private ProgressBar healthBar;

    // State
    private enum States { Idle, Patrolling, Dying, TakenDamage };
    private States currentState = States.Patrolling;
    private bool isDeath = false;

    // Death Physics
    private float deathVerticalVelocity = -150f; // Initial pop-up speed
    private float deathGravity = 500f;           // Pull-down force
    private bool isHitDirectionRight = true; // Direction of the hit
    private float alphaValue = 0f; // Alpha value of the Health Bar 
    private float deltaValue = 0f;
    private bool isHealthBarShow = false;
    private Timer healthBarTimerAnimation;

    // Health
    private int currentHealth;
    private int maxHealth = 3;

    // Melee hit flag
    private bool hitByMelee = false;

    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        collision = GetNode<CollisionShape2D>("CollisionShape2D");
        damageTimer = GetNode<Timer>("DamageTimer");
        healthBar = GetNode("Health").GetNode<ProgressBar>("ProgressBar");
        healthBarTimerAnimation = GetNode("Health").GetNode<Timer>("HealthBarAnimationTimer");

        //Init Variables
        initialPosition = Position;
        currentHealth = maxHealth;
        healthBar.Value = 100f;

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

        //update the deta value accordingly
        deltaValue = (float)delta;

        if (isHealthBarShow) HealthBarShow();
        else HealthBarHide();
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

    public void OnHitboxBodyEntered(Area2D area)
    {
        // Melee damage
        if (area.IsInGroup("melee"))
        {
            this.isHitDirectionRight = area.Position.X > Position.X;
            TakeDamage(1);
        }
        // Bullet damage
        if (area.IsInGroup("bullets"))
        {
            this.isHitDirectionRight = area.Position.X > Position.X;
            TakeDamage(1);
            area.QueueFree();
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        healthBar.Value = calculateHealthPercentage(currentHealth, maxHealth); //set the right value in the progress bar
        GD.Print($"Slime took {amount} damage! Health now: {currentHealth}");

        animationPlayer.Play("Damage", customSpeed: 2f);

        //Health Bar Animation
        isHealthBarShow = true;
        healthBarTimerAnimation.Start();

        if (currentHealth <= 0)
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

    private void HealthBarShow()
    {
        if (alphaValue < 0.4f) alphaValue = Math.Clamp(alphaValue + deltaValue, 0f, 0.4f);
        healthBar.Modulate = new Color(healthBar.Modulate.R, healthBar.Modulate.G, healthBar.Modulate.B, alphaValue);
    }

    private void HealthBarHide()
    {
        if (alphaValue > 0f) alphaValue = Math.Clamp(alphaValue - deltaValue, 0f, 0.4f);
        healthBar.Modulate = new Color(healthBar.Modulate.R, healthBar.Modulate.G, healthBar.Modulate.B, alphaValue);
    }

    private float calculateHealthPercentage(int currentHealth, int maxHealth)
    {
        return ((100 * currentHealth) / maxHealth);
    }

    //Signals
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

    private void onHeathBarAnimationTimeOut()
    {
        isHealthBarShow = false;
    }

}
