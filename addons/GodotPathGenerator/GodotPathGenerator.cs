using Godot;
using System;
using System.Collections.Generic;
using static Godot.GD;

[Tool]
public class GodotPathGenerator : EditorPlugin
{
    private EditorInterface _editor;
    
    public override void _EnterTree()
    {
        _editor = GetEditorInterface();
        var fileSystem = _editor.GetResourceFilesystem();

        fileSystem.Connect("filesystem_changed", this, nameof(OnFilesystemChanged));

        Print("------------------------------------");
    }

    private void OnFilesystemChanged()
    {
        Node root = _editor.GetEditedSceneRoot();
        if (root != null)
        {
            var pathList = new List<string>();
            
        }
        
        Print("OnFilesystemChanged");
    }

    private void TraversalChildren(Node root, Node start)
    {
        Print(start.GetPathTo(root));

        var children = start.GetChildren();
        if (children != null && children.Count > 0)
        {
            foreach (var child in children)
            {
                if (child is Node node)
                {
                    TraversalChildren(root, node);
                }
            }
        }
    }
}
