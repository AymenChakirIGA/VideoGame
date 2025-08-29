using Godot;
using System;
using System.Diagnostics;

public partial class Hud : CanvasLayer
{
	private Label actName;
	private Label coinsLabel;
	private Label timerLabel;
	private TextureRect weatherGUI;
	private Timer timer;
	private enum weatherState { Sunny, Night }
	[Export] public string ActName = "Act 1";
	public static int numberOfCoins = 0;
	private int minutes = 0;
	private int seconds = 0;
	private int miliSeconds = 0;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		actName = GetNode("VBoxContainer").GetNode<Label>("ActName");
		coinsLabel = GetNode("VBoxContainer").GetNode<Label>("Coins");
		timerLabel = GetNode("VBoxContainer").GetNode<Label>("TimerLabel");
		timer = GetNode<Timer>("Timer");
		actName.Text = ActName;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		coinsLabel.Text = "Coins: " + numberOfCoins.ToString();

		string minutesString = minutes < 10 ? "0" + minutes.ToString() : minutes.ToString();
		string secondsString = seconds < 10 ? "0" + seconds.ToString() : seconds.ToString();
		string miliSecondsString = miliSeconds < 10 ? "0" + miliSeconds.ToString() : miliSeconds.ToString();
		timerLabel.Text = "Timer: " + minutesString + ":" + secondsString + ":" + miliSecondsString;
	}

	//Signals
	private void OnTimeout()
	{
		//Update Timer
		if (!(miliSeconds + 1 >= 60))
		{
			miliSeconds++;	
		}
		else if (!(seconds + 1 >= 60))
		{
			miliSeconds = 0;
			seconds++;
		}
		else
		{
			miliSeconds = 0;
			seconds = 0;
			minutes++;
		}
	}
}
