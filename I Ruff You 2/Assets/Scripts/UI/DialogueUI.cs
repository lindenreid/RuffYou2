using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DialogueUI : UI {

    // UI references
    // note: NPC's text reference is UI.Text
    public GameObject NPCDialogue;
    public GameObject MCDialogue;
    public Image HeadshotImage;
    public Button Option1Button;
    public Button Option2Button;

    // Tuning
    public Sprite PlayerHeadshotSprite;

    // Internal
    private Dog mConvoDog;
    private List<DialogueNode> CurrentNodes;

    public void StartDialogue(Dog dog)
    {
        mConvoDog = dog;

        List<DialogueNode> firstNode = new List<DialogueNode>();
        int currentDay = DialogueController.GameController.CurrentDay;
        firstNode.Add(DialogueController.GetConversationRoot(mConvoDog.ConvoIds[currentDay]));
        DisplayDialogue( firstNode );
    }

    public void EndDialogue()
    {
        DialogueController.GameController.IncrementDay();
        Close();
    }

    // *** Use ONLY for handling dialogue option button when MC is speaking *** 
    public void ChooseDialogue(int choice)
    {
        if (CurrentNodes[choice].GoodOption)
        {
            mConvoDog.IncreaseLoveScore();
            // TODO: set headshot to happy headshot
        }
        else
        {
            mConvoDog.DecreaseLoveScore();
            // TODO: set headshot to disappointed headshot
        }

        if (!CurrentNodes[choice].IsLeaf())
        {
            DisplayDialogue(CurrentNodes[choice].NextNodes);
        } else
        {
            EndDialogue();
        }
    }

    // *** Use ONLY for handling "next" button when NPC is speaking *** 
    public void NextDialogue()
    {
        if (!CurrentNodes[0].IsLeaf())
        {
            DisplayDialogue(CurrentNodes[0].NextNodes);
        }
        else
        {
            EndDialogue();
        }
    }

    private void DisplayDialogue(List<DialogueNode> newNodes)
    {
        // TODO: sliding transition

        CurrentNodes = newNodes;

        if (newNodes[0].IsOption)   // IE, MC is speaking
        {
            HeadshotImage.sprite = PlayerHeadshotSprite;
            Option1Button.GetComponentInChildren<Text>().text = CurrentNodes[0].Text;
            Option2Button.GetComponentInChildren<Text>().text = CurrentNodes[1].Text;

            NPCDialogue.SetActive(false);
            MCDialogue.SetActive(true);
        }
        else   // IE, NPC is speaking
        {
            HeadshotImage.sprite = mConvoDog.HeadshotSprite;
            Text.text = CurrentNodes[0].Text;

            NPCDialogue.SetActive(true);
            MCDialogue.SetActive(false);
        }
    }

}
