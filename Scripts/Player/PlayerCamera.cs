using Godot;
using Godot.Collections;
using System;

public partial class PlayerCamera : Camera2D
{
    //Variables
    private Vector2 zoom;
    private Vector2 targetZoom = new Vector2(1f, 1f); // Target zoom level
    private PlayerController player;
    private Vector2 cameraPosition;
    [Export] public float zoomSpeed = 0.05f; // Speed of zooming in and out
    private Array<Node> cameraStartLimits;
    private Array<Node> cameraEndLimits;
    private int cameraLimitIndex = 0;

    public override void _Ready()
    {
        // Initialize zoom level
        zoom = new Vector2(1f, 1f);
        this.Zoom = zoom;

        //init variables
        player = GetParent().GetNode<PlayerController>("Player");
        this.Position = new Vector2(player.Position.X, player.Position.Y);
        cameraPosition = this.Position;
        cameraStartLimits = GetParent().GetNode("Camera Sections").GetNode("Start").GetChildren();
        cameraEndLimits = GetParent().GetNode("Camera Sections").GetNode("End").GetChildren();

        //init Limits
        Marker2D markerLeft = cameraStartLimits[0] as Marker2D;
        LimitLeft = (int)markerLeft.Position.X;

        Marker2D markerRight = cameraEndLimits[cameraLimitIndex] as Marker2D;
        LimitRight = (int)markerRight.Position.X;
    }

    public override void _Process(double delta)
    {
        OnZoom((float)delta);
        // Update the camera's zoom
        this.Zoom = zoom;

        CameraFollowPlayer();

        //Adjust Camera Limits
        Marker2D markerLeft = cameraStartLimits[cameraLimitIndex] as Marker2D;
        LimitLeft = (int)Mathf.Lerp(LimitLeft, markerLeft.Position.X, 0.5);

        Marker2D markerRight = cameraEndLimits[cameraLimitIndex] as Marker2D;
        LimitRight = (int)Mathf.Lerp(LimitRight, markerRight.Position.X, 0.5);

    }

    private void CameraFollowPlayer()
    {
        //Follow player mouvements
        cameraPosition.X = Mathf.Lerp(cameraPosition.X, player.Position.X, 0.2f);
        cameraPosition.Y = Mathf.Lerp(cameraPosition.Y, player.Position.Y, 0.2f);
        this.Position = cameraPosition;
    }


    //Public functions
    public void OnZoom(float delta = 0)
    {
        if (zoom < targetZoom)
        {
            // Zoom in
            if (zoom + new Vector2(zoomSpeed * delta, zoomSpeed * delta) > targetZoom) zoom = targetZoom; // If zoom exceeds target, set it to target
            else zoom += new Vector2(zoomSpeed * delta, zoomSpeed * delta);
        }
        else if (zoom > targetZoom)
        {
            // Zoom out
            if (zoom - new Vector2(zoomSpeed * delta, zoomSpeed * delta) < targetZoom) zoom = targetZoom; // If zoom goes below target, set it to target
            else zoom -= new Vector2(zoomSpeed * delta, zoomSpeed * delta);
        }
    }
    public void SetTargetZoom(Vector2 newTargetZoom)
    {
        targetZoom = newTargetZoom;
    }

    public void SetZoomSpeed(float newZoomSpeed)
    {
        zoomSpeed = newZoomSpeed;
    }

    //Signals
    private void OnEnterLimitSwitchArea(Node2D body)
    {
        if (body.IsInGroup("Player"))
        {
            Marker2D markerLeft = cameraStartLimits[cameraLimitIndex] as Marker2D;
            Marker2D markerRight = cameraEndLimits[cameraLimitIndex] as Marker2D;
            if (body.Position.X <= markerRight.Position.X + 50 && body.Position.X >= markerRight.Position.X - 50) cameraLimitIndex++; //50 margin of error
            else if (body.Position.X <= markerLeft.Position.X + 50 && body.Position.X >= markerLeft.Position.X - 50) cameraLimitIndex--; //50 margin of error
        }
    }
}
