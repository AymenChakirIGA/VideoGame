using Godot;
using System;

public partial class StateSpikes : Node2D
{
    private bool isSpikeActive = false;
    private Area2D damageArea;
    private CollisionShape2D damageAreaCollision;
    private AnimatedSprite2D animatedSprite;
    private Timer timer;
    [Export] double timerWaitTime = 1;


    public override void _Ready()
    {
        damageArea = GetNode<Area2D>("Damage Area");
        damageAreaCollision = damageArea.GetNode<CollisionShape2D>("CollisionShape2D");
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        timer = GetNode<Timer>("Timer");
        timer.WaitTime = timerWaitTime;
    }

    private void SpikeActivate()
    {
        isSpikeActive = true;
        damageAreaCollision.Disabled = false;
        animatedSprite.PlayBackwards("Open_Close");
    }

    private void SpikeDisactivate()
    {
        isSpikeActive = false;
        damageAreaCollision.Disabled = true;
        animatedSprite.Play("Open_Close");
    }

    //Signals
    private void OnTimeout()
    {
        if (isSpikeActive) SpikeDisactivate();
        else SpikeActivate();
    }
}
