using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

    // ------------ PUBLIC FIELDS ------------

    // TUNING
    public float MinInteractionDistance = 2.0f;

    // ACTORS
    public List<Interactable> Actors;
    public Transform Player;
    public CharacterMovement CharacterMovement;

    // DIALOGUE
    public DialogueController DialogueController;

    // GAMEPLAY
    private int mCurrentDay;
    public int CurrentDay
    {
        get { return mCurrentDay; }
    }

    // ------------ PUBLIC FUNCTIONS ------------

    // MONOBEHAVIOR
    void Awake()
    {
        mCurrentDay = 0;
    }

    // INPUT CONTROL
    public void Interact()
    {
        Interactable closestActor = Actors[0];
        float closestDistance = Vector3.Distance(Actors[0].GetComponent<Transform>().position, Player.position);
        for (int i = 0; i < Actors.Count; i++)
        {
            float dist = Vector3.Distance(Actors[i].GetComponent<Transform>().position, Player.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestActor = Actors[i];
            }
        }

        if (closestDistance <= MinInteractionDistance)
            closestActor.Interact();
    }

    // GAMEPLAY
    public void IncrementDay()
    {
        // TODO: play day change transition
        mCurrentDay++;
    }

}
