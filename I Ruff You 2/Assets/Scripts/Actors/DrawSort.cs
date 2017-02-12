using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DrawSort : MonoBehaviour {

    private SpriteRenderer spriteRenderer;

    void Awake () {
        spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	void Update () {
        spriteRenderer.sortingOrder = 500 - Mathf.FloorToInt(spriteRenderer.gameObject.transform.position.y * 4);
    }
}
