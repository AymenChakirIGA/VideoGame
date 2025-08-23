using Godot;
using System;

public partial class Level0 : Node2D
{
    private int coinCount = 0;
    private Label coinLabel;

    public override void _Ready()
    {
        coinLabel = GetNode<Label>("CoinLabel");
        coinLabel.Text = "Coins: 0";

        foreach (Node node in GetChildren())
        {
            if (node is Collectable collectable)
            {
                collectable.Collected += _on_collectable_collected;
            }
        }
    }

    private void _on_collectable_collected(int value)
    {
        coinCount += value;
        coinLabel.Text = "Coins: " + coinCount;
        GD.Print($"Coins: {coinCount}");
    }
}
