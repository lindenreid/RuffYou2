using UnityEngine;
using System.Collections.Generic;
using System;

public class Item : Interactable {

    public enum InteractableType
    {
        Tree,
        Ball,
        Food,
        Bush
    }

    public GameController GameController;

    // TUNING
    public string           DialogueText;
    public string           MultipleInteractionsText;
    public InteractableType ItemType;
    public List<int>        DialogueNodeActivationIds;

    // ball
    public Vector3  BallPositionUpLeft;
    public Vector3  BallPositionUpRight;
    public Vector3  BallPositionDown;

    // pee
    public  GameObject          PeePuddlePrefab;
    private List<GameObject>    mPeePuddles;
    public  float               PeeRadius;
    public  Vector2             PeeCenterOffset;

    // bowl
    public Sprite   EmptyBowlSprite;
    public Sprite   FullBowlSprite;

    // INTERNAL
    public bool Interacted
    {
        get { return mInteracted; }
    }
    private bool mInteracted = false;


    // ---- PUBLIC FUNCTIONS ----

    public void Awake()
    {
        mPeePuddles = new List<GameObject>();
    }

    public override void Interact()
    {
        if(!mInteracted)
            GameController.DialogueController.DisplayItemUI(DialogueText);
        else
            GameController.DialogueController.DisplayItemUI(MultipleInteractionsText);

        switch (ItemType)
        {
            case InteractableType.Ball: InteractBall(); break;
            case InteractableType.Food: InteractFood(); break;
            case InteractableType.Tree: InteractTree(); break;
            case InteractableType.Bush: InteractBush(); break;
        }
    }

    public override void ChangeDay()
    {
        Reset();
    }

    public override void Reset()
    {
        if (mInteracted)
        {
            switch (ItemType) // undo actions
            {
                case InteractableType.Ball:
                    InteractBall();
                    break;
                case InteractableType.Food:
                    GetComponent<SpriteRenderer>().sprite = FullBowlSprite;
                    break;
                case InteractableType.Tree:
                    ClearPee();
                    break;
                case InteractableType.Bush:
                    ClearPee();
                    break;
            }

            mInteracted = false;
            GameController.DialogueController.EnableChoices(false, DialogueNodeActivationIds);
        }
    }

    public void SwitchBallPosition()
    {
        if (GameController.CharacterMovement.FacingRight)
            transform.localPosition = BallPositionUpRight;
        else
            transform.localPosition = BallPositionUpLeft;
    }


    // ---- PRIVATE FUNCTIONS ----

    private void ClearPee()
    {
        foreach (GameObject pee in mPeePuddles)
        {
            Destroy(pee);
        }
        mPeePuddles.Clear();
    }

    private void Pee()
    {
        GameObject puddle = Instantiate(PeePuddlePrefab) as GameObject;
        mPeePuddles.Add(puddle);
        float w = Mathf.Sqrt(UnityEngine.Random.Range(0f, 1f));
        float t = UnityEngine.Random.Range(0f, 1f);
        float x0 = PeeRadius * w * Mathf.Cos(Mathf.PI * t);
        float y0 = PeeRadius * w * Mathf.Sin(Mathf.PI * t);
        puddle.transform.position = new Vector3(transform.position.x + x0 + PeeCenterOffset.x, transform.position.y + y0 + PeeCenterOffset.y, 0f);
    }

    private void InteractBush()
    {
        if(!mInteracted)
            GameController.DialogueController.EnableChoices(true, DialogueNodeActivationIds);

        Pee();
        mInteracted = true;
    }

    private void InteractTree()
    {
        if(!mInteracted)
            GameController.DialogueController.EnableChoices(true, DialogueNodeActivationIds);

        Pee();
        mInteracted = true;
    }

    private void InteractBall()
    {
        if (!mInteracted) // ball on ground; pick it up
        {
            GetComponent<SpriteRenderer>().sortingOrder += 2;
            transform.SetParent(GameController.Player, true);

            if(GameController.CharacterMovement.FacingRight)
                transform.localPosition = BallPositionUpRight;
            else
                transform.localPosition = BallPositionUpLeft;

            GameController.DialogueController.EnableChoices(true, DialogueNodeActivationIds);
        }
        else // player has ball; put it down
        {
            GetComponent<SpriteRenderer>().sortingOrder -= 2;
            transform.localPosition = BallPositionDown;
            transform.SetParent(null, true);

            GameController.DialogueController.EnableChoices(false, DialogueNodeActivationIds);
        }
        mInteracted = !mInteracted;
        GameController.HoldingBall = mInteracted;
    }

    private void InteractFood()
    {
        if (!mInteracted)
            GameController.DialogueController.EnableChoices(true, DialogueNodeActivationIds);

        GetComponent<SpriteRenderer>().sprite = EmptyBowlSprite;
        mInteracted = true;
    }

}
