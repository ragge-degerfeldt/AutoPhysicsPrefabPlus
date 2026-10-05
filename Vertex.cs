using Godot;
using System;

[Tool]
public partial class Vertex : Node
{
    [Export] private SpinBox xSpinbox;
	[Export] private SpinBox ySpinbox;
    [Export] private SpinBox zSpinbox;

	public Vector3 GetVertex()
	{
        return new Vector3((float)xSpinbox.Value, (float)ySpinbox.Value, (float)zSpinbox.Value);
    }
}
