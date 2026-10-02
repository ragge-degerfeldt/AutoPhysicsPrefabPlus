using Godot;
using System;


[Tool]
public partial class Debug
{
    private static string debugPrefix = "Auto Physics Prefab Plus -- ";

	public static void Print(string msg)
	{
        GD.Print(debugPrefix + msg);
    }
}
