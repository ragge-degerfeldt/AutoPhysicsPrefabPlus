using Godot;
using System;

[Tool]
public partial class LayerSelect : Node
{
    [Export] private CheckBox checkbox;

    int idx = -1;

    public void Initialize(string str, int i)
	{
        checkbox.Text = str;
        idx = i;
    }

	public int CheckedState()
	{
        return checkbox.ButtonPressed ? idx : -1;
    }
}
