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


        Node3D mesh = (Node3D)((item.asset).Instantiate());

        Node3D instance = mesh;

        CollisionShape3D collider = new CollisionShape3D();
        //collider.GlobalPosition = instance.GlobalPosition;

        switch(item.myBody)
        {
            case Item.Body.Rigidbody:
                instance = new RigidBody3D();
                instance.AddChild(mesh);
                mesh.Owner = instance;
                instance.AddChild(collider);
                //rb.GlobalPosition = instance.GlobalPosition;
                ((RigidBody3D)instance).Mass = item.GetMass();
                if(item.IsFragile())
                {
                    instance.SetScript(GD.Load<Script>("res://addons/AutoPhysicsPrefabPlus/Fragile.cs"));
                }
                collider.Owner = instance;
                break;
            case Item.Body.Characterbody:
                instance = new CharacterBody3D();
                instance.AddChild(mesh);
                mesh.Owner = instance;
                instance.AddChild(collider);
                collider.Owner = instance;
                //cb.GlobalPosition = instance.GlobalPosition;
                break;
            case Item.Body.Staticbody:
                instance = new StaticBody3D();
                instance.AddChild(mesh);
                mesh.Owner = instance;
                instance.AddChild(collider);
                collider.Owner = instance;
                //sb.GlobalPosition = instance.GlobalPosition;
                break;
        }

        ((PhysicsBody3D)instance).CollisionLayer = (uint)item.GetLayer();

        switch(item.myShape)
        {
            case Item.Shape.Box:
                BoxShape3D box = new BoxShape3D();
                box.Size = new Vector3(item.GetBoxWidth(), item.GetBoxHeight(), item.GetBoxDepth());
                collider.Shape = box;
                break;
            case Item.Shape.Sphere:
                SphereShape3D sphere = new SphereShape3D();
                sphere.Radius = item.GetSphereRadius();
                collider.Shape = sphere;
                break;
            case Item.Shape.Capsule:
                CapsuleShape3D capsule = new CapsuleShape3D();
                capsule.Radius = item.GetCapsuleRadius();
                capsule.Height = item.GetCapsuleHeight();
                collider.Shape = capsule;
                break;
            case Item.Shape.Cylinder:
                CylinderShape3D cylinder = new CylinderShape3D();
                cylinder.Radius = item.GetCylinderRadius();
                cylinder.Height = item.GetCylinderHeight();
                collider.Shape = cylinder;
                break;
            case Item.Shape.Convex_Shape:
                ConvexPolygonShape3D convexShape = new ConvexPolygonShape3D();
                convexShape.Points = item.GetVertices();
                collider.Shape = convexShape;
                break;
        }

        instance.Name = item.filename;
        PackedScene export = new PackedScene();
        export.Pack(instance);

        string outputFilename = item.filename + ".tscn";

        if(DirAccess.GetFilesAt(ProcessPath(config.outputPath)).Contains(outputFilename))
        {
            DirAccess.RemoveAbsolute(ProcessPath(config.outputPath) + outputFilename);
        }

        ResourceSaver.Save(export, ProcessPath(config.outputPath) + outputFilename);
        instance.QueueFree();
    }

    private string ProcessPath(string _str)
    {
        string str = _str;
        if(!str.EndsWith('/')) { str += "/"; }
        return str;
    }
}