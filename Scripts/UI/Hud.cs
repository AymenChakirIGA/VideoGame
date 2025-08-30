using Godot;
using System;
using System.Diagnostics;

public partial class Hud : CanvasLayer
{
	private Label coinsLabel;
	private Label timerLabel;
	private Timer timer;
	public static int numberOfCoins = 0;
	private int minutes = 0;
	private int seconds = 0;
	private int miliSeconds = 0;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		coinsLabel = GetNode("VBoxContainer").GetNode("HBoxContainer3").GetNode<Label>("CoinsValue");
		timerLabel = GetNode("VBoxContainer").GetNode("HBoxContainer2").GetNode<Label>("TimerValue");
		timer = GetNode<Timer>("Timer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		coinsLabel.Text = numberOfCoins.ToString();

		updateTimer();
	}

	private void updateTimer()
	{
		string minutesString = minutes < 10 ? "0" + minutes.ToString() : minutes.ToString();
		string secondsString = seconds < 10 ? "0" + seconds.ToString() : seconds.ToString();
		string miliSecondsString = miliSeconds < 10 ? "0" + miliSeconds.ToString() : miliSeconds.ToString();
		timerLabel.Text = minutesString + ":" + secondsString + ":" + miliSecondsString;
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
