using Godot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static Godot.GD;

[Tool]
public class GodotPathGenerator : EditorPlugin
{
    private const string PluginName = "GodotPathGenerator";
    private const string DirPath = "res://script";
    private const string FileName = "GPG.cs";
    private const string FilePath = DirPath + "/" + FileName;

    private const string ClassNameRegex = "(?<=[//|\\.])(@*[a-zA-Z0-9]+)(?=\\.cs$)";

    private EditorInterface _editor;

    public override void _EnterTree()
    {
        _editor = GetEditorInterface();
        EditorFileSystem fileSystem = _editor.GetResourceFilesystem();

        fileSystem.Connect("filesystem_changed", this, nameof(OnFilesystemChanged));

        Print($"{PluginName}: Start Plugin");
    }

    private void OnFilesystemChanged()
    {
        Node root = _editor.GetEditedSceneRoot();
        if (root != null)
        {
            var reference = root.GetScript();

            if (reference != null && reference is CSharpScript script)
            {
                var resourcePath = script.ResourcePath;
                var matchCollection = Regex.Matches(resourcePath, ClassNameRegex);

                if (matchCollection.Count > 0)
                {
                    var pathList = new List<string>();
                    TraversalChildren("", root, pathList);
                    var nameMatch = matchCollection[0];

                    StartGeneratePath(nameMatch.ToString(), pathList);
                }
            }
        }

        Print("OnFilesystemChanged");
    }

    private void TraversalChildren(string prefix, Node start, List<string> pathList)
    {
        var path = prefix + "/" + start.Name;
        pathList.Add(path);
        Print(path);

        var children = start.GetChildren();
        if (children != null && children.Count > 0)
        {
            foreach (var child in children)
            {
                if (child is Node node)
                {
                    TraversalChildren(path, node, pathList);
                }
            }
        }
    }

    private void StartGeneratePath(string className, List<string> pathList)
    {
        var scriptDir = new Directory();
        var file = new File();
        try
        {
            if (!scriptDir.DirExists(DirPath))
            {
                if (scriptDir.MakeDir(DirPath) != Error.Ok)
                {
                    PrintErr($"{PluginName}: can't create '{DirPath}' dir");
                    return;
                }
            }

            if (scriptDir.Open(DirPath) == Error.Ok)
            {
                bool isNew = false;
                if (!file.FileExists(FilePath))
                {
                    if (file.Open(FilePath, File.ModeFlags.Write) != Error.Ok)
                    {
                        PrintErr($"{PluginName}: can't create file '{FilePath}'");
                    }

                    file.StoreString("");
                    isNew = true;
                    file.Close();
                }

                if (isNew)
                {
                    if (file.Open(FilePath, File.ModeFlags.Write) != Error.Ok)
                    {
                        PrintErr($"{PluginName}: can't write file '{FilePath}'");
                        return;
                    }

                    file.StoreString("/// <summary>");
                    file.StoreString(System.Environment.NewLine);
                    file.StoreString("/// Don't modify this file, let plugin update it");
                    file.StoreString(System.Environment.NewLine);
                    file.StoreString("/// Created by GodotPathGenerator");
                    file.StoreString(System.Environment.NewLine);
                    file.StoreString("/// </summary>");
                    file.StoreString(System.Environment.NewLine);
                    file.StoreString("public static class GPG");
                    file.StoreString(System.Environment.NewLine);
                    file.StoreString("{");
                    file.StoreString(System.Environment.NewLine);

                    WriteOneClass(file, className, pathList);

                    file.StoreString("}");

                    file.Close();
                }
                else
                {
                    if (file.Open(FilePath, File.ModeFlags.Read) != Error.Ok)
                    {
                        PrintErr($"{PluginName}: can't read file '{FilePath}'");
                        return;
                    }

                    var text = file.GetAsText();
                    var lines = text.Split(System.Environment.NewLine);

                    int state = 0;

                    for (var i = 0; i < lines.Length; i++)
                    {
                        var line = lines[i];

                        if (IsBlank(line))
                        {
                            continue;
                        }

                        if (i <= 6)
                        {
                            state = 1;
                            continue;
                        }

                    }
                }
            }
            else
            {
                PrintErr($"{PluginName}: can't open '{DirPath}' dir");
            }
        }
        finally
        {
            if (file.IsOpen())
            {
                file.Close();
            }
        }
    }

    private bool IsBlank(string str)
    {
        return string.IsNullOrEmpty(str) || str.Trim().Length == 0;
    }

    private void WriteOneClass(File file, string className, List<string> pathList)
    {
        file.StoreString($"    public static class {className}Path");
        file.StoreString(System.Environment.NewLine);
        file.StoreString("    {");
        file.StoreString(System.Environment.NewLine);

        foreach (var path in pathList)
        {
            var fieldName = path.Replace("/", "_");

            file.StoreString($"        public const string {fieldName} = \"{path}\";");
            file.StoreString(System.Environment.NewLine);
        }

        file.StoreString(System.Environment.NewLine);
        file.StoreString("    }");
        file.StoreString(System.Environment.NewLine);
    }
}