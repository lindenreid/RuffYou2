using UnityEngine;
using UnityEngine.EventSystems;

public class MenuInput : MonoBehaviour {

    // World references
    public MenuController MenuController;

    // Tuning
    public KeyCode PressButton          = KeyCode.Space;
    public KeyCode MoveSelectionDown1   = KeyCode.DownArrow;
    public KeyCode MoveSelectionDown2   = KeyCode.W;
    public KeyCode MoveSelectionUp1     = KeyCode.UpArrow;
    public KeyCode MoveSelectionUp2     = KeyCode.S;

    // Internal
    private int     currentButton;
    private int     lastButton;
    private bool    selectedAny;
    
    void Awake()
    {
        ClearInput();
    }

    public void ClearInput()
    {
        currentButton = 0;
        lastButton = 0;
        selectedAny = false;
    }

    void Update()
    {
        var pointer = new PointerEventData(EventSystem.current);
        if (Input.GetKeyUp(MoveSelectionDown1) || Input.GetKeyUp(MoveSelectionDown2))    // Move Up
        {
            lastButton = currentButton;
            if (!selectedAny)
            {
                currentButton = 0;
            }
            else
            {
                currentButton--;
                if (currentButton < 0)
                    currentButton = MenuController.ActiveUI.ActiveButtons.Count - 1;
            }

            ExecuteEvents.Execute(MenuController.ActiveUI.ActiveButtons[currentButton].gameObject, pointer, ExecuteEvents.pointerEnterHandler);
            if(currentButton != lastButton)
                ExecuteEvents.Execute(MenuController.ActiveUI.ActiveButtons[lastButton].gameObject, pointer, ExecuteEvents.pointerExitHandler);

            selectedAny = true;
        }
        else if(Input.GetKeyUp(MoveSelectionUp1) || Input.GetKeyUp(MoveSelectionUp2))   // Move Down
        {
            lastButton = currentButton;
            if (!selectedAny)
            {
                currentButton = MenuController.ActiveUI.ActiveButtons.Count - 1;
            }
            else
            {
                currentButton++;
                if (currentButton >= MenuController.ActiveUI.ActiveButtons.Count)
                    currentButton = 0;
            }

            ExecuteEvents.Execute(MenuController.ActiveUI.ActiveButtons[currentButton].gameObject, pointer, ExecuteEvents.pointerEnterHandler);
            if (currentButton != lastButton)
                ExecuteEvents.Execute(MenuController.ActiveUI.ActiveButtons[lastButton].gameObject, pointer, ExecuteEvents.pointerExitHandler);

            selectedAny = true;
        }
        else if (Input.GetKeyUp(PressButton))   // Select
        {
            ExecuteEvents.Execute(MenuController.ActiveUI.ActiveButtons[currentButton].gameObject, pointer, ExecuteEvents.submitHandler);
        }
    }

    void OnEnable()
    {
        ClearInput();
    }

    void OnDisable()
    {
        ClearInput();
    }

}
