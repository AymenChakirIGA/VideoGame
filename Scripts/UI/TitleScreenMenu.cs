using Godot;
using System;

public partial class TitleScreenMenu : CanvasLayer
{
    private Control cursor;
    private Vector2 cursorTargetPosition;
    private Vector2 cursorStartPosition;
    private float yOffset = 40;
    private int cursorIndex = 0;
    [Export] private int totalMenuItems = 2;

    public override void _Ready()
    {
        cursor = GetNode<Control>("Cursor");
        cursorStartPosition = cursor.Position;
        cursorTargetPosition = cursorStartPosition;
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
    }

    private void IncreaseCursorIndex()
    {
        if (cursorIndex < totalMenuItems-1)
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
            cursorIndex = totalMenuItems-1;
        }
    }

}
