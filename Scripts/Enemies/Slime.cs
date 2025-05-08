using Godot;
using System;
using System.Diagnostics;

public partial class Slime : CharacterBody2D
{
	//Variables
	private float gravity = 10f;
	private AnimatedSprite2D animatedSprite;
	[Export] public float leftEdge = 5f;
	[Export] public float rightEdge = 5f;
	[Export] public float speed = 1f;
	private enum States {Idle,Patrolling};
	private States currentState = States.Patrolling;
	private bool isFacingRight = true;
	private Vector2 initialPosition;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		initialPosition = this.Position;
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		animatedSprite.Play("Idle", customSpeed:0.5f);

		//Enemy Behaviors Depending on States
		
		
	}

    public override void _PhysicsProcess(double delta)
    {
		Vector2 velocity = Velocity;

		//Gravity
		if(!this.IsOnFloor()){
			velocity += GetGravity() * (float)delta;
		}

		//Patrolling
		if (currentState is States.Patrolling)
		{
			//Going Right
			if(isFacingRight){
				if(this.Position.X <= initialPosition.X + rightEdge){
					Debug.Print(Position.ToString());
					velocity.X = speed * (float)delta;
				}
				else{
					isFacingRight = !isFacingRight;
					this.Scale = new Vector2(-this.Scale.X, this.Scale.Y);
				}
			}

			else{
				if(this.Position.X >= initialPosition.X - leftEdge){
					Debug.Print(Position.ToString());
					velocity.X = -speed * (float)delta;
				}
				else{
					Debug.Print("Switch");
					isFacingRight = !isFacingRight;
					this.Scale = new Vector2(-this.Scale.X, this.Scale.Y);
				}
			}

		}

        base._PhysicsProcess(delta);
		Velocity = velocity;
		MoveAndSlide();
    }


}
