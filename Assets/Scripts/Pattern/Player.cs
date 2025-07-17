using UnityEngine;

public class Player : MonoBehaviour
{
    private Shadow currentShadow = null;
    public float moveSpeed = 5f;

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, v, 0) * moveSpeed * Time.deltaTime;

        transform.Translate(move, Space.World);

        if (Input.GetKeyDown(KeyCode.Space) && currentShadow != null)
        {
            PatternManager levelManager = FindObjectOfType<PatternManager>();
            if (levelManager != null)
            {
                currentShadow.LightUp();
                levelManager.PlayerInput(currentShadow);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Shadow shadow = other.GetComponent<Shadow>();
        if (shadow != null)
        {
            currentShadow = shadow;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Shadow shadow = other.GetComponent<Shadow>();
        if (shadow != null && currentShadow == shadow)
        {
            currentShadow = null;
        }
    }
}
