using UnityEngine;

public class Item : Interactable {

    public enum InteractableType
    {
        Tree,
        Ball,
        Food
    }

    public GameController GameController;

    // TUNING
    public string DialogueText;
    public InteractableType ItemType;

    public override void Interact()
    {
        GameController.DialogueController.DisplayItemUI(DialogueText);

        switch (ItemType)
        {
            case InteractableType.Ball: InteractBall(); break;
            case InteractableType.Food: InteractFood(); break;
            case InteractableType.Tree: InteractTree(); break;
        }
    }

    private void InteractTree()
    {
        Debug.Log("tree");
    }

    private void InteractBall()
    {
        Debug.Log("ball");
    }

    private void InteractFood()
    {
        Debug.Log("food");
    }

}
