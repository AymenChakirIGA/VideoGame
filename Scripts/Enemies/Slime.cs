using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class Slime : Enemy
{
    // Components
    private AnimatedSprite2D animatedSprite;
    private CollisionShape2D collision;

    // Movement
    [Export] public float leftEdge = 5f;
    [Export] public float rightEdge = 5f;
    [Export] public float speed = 1f;
    private bool isFacingRight = true;

    // Death Physics
    private float deathVerticalVelocity = -150f; // Initial pop-up speed
    private float deathGravity = 500f;           // Pull-down force
    private float alphaValue = 0f; // Alpha value of the Health Bar 
    private float deltaValue = 0f;

    // Melee hit flag
    private bool hitByMelee = false;

    public override void _Ready()
    {
        base.Initialize();
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        collision = GetNode<CollisionShape2D>("CollisionShape2D");

        //Init Variables
        currentHealth = maxHealth;
        healthBar.Value = 100f;

        AddToGroup("mobs");
    }

    public override void _Process(double delta)
    {
        base.Process();
    }

    public override void _PhysicsProcess(double delta)
    {

        //Gravity
        if (!this.IsOnFloor())
        {
            Velocity += GetGravity() * (float)delta;
        }

        //Patrolling
        if (currentState is EnemyStates.Patrolling)
        {
            if (isEnemyAlerted())
            {
                currentState = EnemyStates.Alerted;
                GD.Print("Enemy Alerted!");
            }
            this.Patrolling();
        }
        else if (currentState is EnemyStates.Alerted)
        {
            if (!isEnemyAlerted())
            {
                currentState = EnemyStates.Patrolling;
            }
            this.Alerted();
        }
        else if (currentState == EnemyStates.Dying)
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
            Velocity = Vector2.Zero;
        }
        else if (currentState == EnemyStates.TakenDamage)
        {
            //Enemy Took Damage
            this.TakenDamage();
        }
        MoveAndSlide();
    }

}
