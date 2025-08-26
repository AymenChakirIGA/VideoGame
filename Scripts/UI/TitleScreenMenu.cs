using Godot;
using System;

public partial class TitleScreenMenu : CanvasLayer
{
    private Control cursor;
    private Vector2 cursorTargetPosition;
    private Vector2 cursorStartPosition;
    private float yOffset = 40;
    private int cursorIndex = 0;
    private string level0ScenePath = "res://Scenes/Levels/Level_0/Level 0.tscn";
    [Export] private int totalMenuItems = 2;

    public override void _Ready()
    {
        cursor = GetNode<Control>("Cursor");
        cursorStartPosition = cursor.Position;
        cursorTargetPosition = cursorStartPosition;
        PackedScene level0Scene = (PackedScene)GD.Load(level0ScenePath);
    }

    public override void _Process(double delta)
    {
        AdjustCursorPosition();

        ManageInputs();

    }

    private void AdjustCursorPosition()
    {
        // Used Lerp for smooth transition
        float targetDirection = cursorStartPosition.Y + (yOffset * cursorIndex);
        cursorTargetPosition.Y = (float)Mathf.Lerp(cursorTargetPosition.Y, targetDirection, 0.3);
        cursor.Position = cursorTargetPosition;
    }

    private void ManageInputs()
    {
        if (Input.IsActionJustPressed("ui_down"))
        {
            IncreaseCursorIndex();
        }

        if (Input.IsActionJustPressed("ui_up"))
        {
            DecreaseCursorIndex();
        }

        if (Input.IsActionJustPressed("ui_accept"))
        {
            ExecuteMenuAction();
        }
    }

    private void IncreaseCursorIndex()
    {
        if (cursorIndex < totalMenuItems - 1)
        {
            cursorIndex++;
        }
        else
        {
            cursorIndex = 0;
        }
    }

    private void DecreaseCursorIndex()
    {
        if (cursorIndex > 0)
        {
            cursorIndex--;
        }
        else
        {
            cursorIndex = totalMenuItems - 1;
        }
    }

    private void ExecuteMenuAction()
    {
        switch (cursorIndex)
        {
            case 0:
                GetTree().ChangeSceneToFile(level0ScenePath);
                GD.Print("Start Game selected");
                break;
            case 1:
                GD.Print("Quit selected");
                break;
            default:
                GD.Print("Unknown menu item selected");
                break;
        }
    }

}
