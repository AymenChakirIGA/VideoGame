using Godot;
using System;
using System.Threading.Tasks;

public partial class DamageArea : Area2D
{
    [Export] private int damage = 0;
    private async void OnCollisionWithBody2D(Node2D body)
    {
        if (body.IsInGroup("Player"))
        {
            PlayerController player = body as PlayerController;
            await player.TakeDamage(damage);
        }
    }
}
