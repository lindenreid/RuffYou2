using UnityEngine;
using System.Collections.Generic;

public class MenuController : MonoBehaviour {

    // Game references
    public GameController   GameController;

    // UI References
    public DayUI            DayUI;
    public GameObject       StartUI;
    public GameObject       EndUI_Happy;
    public GameObject       EndUI_Sad;
    public List<GameObject> Credits;
    public UI               ActiveUI;
    public MenuInput        MenuInput;

    // Internal
    private int currentCredits;

    void Awake()
    {
        currentCredits = -1;
    }

    public void EnableMovement(bool enable)
    {
        GameController.SwitchControl(enableCharacterMovement: enable);
    }

    public void StartGame()
    {
        StartUI.SetActive(false);
    }

    public void EndGame(bool happyEnding)
    {
        if (happyEnding)
        {
            EndUI_Happy.SetActive(true);
            ActiveUI = EndUI_Happy.GetComponent<UI>();
        }
        else {
            EndUI_Sad.SetActive(true);
            ActiveUI = EndUI_Sad.GetComponent<UI>();
        }
    }

    public void ChangeDay()
    {
        DayUI.ChangeDay();
    }

    public void NextCreditScreen()
    {
        currentCredits++;

        if(currentCredits > 0)
            Credits[currentCredits - 1].SetActive(false);
        if (currentCredits < Credits.Count)
        {
            Credits[currentCredits].SetActive(true);
            ActiveUI = Credits[currentCredits].GetComponent<UI>();
        }

        if(currentCredits == 0)
        {
            EndUI_Happy.SetActive(false);
            EndUI_Sad.SetActive(false);
        }
    }

    public void Restart()
    {
        EndUI_Happy.SetActive(false);
        EndUI_Sad.SetActive(false);
        Credits[Credits.Count - 1].SetActive(false);
        GameController.Restart();
    }
	
}
