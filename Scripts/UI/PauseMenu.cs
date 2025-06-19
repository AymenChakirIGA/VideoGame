using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{
	private bool isPaused = false;
	AnimationPlayer animationPlayer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.Visible = false; // Hide the pause menu initially
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animationPlayer.Play("Idle");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!isPaused && Input.IsActionJustPressed("Pause"))
		{
			Pause();
			isPaused = true; // Set the pause state to true
		}
		else if (isPaused && Input.IsActionJustPressed("Pause"))
		{
			Resume();
			isPaused = false; // Set the pause state to false
		}
	}

	private void Resume()
	{
		this.Visible = false; // Hide the pause menu
		GetTree().Paused = false; // Resume the game
		animationPlayer.PlayBackwards("Pause");
	}
	private void Pause()
	{
		this.Visible = true; // Show the pause menu
		GetTree().Paused = true; // Pause the game
		animationPlayer.Play("Pause");
	}
}
