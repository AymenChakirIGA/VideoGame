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
		GetTree().ChangeSceneToFile("res://Scenes/levels/level_0/level_0.tscn");
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
