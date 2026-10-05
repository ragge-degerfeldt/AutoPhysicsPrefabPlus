using Godot;
using System;
using System.IO;

[Tool]
public partial class PathValidation : Node
{
	public enum PathType
	{
		Input,
		Output
	}

    private PathType myType;

    [Export] private EditorFileDialog pickFolderDialog;
    [Export] private LineEdit folderLineEdit;
    [Export] private Label invalidPathLabel;
    [Export] private Button pickFolderButton;

    private AutoPhysicsPrefabPlusUI ui;

    private bool validPath = false;

	public void InjectDependencies(PathType type, AutoPhysicsPrefabPlusUI _ui)
	{
        ui = _ui;
        myType = type;
        OnInputFolderLineEditChanged("");
    }

    public override void _EnterTree()
    {
        invalidPathLabel.Visible = false;
        folderLineEdit.TextChanged += OnInputFolderLineEditChanged;
        pickFolderButton.ButtonDown += OnPickFolderButtonPressed;
        pickFolderDialog.DirSelected += OnDirectorySelected;
        folderLineEdit.GuiInput += OnLineEditInput;

        OnLineEditInput(null);
    }

    public override void _ExitTree()
    {
        folderLineEdit.TextChanged -= OnInputFolderLineEditChanged;
        pickFolderButton.ButtonDown -= OnPickFolderButtonPressed;
		pickFolderDialog.DirSelected -= OnDirectorySelected;
        folderLineEdit.GuiInput -= OnLineEditInput;
	}

	private void OnInputFolderLineEditChanged(string newText)
	{
        validPath = Directory.Exists(folderLineEdit.Text.Replace("res://",AutoPhysicsPrefabPlusUI.pathPrefix));
        invalidPathLabel.Visible = !validPath;
        if(validPath)
        {
            Debug.Print("Valid " + myType.ToString().ToLower() + " path selected");
            ui.LoadAll();
        }
    }

	private void OnPickFolderButtonPressed()
	{
        pickFolderDialog.Popup();
    }

	private void OnDirectorySelected(string dir)
	{
        folderLineEdit.Text = dir;
        OnInputFolderLineEditChanged("");
    }

    public bool HasValidPath() { return validPath; }

    public string Path()
    {
        return folderLineEdit.Text;
    }

    public void SetPath(string path)
    {
        folderLineEdit.Text = path;
    }

    private void OnLineEditInput(InputEvent @e)
    {
        folderLineEdit.CaretColumn = folderLineEdit.Text.Length;
    }
}
