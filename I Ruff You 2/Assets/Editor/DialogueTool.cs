using UnityEngine;
using UnityEditor;

public class DialogueTool : EditorWindow
{
    string _titleString = "Dialogue Tool";
    
    [MenuItem("Window/Dialogue Tool")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(DialogueTool));
    }

    void OnGUI()
    {
        GUILayout.Label(_titleString, EditorStyles.boldLabel);
    }

}