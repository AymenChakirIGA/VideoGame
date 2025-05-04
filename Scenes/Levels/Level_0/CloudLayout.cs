using Godot;
using System;

public partial class CloudLayout : ParallaxLayer
{
	[Export] public float Cloud_speed = -15f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		this.MotionOffset = new Vector2(this.MotionOffset.X+Cloud_speed * (float)delta, this.MotionOffset.Y);
	}
}
