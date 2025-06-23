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

	// State
	private enum States { Idle, Patrolling, Dying };
	private States currentState = States.Patrolling;
	private bool isDeath = false;

	// Death Physics
	private float deathVerticalVelocity = -150f; // Initial pop-up speed
	private float deathGravity = 500f;           // Pull-down force

	// Health
	public int Health = 3;

	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		collision = GetNode<CollisionShape2D>("CollisionShape2D");
		initialPosition = Position;

		// Connect animation finished for cleanup
		animationPlayer.AnimationFinished += OnAnimationFinished;
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
		if (area.IsInGroup("bullets"))
		{
			await TakeDamage(1);
			area.QueueFree();
		}
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
			animationPlayer.Play("Death");
		}
	}

	private void OnAnimationFinished(StringName animName)
	{
		if (animName == "Death")
		{
			QueueFree(); // Clean up enemy
		}
	}
}
