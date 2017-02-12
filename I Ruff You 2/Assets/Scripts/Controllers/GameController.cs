using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

    // ------------ PUBLIC FIELDS ------------

    // TUNING
    public int          LastDay = 5;
    public int          HappyEndingLoveLevel = 3;
    public float        MinInteractionDistance = 2.0f;
    public Transform    PlayerStartLocation;

    // AUDIO
    public AudioSource  AudioSource;
    public AudioClip    GameMusic;
    public AudioClip    HappyEndingMusic;
    public AudioClip    SadEndingMusic;

    // ACTORS
    public List<Interactable>   Actors;
    public Transform            Player;
    public CharacterMovement   CharacterMovement;
    
    // DIALOGUE
    public DialogueController DialogueController;

    // UI
    public MenuController MenuController;
    [SerializeField]
    private MenuInput MenuInput;

    // GAMEPLAY
    private int     mCurrentDay;
    public  int     CurrentDay
    {
        get { return mCurrentDay; }
    }

    [HideInInspector]
    public bool HoldingBall;

    [HideInInspector]
    public bool CompletedAnyRomance;

    // ------------ PUBLIC FUNCTIONS ------------

    // MONOBEHAVIOR
    void Awake()
    {
        AudioSource.clip = GameMusic;
        AudioSource.Play();

        mCurrentDay = 0;
        CompletedAnyRomance = false;

        Player.transform.position = PlayerStartLocation.position;
    }

    void OnEnable()
    {
        MenuController.ChangeDay();
        foreach (Interactable actor in Actors)
            actor.ChangeDay();
    }

    // INPUT CONTROL
    public void SwitchControl(bool enableCharacterMovement)
    {
        CharacterMovement.enabled = enableCharacterMovement;

        if (!MenuController.DayUI.PlayingAnimation)
            MenuInput.enabled = !enableCharacterMovement;
        else
            MenuInput.enabled = false;
    }

    public void Interact()
    {
        Interactable closestActor = Actors[0];
        float closestDistance = Vector3.Distance(Actors[0].GetComponent<Transform>().position, Player.position);
        for (int i = 0; i < Actors.Count; i++)
        {
            float dist = Vector3.Distance(Actors[i].GetComponent<Transform>().position, Player.position);
            if (dist < closestDistance && !(HoldingBall && Actors[i] is Item))
            {
                closestDistance = dist;
                closestActor = Actors[i];
            }
        }

        if (closestDistance <= MinInteractionDistance)
            closestActor.Interact();
    }

    // being lazy and not creating an event for this,
    // even though that pattern may make more sense,
    // since literally the only thing that cares about
    // the character flipping right now is the ball
    public void CharacterFlip()
    {
        foreach(Interactable actor in Actors)
        {
            if(actor is Item)
            {
                Item item = (Item)actor;
                if (item.ItemType == Item.InteractableType.Ball && item.Interacted)
                    item.SwitchBallPosition();
            }
        }
    }

    // GAMEPLAY
    public void IncrementDay()
    {
        mCurrentDay++;
        
        MenuController.ChangeDay();
        Player.transform.position = PlayerStartLocation.position;
        foreach (Interactable actor in Actors)
            actor.ChangeDay();
    }

    public void EndGame(bool happyEnding)
    {
        if (happyEnding)
        {
            AudioSource.clip = HappyEndingMusic;
            AudioSource.Play();
            CompletedAnyRomance = true;
        }
        else
        {
            AudioSource.clip = SadEndingMusic;
            AudioSource.Play();
            CompletedAnyRomance = false;
        }
        MenuController.EndGame(happyEnding);
    }

    public void Restart()
    {
        AudioSource.clip = GameMusic;
        AudioSource.Play();

        mCurrentDay = 0;

        MenuController.ChangeDay();
        foreach (Interactable actor in Actors)
        {
            actor.Reset();
        }

        Player.transform.position = PlayerStartLocation.position;
    }

}
