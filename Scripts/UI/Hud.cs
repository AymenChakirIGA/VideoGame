using Godot;
using System;
using System.Diagnostics;

public partial class Hud : CanvasLayer
{
	private Label label;
	private TextureRect weatherGUI;
	private enum weatherState { Sunny, Night}
	[Export] public string ActName = "Act 1";
	[Export] private weatherState currentWeather = weatherState.Sunny;
	[Export] private Texture2D Sunny;
	[Export] private Texture2D Night;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		label = GetNode<Label>("Label");
		label.Text = ActName;
		Debug.Print(currentWeather.ToString());
		weatherGUI = GetNode<TextureRect>("TextureRect2");

		switch (currentWeather)
		{
			case weatherState.Sunny:
				weatherGUI.Texture = Sunny;
				break;
			case weatherState.Night:
				weatherGUI.Texture = Night;
				break;
			default:
				weatherGUI.Texture = Sunny;
				break;

		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
