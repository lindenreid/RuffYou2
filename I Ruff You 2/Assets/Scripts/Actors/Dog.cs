using UnityEngine;
using System.Collections.Generic;

public class Dog : Interactable {

    public enum DogType
    {
        Corgi,
        Chihuahua,
        Terrier,
        Squirrel
    }

    public GameController GameController;

    // Tuning
    public DogType      DogName;
    public Sprite       HeadshotSpritePlain;
    public Sprite       HeadshotSpriteHappy;
    public Sprite       HeadshotSpriteSad;
    public Sprite       NametagSprite;
    public List<int>    ConvoIds;
    public GameObject   EmotePrefabHappy;
    public GameObject   EmotePrefabSad;
    public Vector2      EmoteOffset;
    public float        EmoteLifetime = 5.0f;
    public float        EmoteRetainCharacterTime = 2.5f;
    public int          UnavailableConvoID;

    // Internal
    private int mLoveScore;
    public  int LoveScore
    {
        get { return mLoveScore; }
    }
    private int mConvoNumber;

    private GameObject      mEmote;
    private float           mEmoteRemainingLife;
    private float           ySpeed = 6f;
    private float           aSpeed = 3f;
    private SpriteRenderer  mSpriteRenderer;
    
    // ---- PUBLIC FUNCTIONS ----
	
    void Update()
    {
        if(mEmote != null)
        {
            mEmoteRemainingLife -= Time.deltaTime;
            if(mEmoteRemainingLife <= 0)
            {
                Destroy(mEmote);
                return;
            }

            if (mEmote.transform.localPosition.y < EmoteOffset.y) // animate
            {
                mEmote.transform.Translate(new Vector3(0.0f, ySpeed * Time.deltaTime, 0.0f));
                mSpriteRenderer.color = new Color(mSpriteRenderer.color.r, mSpriteRenderer.color.g, mSpriteRenderer.color.g, mSpriteRenderer.color.a + (aSpeed * Time.deltaTime));
            }
        }
    }

    // Interactable
    public override void Interact()
    {
        GameController.DialogueController.DisplayDialogueUI(this);
    }

    public override void ChangeDay()
    {
        if (DogName != DogType.Squirrel)
            transform.position = GameController.DialogueController.GetDogLocation(GetCurrentConvoID());
    }

    public override void Reset()
    {
        mLoveScore = 0;
        mConvoNumber = 0;
        transform.position = GameController.DialogueController.GetDogLocation(ConvoIds[0]);
    }

    // Misc
    public int GetCurrentConvoID()
    {
        if (DogName == DogType.Squirrel && !GameController.CompletedAnyRomance)
            return UnavailableConvoID;
        return ConvoIds[mConvoNumber];
    }

    public void IncrementConvoNumber()
    {
        mConvoNumber++;
    }

    public void IncreaseLoveScore()
    {
        mLoveScore++;
        CreateEmote(happy: true);
    }

    public void DecreaseLoveScore()
    {
        mLoveScore--;
        CreateEmote(happy: false);
    }


    // ---- PRIVATE FUNCTIONS ----

    private void CreateEmote(bool happy)
    {
        if (mEmote)
            Destroy(mEmote);

        if(happy)
            mEmote = Instantiate(EmotePrefabHappy) as GameObject;
        else
            mEmote = Instantiate(EmotePrefabSad) as GameObject;
        mEmote.transform.parent = transform;

        mEmote.transform.localPosition = new Vector3(EmoteOffset.x, 0.0f, 0);
        mSpriteRenderer = mEmote.GetComponent<SpriteRenderer>();
        mSpriteRenderer.color = new Color(mSpriteRenderer.color.r, mSpriteRenderer.color.g, mSpriteRenderer.color.g, 0.0f);

        mEmoteRemainingLife = EmoteLifetime;
    }


}
