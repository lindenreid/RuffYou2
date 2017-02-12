using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DayUI : MonoBehaviour {

    public GameController GameController;

    // UI References
    public GameObject   Day_Large;
    public Image        Image_Day_Small;

    // Tuning
    public List<Sprite>     DaySprites;
    public Vector2          OriginalSize;
    public Vector2          OriginalPos;
    public RectTransform    DayTransitionTargetTransform;
    public float            DayTransitionLifetime;
    public float            DayTransitionKeyframe;
    public bool             PlayingAnimation;

    // Internal
    private bool    playDayTransition;
    private bool    dayChangeJustFinished;
    private float   timeLeft;
    private Vector2 sizeDelta;
    private Vector2 posDelta;

    // This code is BAD and I HATE IT
    // Maybe learn how to write my OWN smooth damp function with BLACKJACK and HOOKERS later?
    // ... And actually have original size and pos to be a reference to the large day image's rect transform
    // (for some reason those were reading as 0,0 before?? kill me please)
    void Update()
    {
        if (playDayTransition) // play animation
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0)
            {
                dayChangeJustFinished = true;
                playDayTransition = false;
            }
            else if (timeLeft < DayTransitionKeyframe)
            {
                Day_Large.GetComponent<RectTransform>().sizeDelta = Vector2.SmoothDamp(Day_Large.GetComponent<RectTransform>().sizeDelta, DayTransitionTargetTransform.sizeDelta, ref sizeDelta, 0.6f);
                Day_Large.GetComponent<RectTransform>().anchoredPosition = Vector2.SmoothDamp(Day_Large.GetComponent<RectTransform>().anchoredPosition, DayTransitionTargetTransform.anchoredPosition, ref posDelta, 0.6f);
            }
        }
        else if(dayChangeJustFinished) // finish animation
        {
            PlayingAnimation = false;
            Image_Day_Small.sprite = DaySprites[GameController.CurrentDay];
            Image_Day_Small.enabled = true;
            Day_Large.SetActive(false);

            dayChangeJustFinished = false;

            if (GameController.CurrentDay == 0)
                GameController.DialogueController.DisplayIntroDialogue();
            else
                GameController.MenuController.EnableMovement(true);
        }
    }

    public void ChangeDay()
    {
        PlayingAnimation = true;
        GameController.SwitchControl(enableCharacterMovement: false);

        Image_Day_Small.enabled = false;

        Day_Large.GetComponent<Image>().sprite = DaySprites[GameController.CurrentDay];
        Day_Large.SetActive(true);
        Day_Large.GetComponent<RectTransform>().sizeDelta           = OriginalSize;
        Day_Large.GetComponent<RectTransform>().anchoredPosition    = OriginalPos;

        timeLeft = DayTransitionLifetime;

        playDayTransition = true;
        dayChangeJustFinished = false;
    }
    
}
