using Godot;
using System;

[Tool]
public partial class Item : Node
{
	public enum Body : int
	{
		Rigidbody = 0,
		Characterbody = 1,
		Staticbody = 2
	}

	public enum Shape : int
	{
		Box = 0,
		Sphere = 1,
		Capsule = 2
	}


    [Export] private EditorResourcePicker assetView;
    [Export] private Label assetName;
    [Export] private OptionButton bodyOption;
    [Export] private OptionButton shapeOption;
    [Export] private VBoxContainer rigidbody;
    [Export] private VBoxContainer characterbody;
    [Export] private VBoxContainer staticbody;
    [Export] private Slider massSlider;
    [Export] private Label massLabel;
    [Export] private VBoxContainer box;
    [Export] private VBoxContainer sphere;
    [Export] private VBoxContainer capsule;
    [Export] private SpinBox x;
	[Export] private SpinBox y;
    [Export] private SpinBox z;
	[Export] private SpinBox radius;
	[Export] private SpinBox _base;
	[Export] private SpinBox height;

    public Body myBody = Body.Rigidbody;
    public Shape myShape = Shape.Box;

    private VBoxContainer[] bodyGroups;
    private VBoxContainer[] shapeGroups;

    public override void _EnterTree()
    {
        bodyOption.Clear();
		foreach(Body body in Enum.GetValues(typeof(Body)))
		{
            bodyOption.AddItem(body.ToString(), (int)body);
        }
        bodyGroups = new VBoxContainer[bodyOption.ItemCount];
        bodyGroups[(int)Body.Rigidbody] = rigidbody;
        bodyGroups[(int)Body.Characterbody] = characterbody;
        bodyGroups[(int)Body.Staticbody] = staticbody;

		bodyOption.Selected = (int)myBody;
        OnBodySelected((int)myBody);

        shapeOption.Clear();
		foreach(Shape shape in Enum.GetValues(typeof(Shape)))
		{
            shapeOption.AddItem(shape.ToString(), (int)shape);
        }
        shapeGroups = new VBoxContainer[shapeOption.ItemCount];
        shapeGroups[(int)Shape.Box] = box;
        shapeGroups[(int)Shape.Sphere] = sphere;
        shapeGroups[(int)Shape.Capsule] = capsule;

        shapeOption.Selected = (int)myShape;
        OnShapeSelected((int)myShape);


        bodyOption.ItemSelected += OnBodySelected;
        shapeOption.ItemSelected += OnShapeSelected;
    }

    public override void _ExitTree()
    {
        bodyOption.ItemSelected -= OnBodySelected;
        shapeOption.ItemSelected -= OnShapeSelected;
    }

	private void OnBodySelected(long index)
	{
        myBody = (Body)bodyOption.Selected;
        for (int i = 0; i < bodyGroups.Length; i++) 
		{
            bodyGroups[i].Visible = false;
        }
        bodyGroups[(int)myBody].Visible = true;
    }

	private void OnShapeSelected(long index)
	{
        myShape = (Shape)shapeOption.Selected;
        for (int i = 0; i < shapeGroups.Length; i++)
		{
            shapeGroups[i].Visible = false;
        }
        shapeGroups[(int)myShape].Visible = true;
    }
}
