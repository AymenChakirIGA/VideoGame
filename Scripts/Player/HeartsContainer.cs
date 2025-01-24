using Godot;
using System;

public partial class HeartsContainer : HBoxContainer
{
	[Export] public PackedScene heart;
	float health = 3;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		for (int i = 0; i < health; i++)
		{
			AddHeart();
		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public void AddHeart()
	{
		Node newHeart = heart.Instantiate();
		AddChild(newHeart);
	}
	public void RemoveHeart()
	{
		if (GetChildCount() >= 0)
		{
			GetChild(GetChildCount() - 1).QueueFree();
		}
	}
}
