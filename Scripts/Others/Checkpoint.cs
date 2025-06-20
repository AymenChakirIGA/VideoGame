using Godot;
using System;

public partial class Checkpoint : Node2D
{
	private bool isActive = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	//Signals
	private void OnAreaBodyEntered(Node2D body)
	{
		if (body.IsInGroup("Player") && !isActive)
		{
			isActive = true;
			var global = (Global)GetNode("/root/Global");
			global.CheckpointPosition = GlobalPosition;
			global.IsCheckpointExists = true;
			
		}
	}
}
