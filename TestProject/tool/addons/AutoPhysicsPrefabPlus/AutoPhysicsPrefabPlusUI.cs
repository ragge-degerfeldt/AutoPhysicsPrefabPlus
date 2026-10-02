using Godot;
using System;

[Tool]
public partial class AutoPhysicsPrefabPlusUI : Control
{
    public AutoPhysicsPrefabPlus tool;
    public static string pathPrefix;

    [Export] private PathValidation inputPath;
    [Export] private PathValidation outputPath;

    [Export] private Button doitButton;

    public override void _EnterTree()
    {
        pathPrefix = ProjectSettings.GlobalizePath("res://");
        Debug.Print("Initialized project path: " + pathPrefix);

        doitButton.ButtonDown += DoIt;
    }

    public override void _ExitTree()
    {
        doitButton.ButtonDown -= DoIt;
    }

    public void Initialize(AutoPhysicsPrefabPlus _tool)
    {
        tool = _tool;
        inputPath.SetPath(tool.config.inputPath);
        outputPath.SetPath(tool.config.outputPath);
        inputPath.InjectDependencies(PathValidation.PathType.Input, this);
        outputPath.InjectDependencies(PathValidation.PathType.Output, this);
    }

    public void FetchConfig()
    {
        tool.config.inputPath = inputPath.HasValidPath() ? inputPath.Path() : tool.config.inputPath;
        tool.config.outputPath = outputPath.HasValidPath() ? outputPath.Path() : tool.config.outputPath;
    }

    private void DoIt()
    {
        FetchConfig();
        tool.ProcessAll();
    }

    public void LoadAll()
    {
        tool.LoadAll();
    }
}
