using System;   // serializable
using System.Collections.Generic;

[Serializable]
public class DialogueNodeList{

    public List<DialogueNode> Nodes;

    public DialogueNodeList(List<DialogueNode> nodes)
    {
        Nodes = nodes;
    }

}
