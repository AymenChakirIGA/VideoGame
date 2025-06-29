using Godot;
using System;

public partial class Goal : Node2D
{
    private AnimatedSprite2D animatedSprite;
    private bool isGoalReached = false;
    private PlayerCamera playerCamera;
    private ClearScreen clearScreen;
    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animatedSprite.Play("Idle");
        playerCamera = GetParent().GetNode("Player").GetNode<PlayerCamera>("Camera2D");
        clearScreen = GetParent().GetNode("Clear Screen") as ClearScreen;
    }

    //Signals
    private void goalReached(Node2D body)
    {
        // Check if the body that reached the goal is the player
        if (!isGoalReached && body.IsInGroup("Player"))
        {
            // Play the goal reached animation
            isGoalReached = true;
            playerCamera.SetZoomSpeed(0.7f); // Set zoom speed for the camera
            playerCamera.SetTargetZoom(new Vector2(2f, 2f)); // Zoom in when goal is reached
            GD.Print("Goal Reached!");
            clearScreen.Clear(); // Clear the screen

        }
    }
}
