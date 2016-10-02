using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization; // serialization exception
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class DialogueController : MonoBehaviour {

    // ------------ PUBLIC FIELDS ------------

    // UI
    public UI ItemUI;
    public DialogueUI DogDialogueUI;

    // INTERNAL
    public List<DialogueNode> DialogueNodes;

    // EXTERNAL
    public GameController GameController;

    // ------------ PUBLIC FUNCTIONS ------------

    void Awake()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file;
        if(Application.isWebPlayer)
            file = File.Open(Application.absoluteURL + "dialoguenodes.bat", FileMode.OpenOrCreate);
        else
            file = File.Open(Application.persistentDataPath + "dialoguenodes.bat", FileMode.OpenOrCreate);

        try
        {
            DialogueNodes = (List<DialogueNode>)formatter.Deserialize(file);
        }
        catch (SerializationException e)
        {
            Debug.LogError("De-serialization failed: " + e);
        }
        finally
        {
            file.Close();

            // initialize list of nodes in case deserialization failed
            if (DialogueNodes == null)
            {
                DialogueNodes = new List<DialogueNode>();
                Debug.LogError("DialogeNodes empty in DialogueController.");
            }
        }
    }

    public void DisplayItemUI(string text)
    {
        ItemUI.Text.text = text;
        ItemUI.gameObject.SetActive(true);
        EnableMovement(false);
    }

    public void DisplayDialogueUI(Dog dog)
    {
        DogDialogueUI.StartDialogue(dog);
        DogDialogueUI.gameObject.SetActive(true);
        EnableMovement(false);
    }

    public void EnableMovement(bool enable)
    {
        if (enable)
            GameController.CharacterMovement.enabled = true;
        else
            GameController.CharacterMovement.enabled = false;
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

}
