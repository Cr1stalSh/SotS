using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    public GameObject hintText; 
    public DialogueSystem dialogueSystem; 

    private bool isPlayerNear = false;

    void Start()
    {
        hintText.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.H))
        {
            dialogueSystem.StartDialogue();
            hintText.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            hintText.SetActive(true);
            isPlayerNear = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            hintText.SetActive(false);
            isPlayerNear = false;
            dialogueSystem.EndDialogue();
        }
    }
}
