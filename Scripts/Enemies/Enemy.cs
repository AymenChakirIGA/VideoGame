using System;
using Godot;

public partial class Enemy : CharacterBody2D
{
    protected enum EnemyStates { Idle, Patrolling, Alerted, Dying, TakenDamage }
    protected EnemyStates currentState = EnemyStates.Patrolling;

    //Patrolling State
    [Export] protected int[] patrollingRange = [0, 0];
    protected Vector2 initialPosition = Vector2.Zero;
    [Export] protected float patrolSpeed = 50.0f;
    private bool isFacingRight = true;

    //Alerted State
    [Export] protected int noticeRange = 70;
    protected float chaseSpeed = 70.0f;

    //Taken Damage

    //Params
    [Export] protected int maxHealth = 3;
    protected int currentHealth;
    protected ProgressBar healthBar;
    protected Timer healthBarTimerAnimation;
    private float alphaValue = 0f;
    protected bool isHealthBarShow = false;
    protected AnimationPlayer animationPlayer;
    protected Timer damageTimer;
    protected Timer knockbackTimer;


    protected void Initialize()
    {
        initialPosition = Position;
        currentHealth = maxHealth;
        currentState = EnemyStates.Patrolling;
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        damageTimer = GetNode<Timer>("DamageTimer");
        knockbackTimer = GetNode<Timer>("KnockbackTimer");
        healthBar = GetNode("Health").GetNode<ProgressBar>("ProgressBar");
        healthBarTimerAnimation = GetNode("Health").GetNode<Timer>("HealthBarAnimationTimer");
        animationPlayer.AnimationFinished += OnAnimationFinished;
    }

    protected void Process()
    {
        if (isHealthBarShow) HealthBarShow();
        else HealthBarHide();
    }

    public void OnAreaBodyEntered(Node2D body)
    {
        if (body.IsInGroup("Player") && currentState != EnemyStates.Dying)
        {
            (body as PlayerController)?.TakeDamage(1);
        }
    }

    private bool isPlayerBehind(Node2D player)
    {
        if (isFacingRight && player.Position.X < this.Position.X) return true;
        if (!isFacingRight && player.Position.X > this.Position.X) return true;
        return false;
    }


    private void HealthBarShow()
    {
        if (alphaValue < 0.4f) alphaValue = Math.Clamp(alphaValue + (float)GetProcessDeltaTime(), 0f, 0.4f);
        healthBar.Modulate = new Color(healthBar.Modulate.R, healthBar.Modulate.G, healthBar.Modulate.B, alphaValue);
    }

    private void HealthBarHide()
    {
        if (alphaValue > 0f) alphaValue = Math.Clamp(alphaValue - (float)GetProcessDeltaTime(), 0f, 0.4f);
        healthBar.Modulate = new Color(healthBar.Modulate.R, healthBar.Modulate.G, healthBar.Modulate.B, alphaValue);
    }

    private float calculateHealthPercentage(int currentHealth, int maxHealth)
    {
        return (100 * currentHealth) / maxHealth;
    }


    //States Behaviors
    protected void Patrolling()
    {
        if (isFacingRight)
        {
            Position = new Vector2(Position.X + patrolSpeed * (float)GetProcessDeltaTime(), Position.Y);
            if (Position.X > patrollingRange[1] + initialPosition.X)
            {
                isFacingRight = false;
                this.Scale = new Vector2(-this.Scale.X, this.Scale.Y);
            }
        }
        else
        {
            Position = new Vector2(Position.X - patrolSpeed * (float)GetProcessDeltaTime(), Position.Y);
            if (Position.X < initialPosition.X - patrollingRange[0])
            {
                isFacingRight = true;
                this.Scale = new Vector2(-this.Scale.X, this.Scale.Y);
            }
        }
    }

    protected void Alerted()
    {
        // Follow Player
        PlayerController player = GetTree().CurrentScene.GetNodeOrNull<PlayerController>("Player");
        if (player != null)
        {
            // Follow Player
            if (isPlayerBehind(player))
            {
                this.Scale = new Vector2(-this.Scale.X, this.Scale.Y);
                isFacingRight = !isFacingRight;
            }
            float newPositionX = Mathf.MoveToward(Position.X, player.Position.X, chaseSpeed * (float)GetProcessDeltaTime());
            Position = new Vector2(newPositionX, Position.Y);
        }
    }

    protected void TakenDamage()
    {
        if (damageTimer.IsStopped())
        {
            currentState = EnemyStates.Patrolling;
        }
    }

    //Switching States Conditions

    public void TakeDamage(int amount, Node2D source = null)
    {
        if (currentState == EnemyStates.TakenDamage) return;
        currentHealth -= amount;
        healthBar.Value = calculateHealthPercentage(currentHealth, maxHealth); //set the right value in the progress bar
        animationPlayer.Play("Damage", customSpeed: 2f);

        //Health Bar Animation
        isHealthBarShow = true;
        healthBarTimerAnimation.Start();

        //Check if dead
        if (currentHealth <= 0)
        {
            currentState = EnemyStates.Dying;
            animationPlayer.Play("Death"); ;
        }
        else
        {
            damageTimer.Start(0.3f);
            bool isHitRight = source != null && source.Position.X > Position.X;
            Knockback(new Vector2(isHitRight ? -1 : 1, -1));
            currentState = EnemyStates.TakenDamage;
        }
    }
    protected bool isEnemyAlerted()
    {
        PlayerController player = GetTree().CurrentScene.GetNodeOrNull<PlayerController>("Player");
        GD.Print("Player Position: " + Position.DistanceTo(player.Position) + " Notice Range: " + noticeRange);
        return player != null && (Position.DistanceTo(player.Position) < noticeRange ? true : false) && !isPlayerBehind(player);
    }

    //Utils and Other Functions
    private void Knockback(Vector2 direction)
    {
        Velocity += direction * 100f;
        GD.Print("Knockback applied: " + Velocity);
        knockbackTimer.Start(0.3f);
    }
    //Signals
    public void OnHitboxBodyEntered(Area2D area)
    {
        // Melee damage
        if (area.IsInGroup("Melee"))
        {
            GD.Print("Hit by melee");
            TakeDamage(1, area.GetParent<Node2D>());
        }
        // Bullet damage
        if (area.IsInGroup("bullets"))
        {
            TakeDamage(1, area.GetParent<Node2D>());
            area.QueueFree();
        }
    }

    private void onHeathBarAnimationTimeOut()
    {
        isHealthBarShow = false;
    }

    protected void OnAnimationFinished(StringName animName)
    {
        if (animName == "Death")
        {
            QueueFree(); // Clean up enemy
        }
    }
    
    public void OnKnockbackTimeout()
    {
        knockbackTimer.Stop();
        Velocity = new Vector2(0, Velocity.Y);
    }

}