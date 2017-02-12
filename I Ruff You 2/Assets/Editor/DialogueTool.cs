using UnityEngine;
using UnityEditor;
using System;   // serializable
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class DialogueTool : EditorWindow
{
    [Serializable]
    class WindowNode
    {
        private float xPos, yPos, width, height;    // have to store rect info as indiviual floats bc Rect is not serializable
        private List<WindowNode> mDestinations;
        public List<WindowNode> Destinations
        {
            get { return mDestinations; }
        }
        public DialogueNode DialogueNode;

        public WindowNode(Rect rect)
        {
            xPos = rect.x;
            yPos = rect.y;
            width = rect.width;
            height = rect.height;
            DialogueNode = new DialogueNode();
            mDestinations = new List<WindowNode>();
        }

        public void AddDestination(WindowNode newDest)
        {
            mDestinations.Add(newDest);
            DialogueNode.NextNodes.Add(newDest.DialogueNode);
            newDest.DialogueNode.ConversationID = DialogueNode.ConversationID;
        }

        public void RemoveDestination(WindowNode removeDest)
        {
            mDestinations.Remove(removeDest);
            DialogueNode.NextNodes.Remove(removeDest.DialogueNode);
        }

        public Rect GetRekt()
        {
            return new Rect(xPos, yPos, width, height);
        }

        public void SetRect(Rect rect)
        {
            xPos = rect.x;
            yPos = rect.y;
            width = rect.width;
            height = rect.height;
        }
    }

    // WINDOW FIELDS
    private Vector2 scrollPosition = Vector2.zero;
    private float y = 3000;
    private List<WindowNode> windows;
    private List<WindowNode> windowsToAttach;
    private List<WindowNode> windowsToDetach;
    private Texture2D text_arrow = null;

    // SAVING/ LOADING DATA FIELDS
    private BinaryFormatter mFormatter;
    private string windowDatafileName = "/dialoguewindows.bat";
    private string nodeDataFileName = "/dialoguenodes.bat";
    private List<DialogueNode> nodes;


    // -------------------WINDOW FUNCTIONS-------------------

    [MenuItem("Window/Dialogue Tool")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(DialogueTool));
    }

    void OnEnable()
    {
        windowsToAttach = new List<WindowNode>();
        windowsToDetach = new List<WindowNode>();

        text_arrow = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/arrow.png", typeof(Texture2D));
        Debug.Assert(text_arrow);
        
        maxSize = new Vector2(1300, 600);

        mFormatter = new BinaryFormatter();
        LoadNodes();
    }

    void OnGUI()
    {
        // attach
        if (windowsToAttach.Count == 2)
        {
            windowsToAttach[0].AddDestination(windowsToAttach[1]);
            windowsToAttach[1].DialogueNode.IsRoot = false;
            windowsToAttach.Clear();
        }
        
        // detach
        if(windowsToDetach.Count == 2)
        {
            windowsToDetach[0].RemoveDestination(windowsToDetach[1]);
            windowsToDetach.Clear();
        }

        // buttons
        // [BUTTON] Create
        if (GUILayout.Button("Create Node"))
        {
            WindowNode newWindow = new WindowNode(new Rect(scrollPosition.x, scrollPosition.y, 200, 420));
            windows.Add(newWindow);
            nodes.Add(newWindow.DialogueNode);
        }
        // [BUTTON] Save
        if (GUILayout.Button("Save"))
        {
            SaveNodes();
        }

        // windows
        scrollPosition = GUI.BeginScrollView(new Rect(0, 50, 1300, 550), scrollPosition, new Rect(0, 0, 1300, y));
        BeginWindows();
            // connections
            for (int i = 0; i < windows.Count; i++)
            {
                for (int j = 0; j < windows[i].Destinations.Count; j++)
                {
                    DrawConnection(windows[i].GetRekt(), windows[i].Destinations[j].GetRekt(), i);
                }
            }
            // [WINDOW] Windows
            for (int i = 0; i < windows.Count; i++)
            {
                windows[i].SetRect( GUI.Window(i, windows[i].GetRekt(), DrawNodeWindow, "Window " + i) );
            }
        EndWindows();
        GUI.EndScrollView();
        if(scrollPosition.y > y - 600)
            y += 100;
    }

    void DrawNodeWindow(int id)
    {
        DialogueNode node = windows[id].DialogueNode;

        // [BUTTON] Attach
        if (GUILayout.Button("Attach"))
        {
            windowsToAttach.Add(windows[id]);
        }

        // [BUTTON] Detach
        if (GUILayout.Button("Detach"))
        {
            windowsToDetach.Add(windows[id]);
        }

        // [TEXT AREA] Text
        EditorGUILayout.LabelField("Text");
        node.Text = EditorGUILayout.TextArea(node.Text);

        // [ENUM] Location
        node.mLocation = (DialogueNode.Location)EditorGUILayout.EnumPopup("Location", node.mLocation);

        // [INT FIELD] Conversation ID
        EditorGUILayout.LabelField("Conversation ID");
        node.ConversationID = EditorGUILayout.IntField(node.ConversationID);

        // [INT FIELD] Node ID
        EditorGUILayout.LabelField("Node ID");
        node.NodeID = EditorGUILayout.IntField(node.NodeID);

        // [TOGGLE] Is Valid
        EditorGUILayout.LabelField("Valid by Default");
        node.IsValid = EditorGUILayout.Toggle(node.IsValid);

        // [TOGGLE] Is MC
        EditorGUILayout.LabelField("Is MC Speaking");
        node.IsMC = EditorGUILayout.Toggle(node.IsMC);

        // [TOGGLE] Is Option
        EditorGUILayout.LabelField("Is Option");
        node.IsOption = EditorGUILayout.Toggle(node.IsOption);

        // [TOGGLE] Is RIGHT Option
        EditorGUILayout.LabelField("Is GOOD Option");
        node.GoodOption = EditorGUILayout.Toggle(node.GoodOption);
        
        // [BUTTON] Delete
        if (GUILayout.Button("Delete"))
        {
            RemoveWindow(id);
        }

        GUI.DragWindow();
    }

    private void RemoveWindow(int id)
    {
        for (int i = 0; i < windows.Count; i++)
        {
            windows[i].RemoveDestination(windows[id]);
        }
        windows.Remove(windows[id]);
    }

    void DrawConnection(Rect start, Rect end, int id)
    {
        Vector3 startPos = new Vector3(start.x + start.width / 2, start.y + start.height / 2, 0);
        Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
        Handles.DrawLine(startPos, endPos);
        GUI.DrawTexture(new Rect(endPos.x-10, endPos.y-5, 10, 10), text_arrow);
    }


    // -------------------SAVING/ LOADING DATA FUNCTIONS-------------------

    private void SaveNodes()
    {
        // Window nodes, for editor
        FileStream file = File.Open(Application.persistentDataPath + windowDatafileName, FileMode.OpenOrCreate);
        try
        { 
            mFormatter.Serialize(file, windows);
        } catch(SerializationException e)
        {
            Debug.LogError("Serialization failed: " + e);
        } finally
        {
            file.Close();
        }

        // Dialoge nodes, for editor
        FileStream nodeFile = File.Open(Application.persistentDataPath + nodeDataFileName, FileMode.OpenOrCreate);
        try
        {
            mFormatter.Serialize(nodeFile, nodes);
        }
        catch (SerializationException e)
        {
            Debug.LogError("Serialization failed: " + e);
        }
        finally
        {
            nodeFile.Close();
        }

        // Dialogue nodes, for game
        FileStream nodeFile2 = File.Open(Application.dataPath + "/Resources/dd.bytes", FileMode.OpenOrCreate);
        DialogueNodeList dnl = new DialogueNodeList(nodes);
        try
        {
            mFormatter.Serialize(nodeFile2, dnl);
        }
        catch (SerializationException e)
        {
            Debug.LogError("Serialization failed: " + e);
        }
        finally
        {
            nodeFile2.Close();
        }
    }

    private void LoadNodes()
    {
        FileStream file = File.Open(Application.persistentDataPath + windowDatafileName, FileMode.OpenOrCreate);
        Debug.Assert(file.CanRead);
        
        try
        {
            windows = (List<WindowNode>)mFormatter.Deserialize(file);
        } catch (SerializationException e)
        {
            Debug.LogError("De-serialization failed: " + e);
        } finally
        {
            file.Close();

            // initialize list of windows in case deserialization failed
            if (windows == null)
                windows = new List<WindowNode>();

            nodes = new List<DialogueNode>();
            foreach (WindowNode window in windows)
                nodes.Add(window.DialogueNode);
        }
        
    }

}