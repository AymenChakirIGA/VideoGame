using Godot;
using System;

public partial class PlayerCamera : Camera2D
{
    //Variables
    private Vector2 zoom;
    private Vector2 targetZoom = new Vector2(1f, 1f); // Target zoom level
    [Export] public float zoomSpeed = 0.05f; // Speed of zooming in and out 

    public override void _Ready()
    {
        // Initialize zoom level
        zoom = new Vector2(1f, 1f);
        this.Zoom = zoom;
    }

    public override void _Process(double delta)
    {
        OnZoom((float)delta);
        // Update the camera's zoom
        this.Zoom = zoom;

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
            if (zoom - new Vector2(zoomSpeed * delta, zoomSpeed * delta ) < targetZoom) zoom = targetZoom; // If zoom goes below target, set it to target
            else zoom -= new Vector2(zoomSpeed * delta, zoomSpeed * delta);
        }
    }
    public void SetTargetZoom(Vector2 newTargetZoom)
    {
        targetZoom = newTargetZoom;
    }
}
