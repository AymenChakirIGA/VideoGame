using Godot;
using System;
using System.Security.Cryptography;

public partial class ChearingCompanions : Node2D
{
	private AnimatedSprite2D animatedSprite;
	private AnimationPlayer animationPlayer;
	private Timer timer;
	private bool isChearing = false;

	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animatedSprite.Play("Idle");
	}

	//Signals
	private void OnTimout()
	{
		// Chearing Animation Cycling
		if (isChearing)
		{

			animatedSprite.Play("Idle");
			animationPlayer.Play("Idle");
			isChearing = false;
		}
		else
		{
			animatedSprite.Play("Chear");
			animationPlayer.Play("Chear");
			isChearing = true;
		}
	}
}
