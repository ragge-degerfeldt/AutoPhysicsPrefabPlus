using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[Tool]
public partial class AutoPhysicsPrefabPlus : EditorPlugin
{
    private EditorDock dock;
    private AutoPhysicsPrefabPlusUI dockUI;

    public Config config;

    public List<Item> loadedItems = new List<Item>();

    public override void _EnterTree()
    {
        LoadConfiguration();
        dock = new EditorDock();
        dock.Title = "Auto Physics Prefab Plus";
        dock.DefaultSlot = EditorDock.DockSlot.RightUl;
        dockUI = GD.Load<PackedScene>("res://addons/AutoPhysicsPrefabPlus/ui.tscn").Instantiate<AutoPhysicsPrefabPlusUI>();
        dockUI.Initialize(this);
        dock.AddChild(dockUI);
        AddDock(dock);
        Debug.Print("Enabled");
    }

    public override void _ExitTree()
    {
        SaveConfiguration();
        RemoveDock(dock);
        dock.QueueFree();
		Debug.Print("Disabled");
    }

    public void SaveConfiguration()
    {
        dockUI.FetchConfig();
        try
        {
            FileAccess file = FileAccess.Open("res://addons/AutoPhysicsPrefabPlus/config.txt", FileAccess.ModeFlags.Write);
            file.StoreString(Config.Save(config));
            file.Close();
        }
        catch(Exception e)
        {

        }
    }

    public void LoadConfiguration()
    {
        try
        {
            FileAccess file = FileAccess.Open("res://addons/AutoPhysicsPrefabPlus/config.txt", FileAccess.ModeFlags.Read);
            config = Config.Load(file.GetAsText());
            file.Close();
        }
        catch(Exception e)
        {
            Debug.Print("Error loading config file, restoring default values, " + e.Message + e.StackTrace);
            config = Config.Default();
        }
    }

    public void LoadAll()
    {
        Debug.Print("Clearing loaded items");
        loadedItems.Clear();
        Debug.Print("Loading all GLTF files in " + ProcessPath(config.inputPath));
        foreach(string filename in DirAccess.GetFilesAt(ProcessPath(config.inputPath)))
        {
            if(filename.GetExtension() != "glb" && filename.GetExtension() != "gltf")
            {
                continue;
            }
            Load(filename.Replace("."+filename.GetExtension(),""),"." + filename.GetExtension());
        }
    }

    private void Load(string filename, string extension)
    {
        Debug.Print("Loading " + filename + extension);
        PackedScene asset = GD.Load<PackedScene>(ProcessPath(config.inputPath) + filename + extension);
        Item item = dockUI.CreateItem(asset,filename,extension);
        loadedItems.Add(item);
    }

    public void ProcessAll()
    {
        Debug.Print("Processing all GLTF files in " + ProcessPath(config.inputPath));
        for (int i = 0; i < loadedItems.Count; i++)
        {
            dockUI.processAllItem.SetAsset(loadedItems[i].asset,loadedItems[i].filename,loadedItems[i].extension);
            ProcessGLTF(dockUI.processAllItem);
        }
        dockUI.processAllItem.SetAsset(null,"","");
    }

    public void ProcessGLTF(Item item)
    {
        Debug.Print("Processing " + item.filename + item.extension);

        Node3D instance = (Node3D)((item.asset).Instantiate());
        RigidBody3D rb = new RigidBody3D();
        instance.AddChild(rb);
        rb.Owner = instance;
        PackedScene export = new PackedScene();
        export.Pack(instance);
        ResourceSaver.Save(export, ProcessPath(config.outputPath) + item.filename + ".tscn");
        instance.QueueFree();
    }

    private string ProcessPath(string _str)
    {
        string str = _str;
        if(!str.EndsWith('/')) { str += "/"; }
        return str;
    }
}