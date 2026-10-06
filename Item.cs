using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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
		Capsule = 2,
        Cylinder = 3,
        Convex_Shape = 4
	}

    [Export] private Label assetName;
    [Export] private OptionButton bodyOption;
    [Export] private OptionButton shapeOption;
    [Export] private VBoxContainer rigidbodyContainer;
    [Export] private VBoxContainer characterbodyContainer;
    [Export] private VBoxContainer staticbodyContainer;
    [Export] private HFlowContainer layerSelectContainer;
    [Export] private Slider massSlider;
    [Export] private SpinBox massSpinbox;
    [Export] private CheckBox fragileCheckbox;
    [Export] private VBoxContainer boxContainer;
    [Export] private VBoxContainer sphereContainer;
    [Export] private VBoxContainer capsuleContainer;
    [Export] private VBoxContainer convexShapeContainer;
    [Export] private SpinBox boxWidthSpinbox;
	[Export] private SpinBox boxHeightSpinbox;
    [Export] private SpinBox boxDepthSpinbox;
	[Export] private SpinBox sphereRadiusSpinbox;
	[Export] private SpinBox capsuleRadiusSpinbox;
	[Export] private SpinBox capsuleHeightSpinbox;
    [Export] private EditorSpinSlider verticesSlider;
    [Export] private Button processButton;
    [Export] private VBoxContainer vertices;
    [Export] private PackedScene vertexPrefab;
    [Export] private PackedScene layerSelectPrefab;

    private Action action;

    public PackedScene asset;
    public string filename;
    public string extension;

    public Body myBody = Body.Rigidbody;
    public Shape myShape = Shape.Box;

    private VBoxContainer[] bodyGroups;
    private VBoxContainer[] shapeGroups;

    private bool changingMass = false;

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
            shapeOption.AddItem(shape.ToString().Replace('_',' '), (int)shape);
        }
        shapeGroups = new VBoxContainer[shapeOption.ItemCount];
        shapeGroups[(int)Shape.Box] = boxContainer;
        shapeGroups[(int)Shape.Sphere] = sphereContainer;
        shapeGroups[(int)Shape.Capsule] = capsuleContainer;
        shapeGroups[(int)Shape.Cylinder] = capsuleContainer;
        shapeGroups[(int)Shape.Convex_Shape] = convexShapeContainer;

        shapeOption.Selected = (int)myShape;
        OnShapeSelected((int)myShape);

        massSlider.Value = 1f;
        massSpinbox.Value = 1f;
        OnMassChangeEnded(true);
        boxWidthSpinbox.Value = 1f;
        boxHeightSpinbox.Value = 1f;
        boxDepthSpinbox.Value = 1f;
        sphereRadiusSpinbox.Value = 0.5f;
        capsuleRadiusSpinbox.Value = 0.5f;
        capsuleHeightSpinbox.Value = 2f;

        for (int i = 1; i <= 32; i++)
        {
            string str = ProjectSettings.GetSetting("layer_names/3d_physics/layer_" + i.ToString()).ToString();
            if(str != "")
            {
                LayerSelect layerSelect = layerSelectPrefab.Instantiate<LayerSelect>();
                layerSelect.Initialize(str, i);
                layerSelectContainer.AddChild(layerSelect);
            }
        }
        verticesSlider.ValueChanged += OnVertexAmountChanged;
        massSpinbox.ValueChanged += OnMassChanged;
        massSlider.DragStarted += OnMassChangeStarted;
        massSlider.DragEnded += OnMassChangeEnded;
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
        verticesSlider.ValueChanged -= OnVertexAmountChanged;
        massSpinbox.ValueChanged -= OnMassChanged;
        massSlider.DragStarted -= OnMassChangeStarted;
        massSlider.DragEnded -= OnMassChangeEnded;
        bodyOption.ItemSelected -= OnBodySelected;
        shapeOption.ItemSelected -= OnShapeSelected;
        processButton.ButtonDown -= ProcessItem;
    }

    public override void _Process(double delta)
    {
        if(!changingMass) { return; }
        massSpinbox.Value = massSlider.Value;
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
        vertices.Visible = shapeGroups[(int)Shape.Convex_Shape].Visible;
    }

    private void OnMassChangeStarted()
    {
        changingMass = true;
    }

    private void OnMassChangeEnded(bool b)
    {
        changingMass = false;
    }

    private void OnMassChanged(double d)
    {
        massSlider.Value = double.Min(massSpinbox.Value, massSlider.MaxValue);
    }

    private void OnVertexAmountChanged(double d)
    {
        int newVertexAmount = (int)d;

        while(vertices.GetChildCount() != newVertexAmount)
        {
            if(vertices.GetChildCount() < newVertexAmount)
            {
                Vertex vertex = vertexPrefab.Instantiate<Vertex>();
                vertices.AddChild(vertex);
            }
            else
            {
                vertices.RemoveChild(vertices.GetChildren()[vertices.GetChildCount() - 1]);
            }
        }
    }

    public float GetMass() { return (float)massSpinbox.Value; }
    public bool IsFragile() { return fragileCheckbox.ButtonPressed; }
    public float GetBoxWidth() { return (float)boxWidthSpinbox.Value; }
    public float GetBoxHeight() { return (float)boxHeightSpinbox.Value; }
    public float GetBoxDepth() { return (float)boxDepthSpinbox.Value; }
    public float GetSphereRadius() { return (float)sphereRadiusSpinbox.Value; }
    public float GetCapsuleRadius() { return (float)capsuleRadiusSpinbox.Value; }
    public float GetCapsuleHeight() { return (float)capsuleHeightSpinbox.Value; }
    public float GetCylinderRadius() { return GetCapsuleRadius(); }
    public float GetCylinderHeight() { return GetCapsuleHeight(); }
    public int[] GetLayers() 
    {
        List<int> layerList = new List<int>();
        foreach(Node node in layerSelectContainer.GetChildren())
        {
            LayerSelect select = (LayerSelect)node;
            int idx = select.CheckedState();
            if(idx != -1) { layerList.Add(idx); }
        }
        return layerList.ToArray();
    }

    public Vector3[] GetVertices()
    {
        Vector3[] arr = new Vector3[vertices.GetChildCount()];
        int idx = 0;
        foreach(Node child in vertices.GetChildren().ToArray())
        {
            arr[idx] = ((Vertex)child).GetVertex();
            idx++;
        }
        return arr;
    }
}
