using System;   // serializable
using System.Collections.Generic;

[Serializable]
public class DialogueNode {

    public List<DialogueNode> NextNodes;
    public bool IsValid;
    public int ConversationID;
    public string Text;
    public bool IsRoot;
    public bool IsOption;
    public bool GoodOption;

    public DialogueNode()
    {
        NextNodes = new List<DialogueNode>();
        IsValid = true;
        ConversationID = -1;
        Text = "<uninitiated>";
        IsRoot = true;
        IsOption = false;
        GoodOption = false;
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
