using UnityEngine;
using System.Collections.Generic;

public class Dog : Interactable {

    public GameController GameController;

    // Tuning
    public Sprite HeadshotSprite;
    public List<int> ConvoIds;

    // Internal
    private int mLoveScore;
    public int LoveScore
    {
        get { return mLoveScore; }
    }
	
    public override void Interact()
    {
        GameController.DialogueController.DisplayDialogueUI(this);
    }

    public void IncreaseLoveScore()
    {
        mLoveScore++;
        // TODO: show happy emoticon
    }

    public void DecreaseLoveScore()
    {
        mLoveScore--;
        // TODO: show sad emoticon
    }


}
