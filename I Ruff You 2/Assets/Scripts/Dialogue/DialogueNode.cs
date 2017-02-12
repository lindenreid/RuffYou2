using System;   // serializable
using System.Collections.Generic;

[Serializable]
public class DialogueNode {

    // WARNING: do not change the order of these!!!
    public enum Location
    {
        Ball = 0,
        Food = 1,
        Pool = 2,
        Tree = 3,
        Bushes = 4
    }

    public List<DialogueNode> NextNodes;
    public bool     IsValid;
    public int      ConversationID;
    public int      NodeID;

    public string   Text;
    public bool     IsRoot;
    public bool     IsMC;
    public bool     IsOption;
    public bool     GoodOption;
    public Location mLocation;

    public DialogueNode()
    {
        NextNodes = new List<DialogueNode>();
        IsValid = true;
        ConversationID = -1;
        Text = "<uninitiated>";
        IsRoot = true;
        IsOption = false;
        GoodOption = false;
        mLocation = Location.Bushes;
    }

    public DialogueNode(bool valid, int conversationID, string text, List<DialogueNode> nextNodes)
    {
        NextNodes = nextNodes;
        IsValid = valid;
        ConversationID = conversationID;
        Text = text;
    }

    public bool IsLeaf()
    {
        return NextNodes.Count == 0;
    }

}
