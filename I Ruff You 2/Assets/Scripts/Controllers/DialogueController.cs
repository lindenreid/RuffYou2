using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization; // serialization exception
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class DialogueController : MonoBehaviour {

    // ------------ PUBLIC FIELDS ------------

    // UI
    public UI           ItemUI;
    public DialogueUI   DogDialogueUI;

    // INTERNAL
    public int                  IntroConvoID;
    public List<Transform>      DogLocations;
    public List<DialogueNode>   DialogueNodes;

    // EXTERNAL
    public GameController GameController;
    public MenuController MenuController;

    // ------------ PUBLIC FUNCTIONS ------------

    // Monobehavior

    void Awake()
    {
        TextAsset data = Resources.Load<TextAsset>("dd");

        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream(data.bytes);
        
        try
        { 
            DialogueNodeList nodeListObj = formatter.Deserialize(stream) as DialogueNodeList;
            DialogueNodes = nodeListObj.Nodes;
        }
        catch (SerializationException e)
        {
            Debug.LogError("De-serialization failed: " + e);
        }
    }

    // Dialogue UI

    public void DisplayIntroDialogue()
    {
        DogDialogueUI.StartIntroDialogue(IntroConvoID);
        DogDialogueUI.gameObject.SetActive(true);
        MenuController.EnableMovement(false);
        MenuController.ActiveUI = DogDialogueUI;
    }

    public void DisplayItemUI(string text)
    {
        ItemUI.Text.text = text;
        ItemUI.gameObject.SetActive(true);
        MenuController.EnableMovement(false);
        MenuController.ActiveUI = ItemUI;
    }

    public void DisplayDialogueUI(Dog dog)
    {
        DogDialogueUI.StartDialogue(dog);
        DogDialogueUI.gameObject.SetActive(true);
        MenuController.EnableMovement(false);
        MenuController.ActiveUI = DogDialogueUI;
    }

    public DialogueNode GetConversationRoot(int convoID)
    {
        foreach(DialogueNode node in DialogueNodes)
        {
            if (node.IsRoot && node.ConversationID == convoID)
                return node;
        }
        Debug.LogError("DialogueController.DialogueNode() : Conversation root not found!");
        return null;
    }

    public Vector3 GetDogLocation(int conversationID)
    {
        int i = (int)GetConversationRoot(conversationID).mLocation;

        Vector3 pos = DogLocations[i].position;
        Vector2 random2d = Random.insideUnitCircle * 5.0f;
        pos += new Vector3(random2d.x, 0, random2d.y);
        
        return pos;
    }

    public void EnableChoices(bool enable, List<int> convoIDs)
    {
        foreach(DialogueNode node in DialogueNodes)
        {
            if (node.IsOption && convoIDs.Contains(node.NodeID))
                node.IsValid = enable;
        }
    }

}
