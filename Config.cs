using Godot;
using System;

[Tool]
public class Config
{
    public string inputPath = "";
    public string outputPath = "";

    public static Config Default()
	{
        Config config = new Config();
		config.inputPath = "res://addons/AutoPhysicsPrefabPlus/DefaultInput";
		config.outputPath = "res://addons/AutoPhysicsPrefabPlus/DefaultOutput";
        return config;
    }

	public static Config Load(string text)
	{
		try
		{
			string[] substrings = text.Split('\n');
			Config config = new Config();
            config.inputPath = substrings[0];
            config.outputPath = substrings[1];
            return config;
		}
		catch(Exception e)
		{
            throw e;
		}
    }

	public static string Save(Config config)
	{
        return
        config.inputPath + '\n' +
        config.outputPath + '\n';
    }
}
