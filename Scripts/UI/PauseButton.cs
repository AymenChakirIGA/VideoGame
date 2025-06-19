using Godot;
using System;

public partial class PauseButton : Button
{

	private AnimationPlayer animationPlayerButton1;
	private AnimationPlayer animationPlayerButton2;
	private AnimationPlayer animationPlayerButton3;
	public override void _Ready()
	{
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExit;
		animationPlayerButton1 = GetParent().GetChild(0).GetNode<AnimationPlayer>("AnimationPlayer");
		animationPlayerButton2 = GetParent().GetChild(1).GetNode<AnimationPlayer>("AnimationPlayer");
		animationPlayerButton3 = GetParent().GetChild(2).GetNode<AnimationPlayer>("AnimationPlayer");

	}

	private void OnMouseEntered()
	{
		GD.Print($"Mouse entered: {Name}");

		switch (Name)
		{
			case "Button":
				GD.Print("Hovered Button 1 logic");
				animationPlayerButton1.Play("hover");
				break;
			case "Button2":
				GD.Print("Hovered Button 2 logic");
				animationPlayerButton2.Play("hover");
				break;
			case "Button3":
				GD.Print("Hovered Button 3 logic");
				animationPlayerButton3.Play("hover");
				break;
		}
	}

	private void OnMouseExit()
	{
		switch (Name)
		{
			case "Button":
				animationPlayerButton1.PlayBackwards("hover");
				break;
			case "Button2":
				animationPlayerButton2.PlayBackwards("hover");
				break;
			case "Button3":
				animationPlayerButton3.PlayBackwards("hover");
				break;
		}
	}
}
