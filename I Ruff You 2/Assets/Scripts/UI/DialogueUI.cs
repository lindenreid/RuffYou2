using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DialogueUI : UI {

    // UI references
    // note: NPC's text reference is UI.Text
    public GameObject   NPCDialogue;
    public GameObject   MCDialogue;
    public Image        HeadshotImage;
    public Image        NametagImage;
    public Button       Option1Button;
    public Button       Option2Button;
    public Button       NextButton;

    // Tuning
    public Sprite   PlayerHeadshotSprite;
    public Sprite   PlayerNametagSprite;
    public float    EndDialogueLag;

    // Internal
    private Dog                 mConvoDog;
    private List<DialogueNode>  CurrentNodes;
    private bool                lag;
    private float               dialogueLagRemaining;

    private enum NPCMoodType
    {
        Neutral, Happy, Sad
    }

    void OnEnable()
    {
        lag = false;
        dialogueLagRemaining = EndDialogueLag;
    }

    void Update()
    {
        if (lag)
        {
            dialogueLagRemaining -= Time.deltaTime;
            if(dialogueLagRemaining <= 0)
            {
                lag = false;
                dialogueLagRemaining = EndDialogueLag;
                ActuallyEndDialogue();
            }
        }
    }

    public void StartDialogue(Dog dog)
    {
        mConvoDog = dog;

        List<DialogueNode> firstNode = new List<DialogueNode>();
        DialogueNode root = DialogueController.GetConversationRoot(mConvoDog.GetCurrentConvoID());
        if(root != null)
        {
            firstNode.Add(root);
            DisplayDialogue(firstNode, NPCMoodType.Neutral);
        }
    }

    public void StartIntroDialogue(int convoID)
    {
        List<DialogueNode> firstNode = new List<DialogueNode>();
        DialogueNode root = DialogueController.GetConversationRoot(convoID);
        if (root != null)
        {
            firstNode.Add(root);
            DisplayDialogue(firstNode, NPCMoodType.Neutral);
        }
    }
    
    // *** Use ONLY for handling dialogue option button when MC is speaking *** 
    public void ChooseDialogue(int choice)
    {
        bool choseGoodOption = CurrentNodes[choice].GoodOption;

        if (choseGoodOption)
        {
            mConvoDog.IncreaseLoveScore();
        }
        else
        {
            mConvoDog.DecreaseLoveScore();
        }

        if (!CurrentNodes[choice].IsLeaf())
        {
            DisplayDialogue(CurrentNodes[choice].NextNodes, choseGoodOption ? NPCMoodType.Happy : NPCMoodType.Sad);
        } else
        {
            LagEndDialogue();
        }
    }

    // *** Use ONLY for handling "next" button when NPC is speaking *** 
    public void NextDialogue()
    {
        if (!CurrentNodes[0].IsLeaf())
        {
            DisplayDialogue(CurrentNodes[0].NextNodes, NPCMoodType.Neutral);
        }
        else
        {
            LagEndDialogue();
        }
    }

    private void LagEndDialogue()
    {
        lag = true;
    }

    private void ActuallyEndDialogue()
    {
        if(CurrentNodes[0].ConversationID == 0) // intro dialogue
        {
            Close(enableMovement: true);
        }
        else if (DialogueController.GameController.CurrentDay + 1 == DialogueController.GameController.LastDay) // last day
        {
            DialogueController.GameController.EndGame(happyEnding: mConvoDog.LoveScore >= DialogueController.GameController.HappyEndingLoveLevel);
            Close(enableMovement: false);
        }
        else // normal convo
        {
            mConvoDog.IncrementConvoNumber(); // important: do this before incrementing the day!
            DialogueController.GameController.IncrementDay();
            Close(enableMovement: false);
        }
    }

    private void DisplayDialogue(List<DialogueNode> newNodes, NPCMoodType npcMood)
    {
        if(mConvoDog)
            mConvoDog.GameController.MenuController.MenuInput.ClearInput();

        CurrentNodes = newNodes;

        if (newNodes[0].IsOption) // is MC, and has choices
        {
            HeadshotImage.sprite = PlayerHeadshotSprite;
            NametagImage.sprite = PlayerNametagSprite;

            if(!CurrentNodes[0].IsValid && !CurrentNodes[1].IsValid)    // if they're both invalid, find "wrong" choice and set that button "active", but replace its text with alternative
            {
                if (!CurrentNodes[0].GoodOption)
                {
                    Option2Button.gameObject.SetActive(false);
                    Option1Button.GetComponentInChildren<Text>().text = CurrentNodes[2].Text;

                    Option1Button.gameObject.SetActive(true);
                    if (!ActiveButtons.Contains(Option2Button)) ActiveButtons.Add(Option1Button);
                }
                else if(!CurrentNodes[1].GoodOption)
                {
                    Option1Button.gameObject.SetActive(false);
                    ActiveButtons.Remove(Option1Button);

                    Option2Button.GetComponentInChildren<Text>().text = CurrentNodes[2].Text;

                    Option2Button.gameObject.SetActive(true);
                    if (!ActiveButtons.Contains(Option2Button)) ActiveButtons.Add(Option2Button);
                }
            }
            else    // activate all valid options
            {
                if (CurrentNodes[0].IsValid)
                {
                    Option1Button.GetComponentInChildren<Text>().text = CurrentNodes[0].Text;
                    Option1Button.gameObject.SetActive(true);

                    if (!ActiveButtons.Contains(Option1Button)) ActiveButtons.Add(Option1Button);
                }
                else
                {
                    Option1Button.gameObject.SetActive(false);
                    ActiveButtons.Remove(Option1Button);
                }

                if (CurrentNodes[1].IsValid)
                {
                    Option2Button.GetComponentInChildren<Text>().text = CurrentNodes[1].Text;
                    Option2Button.gameObject.SetActive(true);
                    if (!ActiveButtons.Contains(Option2Button)) ActiveButtons.Add(Option2Button);
                }
                else
                {
                    Option2Button.gameObject.SetActive(false);
                    ActiveButtons.Remove(Option2Button);
                }
            }

            if (ActiveButtons.Contains(NextButton)) ActiveButtons.Remove(NextButton);
            NPCDialogue.SetActive(false);
            MCDialogue.SetActive(true);
        }
        else if(newNodes[0].IsMC) // is MC, but has no choices
        {
            HeadshotImage.sprite = PlayerHeadshotSprite;
            NametagImage.sprite = PlayerNametagSprite;

            Text.text = CurrentNodes[0].Text;

            NPCDialogue.SetActive(true);
            MCDialogue.SetActive(false);

            ActiveButtons.Remove(Option1Button);
            ActiveButtons.Remove(Option2Button);
            if(!ActiveButtons.Contains(NextButton)) ActiveButtons.Add(NextButton);
        }
        else // is NPC
        {
            if(npcMood == NPCMoodType.Neutral)
                HeadshotImage.sprite = mConvoDog.HeadshotSpritePlain;
            else if(npcMood == NPCMoodType.Happy)
                HeadshotImage.sprite = mConvoDog.HeadshotSpriteHappy;
            else if (npcMood == NPCMoodType.Sad)
                HeadshotImage.sprite = mConvoDog.HeadshotSpriteSad;

            NametagImage.sprite = mConvoDog.NametagSprite;

            Text.text = CurrentNodes[0].Text;

            NPCDialogue.SetActive(true);
            MCDialogue.SetActive(false);

            ActiveButtons.Remove(Option1Button);
            ActiveButtons.Remove(Option2Button);
            if (!ActiveButtons.Contains(NextButton)) ActiveButtons.Add(NextButton);
        }
    }

}
