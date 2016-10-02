using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {

    public DialogueController DialogueController;
    public Text Text;

    public void Close()
    {
        gameObject.SetActive(false);
        DialogueController.EnableMovement(true);
    }

}
