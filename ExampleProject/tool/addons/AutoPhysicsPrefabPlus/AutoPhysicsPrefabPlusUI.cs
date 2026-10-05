using Godot;
using System;

[Tool]
public partial class AutoPhysicsPrefabPlusUI : Control
{
    public AutoPhysicsPrefabPlus tool;
    public static string pathPrefix;

    [Export] private PathValidation inputPath;
    [Export] private PathValidation outputPath;

    [Export] private VBoxContainer processAllContainer;

    [Export] private PackedScene itemPrefab;

    [Export] private VBoxContainer itemContainer;

    public Item processAllItem;

    public override void _EnterTree()
    {
        pathPrefix = ProjectSettings.GlobalizePath("res://");
        Debug.Print("Initialized project path: " + pathPrefix);
    }

    public void Initialize(AutoPhysicsPrefabPlus _tool)
    {
        processAllItem = itemPrefab.Instantiate<Item>();
        processAllItem.SetAction(() => ProcessAll(processAllItem));
        processAllContainer.AddChild(processAllItem);
        processAllContainer.MoveChild(processAllItem,0);
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

    private void ProcessAll(Item item)
    {
        FetchConfig();
        tool.ProcessAll();
    }

    public void LoadAll()
    {
        foreach(Node node in itemContainer.GetChildren())
        {
            node.QueueFree();
        }
        FetchConfig();
        tool.LoadAll();

        processAllItem.Visible = tool.loadedItems.Count > 0;
        processAllItem.SetAsset(null,tool.loadedItems.Count.ToString() + " files","");
    }

    public Item CreateItem(PackedScene asset, string filename, string extension)
    {
        Item item = itemPrefab.Instantiate<Item>();
        item.SetAsset(asset,filename,extension);
        item.SetAction(() => tool.ProcessGLTF(item));
        itemContainer.AddChild(item);
        return item;
    }
}
