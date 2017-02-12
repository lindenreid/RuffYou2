using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UI : MonoBehaviour {

    // World references
    public MenuController MenuController;
    public DialogueController DialogueController;

    // UI
    public Text Text;
    public List<Button> ActiveButtons;

    void OnEnable()
    {
        MenuController.ActiveUI = this;
    }

    public void Close(bool enableMovement = true)
    {
        MenuController.ActiveUI = null;
        gameObject.SetActive(false);
        MenuController.EnableMovement(enableMovement);
    }
}
