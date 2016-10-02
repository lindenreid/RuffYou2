using UnityEngine;

public class CharacterMovement : MonoBehaviour {

    public GameController GameController;

	public float Speed = 1.0f;
    
	void Update()
    {
        // MOVEMENT
        float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");

        if (vertical != 0 || horizontal != 0) {
            float hyp = Mathf.Sqrt(transform.position.x * transform.position.x + transform.position.y * transform.position.y);
            transform.position = new Vector3(transform.position.x, transform.position.y, (hyp / 2.525f) * -0.01f);
        }

        if (Mathf.Abs(vertical) > 0.001)
		    transform.Translate(new Vector3(horizontal, vertical, 0) * Speed * 0.6f * Time.deltaTime );
        else
            transform.Translate(new Vector3(horizontal, vertical, 0) * Speed * Time.deltaTime);

        // INPUT
        if (Input.GetKeyUp(KeyCode.Space))
        {
            GameController.Interact();
        }
    }
}
