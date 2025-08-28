using Godot;
using System;
using System.Threading.Tasks;

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
		GetTree().Paused = false; // Ensure the game is not paused at the start
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!isPaused && Input.IsActionJustPressed("Pause"))
		{
			Pause();
		}
		else if (isPaused && Input.IsActionJustPressed("Pause"))
		{
			Resume();
		}
	}

	private void Resume()
	{
		this.Visible = false; // Hide the pause menu
		GetTree().Paused = false; // Resume the game
		animationPlayer.PlayBackwards("Pause");
		isPaused = false; 
	}
	private void Pause()
	{
		this.Visible = true; // Show the pause menu
		isPaused = true; 
		GetTree().Paused = true; // Pause the game
		animationPlayer.Play("Pause");
	}

	private void Restart()
	{
		GetTree().Paused = false;
    	this.Visible = false; 
		GetTree().ReloadCurrentScene(); // Restart the current scene
		
	}

	private void Quit()
	{
		GetTree().Quit();
	}
}
