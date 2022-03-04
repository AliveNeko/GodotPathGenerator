using Godot;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static Godot.GD;

[Tool]
public class GodotPathGenerator : EditorPlugin
{
    private const string PluginName = "GodotPathGenerator";

    private const string ClassNameRegex = "(?<=[//|\\.])(@*[a-zA-Z0-9]+)(?=\\.cs$)";
    
    private string _dirPath = "res://script/gpg";

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
        string fileName = $"{className}Path.cs";
        string filePath = _dirPath + "/" + fileName;

        var scriptDir = new Directory();
        var file = new File();
        try
        {
            if (!scriptDir.DirExists(_dirPath))
            {
                if (scriptDir.MakeDirRecursive(_dirPath) != Error.Ok)
                {
                    PrintErr($"{PluginName}: can't create '{_dirPath}' dir");
                    return;
                }
            }

            if (scriptDir.Open(_dirPath) == Error.Ok)
            {
                if (file.Open(filePath, File.ModeFlags.Write) != Error.Ok)
                {
                    PrintErr($"{PluginName}: can't create file '{filePath}'");
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
                file.StoreString("public static partial class GPG");
                file.StoreString(System.Environment.NewLine);
                file.StoreString("{");
                file.StoreString(System.Environment.NewLine);

                WriteOneClass(file, className, pathList);

                file.StoreString("}");
                file.Flush();

                file.Close();
            }
            else
            {
                PrintErr($"{PluginName}: can't open '{_dirPath}' dir");
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

    private void WriteOneClass(File file, string className, List<string> pathList)
    {
        file.StoreString($"    public static class {className}Path");
        file.StoreString(System.Environment.NewLine);
        file.StoreString("    {");
        file.StoreString(System.Environment.NewLine);

        foreach (var path in pathList)
        {
            var fieldName = path.Replace("/", "_");

            file.StoreString($"        public const string {fieldName} = \"/root{path}\";");
            file.StoreString(System.Environment.NewLine);
        }

        file.StoreString("    }");
        file.StoreString(System.Environment.NewLine);
    }
}