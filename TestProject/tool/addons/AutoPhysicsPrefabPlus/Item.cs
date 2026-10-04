using Godot;
using System;

[Tool]
public partial class Item : Control
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

    [Export] private Label assetName;
    [Export] private OptionButton bodyOption;
    [Export] private OptionButton shapeOption;
    [Export] private VBoxContainer rigidbodyContainer;
    [Export] private VBoxContainer characterbodyContainer;
    [Export] private VBoxContainer staticbodyContainer;
    [Export] private Slider massSlider;
    [Export] private Label massLabel;
    [Export] private VBoxContainer boxContainer;
    [Export] private VBoxContainer sphereContainer;
    [Export] private VBoxContainer capsuleContainer;
    [Export] private SpinBox boxWidthSpinbox;
	[Export] private SpinBox boxHeightSpinbox;
    [Export] private SpinBox boxDepthSpinbox;
	[Export] private SpinBox sphereRadiusSpinbox;
	[Export] private SpinBox capsuleBaseSpinbox;
	[Export] private SpinBox capsuleHeightSpinbox;
    [Export] private Button processButton;

    private Action action;

    public PackedScene asset;
    public string filename;
    public string extension;

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
        bodyGroups[(int)Body.Rigidbody] = rigidbodyContainer;
        bodyGroups[(int)Body.Characterbody] = characterbodyContainer;
        bodyGroups[(int)Body.Staticbody] = staticbodyContainer;

		bodyOption.Selected = (int)myBody;
        OnBodySelected((int)myBody);

        shapeOption.Clear();
		foreach(Shape shape in Enum.GetValues(typeof(Shape)))
		{
            shapeOption.AddItem(shape.ToString(), (int)shape);
        }
        shapeGroups = new VBoxContainer[shapeOption.ItemCount];
        shapeGroups[(int)Shape.Box] = boxContainer;
        shapeGroups[(int)Shape.Sphere] = sphereContainer;
        shapeGroups[(int)Shape.Capsule] = capsuleContainer;

        shapeOption.Selected = (int)myShape;
        OnShapeSelected((int)myShape);

        massSlider.Value = 1f;
        OnMassChanged(true);
        boxWidthSpinbox.Value = 1f;
        boxHeightSpinbox.Value = 1f;
        boxDepthSpinbox.Value = 1f;
        sphereRadiusSpinbox.Value = 1f;
        capsuleBaseSpinbox.Value = 1f;
        capsuleHeightSpinbox.Value = 1f;

        massSlider.DragEnded += OnMassChanged;
        bodyOption.ItemSelected += OnBodySelected;
        shapeOption.ItemSelected += OnShapeSelected;
        processButton.ButtonDown += ProcessItem;
    }

    public void SetAction(Action _action)
    {
        action = _action;
    }

    public void SetAsset(PackedScene _asset, string _filename, string _extension)
    {
        asset = _asset;
        filename = _filename;
        extension = _extension;
        assetName.Text = "File: " + filename + extension;
    }

    public override void _ExitTree()
    {
        massSlider.DragEnded -= OnMassChanged;
        bodyOption.ItemSelected -= OnBodySelected;
        shapeOption.ItemSelected -= OnShapeSelected;
        processButton.ButtonDown -= ProcessItem;
    }

    private void ProcessItem()
    {
        action();
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

    private void OnMassChanged(bool b)
    {
        massLabel.Text = "Mass: " + massSlider.Value + "kg";
    }
}
