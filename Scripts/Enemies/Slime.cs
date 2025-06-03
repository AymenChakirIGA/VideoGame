using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class Slime : CharacterBody2D
{
	//Variables
	private float gravity = 10f;
	private AnimatedSprite2D animatedSprite;
	[Export] public float leftEdge = 5f;
	[Export] public float rightEdge = 5f;
	[Export] public float speed = 1f;
	private enum States { Idle, Patrolling };
	private States currentState = States.Patrolling;
	private bool isFacingRight = true;
	private Vector2 initialPosition;
	private AnimationPlayer animationPlayer;
	private bool isDeath = false;

	public int Health = 1; // Or whatever health you want

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		initialPosition = this.Position;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		animatedSprite.Play("Idle", customSpeed: 0.5f);

		//Enemy Behaviors Depending on States
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

		base._PhysicsProcess(delta);
		Velocity = isDeath ? Vector2.Zero : velocity;
		MoveAndSlide();
	}

	public void OnAreaBodyEntered(Node2D body)
	{
		
		if (body.IsInGroup("Player") &&!isDeath)
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
	    if (Health <= 0)
	    {
			isDeath = true; //Prevent the enemy to continue patrolling
	        GD.Print("Slime defeated!");
			animationPlayer.Play("Death");
			await ToSignal(animationPlayer, "animation_finished");
	        QueueFree(); 
	    }
	}
}
