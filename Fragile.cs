using Godot;
using System;

public partial class Fragile : RigidBody3D
{
    public override void _EnterTree()
    {
        ContactMonitor = true;
        MaxContactsReported = 1;
        BodyEntered += OnBodyEntered;
    }

    public override void _ExitTree()
    {
        BodyEntered -= OnBodyEntered;
    }

	private void OnBodyEntered(Node body)
	{
        GD.Print(Name + " broke!!");
        QueueFree();
    }
}
