using Godot;
using System;
using System.Diagnostics;

public partial class Health : Node
{
	// Declare member variables here:
	public delegate void MaxHealthChanged(int OldHealth, int NewHealth);
	public delegate void HealthChanged(int OldHealth, int NewHealth);
	public delegate void HealthDepleted();

	[Export] public int maxHealth= 3;
	[Export] public bool immortal = false;
	int health;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		health = maxHealth;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	//setters and getters
	public int GetHealth()
	{
		return health;
	}
	public bool getImmortal()
	{
		return immortal;
	}

	public void SetHealth(int value)
	{
		health = value;
	}
	
	public void SetMaxHealth(int value)
	{
		maxHealth = value;
	}
	public void SetImmortal(bool value)
	{
		immortal = value;
	}

	//Helper functions

	public void ChangeHealth(int amount)
	{
		//add the amount to the health
		if (immortal)
		{
			//no neeed to change health if health is immortal
			return;
		}
		int oldHealth = health;
		health += amount;
		if (health > maxHealth)
		{
			health = maxHealth;
		}
		else if (health < 0)
		{
			health = 0;
		}
		if (oldHealth != health)
		{
			if (health == 0)
			{
				EmitSignal(nameof(HealthDepleted));
			}
			EmitSignal(nameof(HealthChanged), oldHealth, health);
		}
	}

	public void ImmortalityTimer(int time)
	{
		immortal = true;
		//Create a timer
		Timer timer = new Timer();
		timer.Name = "Timer";
		timer.WaitTime = time;
		timer.OneShot = true;
		Callable callable = new Callable(this, nameof(On_Timout_Immortality));
		timer.Connect("timeout", callable);
		AddChild(timer);
		timer.Start();
	}
	public void On_Timout_Immortality()
	{
		//Set back to initial state
		this.SetImmortal(false);
		//Free the timer
		Debug.WriteLine("Timer Ended");
		Timer timer = GetNode<Timer>("Timer");
		timer.QueueFree();
	}

}
