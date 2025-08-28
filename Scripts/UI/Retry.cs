using Godot;
using System;

public partial class Retry : Button
{
	private AnimationPlayer animationPlayer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animationPlayer.Play("Init");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Pressed()
	{
		GetTree().Paused = false;
    	this.Visible = false; 
		GetTree().ReloadCurrentScene();
	}

	private void onHover()
	{
		animationPlayer.Play("Hover");

	}

	private void onNormal()
	{
		animationPlayer.Play("Normal");
	}
}
