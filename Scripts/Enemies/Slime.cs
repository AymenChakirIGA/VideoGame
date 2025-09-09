using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class Slime : Enemy
{
    // Components

    // Movement
    [Export] public float leftEdge = 5f;
    [Export] public float rightEdge = 5f;
    [Export] public float speed = 1f;
    private bool isFacingRight = true;
    private float alphaValue = 0f; // Alpha value of the Health Bar 
    private float deltaValue = 0f;

    // Melee hit flag
    private bool hitByMelee = false;

    public override void _Ready()
    {
        base.Initialize();
    }

    public override void _Process(double delta)
    {
        base.Process();
    }

    public override void _PhysicsProcess(double delta)
    {

        //Gravity
        if (!this.IsOnFloor())
        {
            Velocity += GetGravity() * (float)delta;
        }

        //Patrolling
        if (currentState is EnemyStates.Patrol)
        {
            this.Patrol();
            //Switch State
            if (isEnemyAlerted())
            {
                currentState = EnemyStates.Alert;
                GD.Print("Enemy Alerted!");
            }
        }
        else if (currentState is EnemyStates.Alert)
        {
            this.Alert();
            //Switch State
            if (!isEnemyAlerted())
            {
                currentState = EnemyStates.Patrol;
            }
        }
        else if (currentState == EnemyStates.Dead)
        {
            //Enemy Dead
            this.Dead();  
        }
        else if (currentState == EnemyStates.TakenDamage)
        {
            //Enemy Took Damage
            this.TakenDamage();
        }
        MoveAndSlide();
    }

}
