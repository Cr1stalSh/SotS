using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class NPCLinearDialogue : MonoBehaviour
{
    [Header("Scene")]
    public string scene_name;

    [Header("Trigger Hint")]
    public GameObject hintText;          

    [Header("Dialogue UI")]
    public GameObject dialoguePanel;     
    public TextMeshProUGUI dialogueText; 
    public Button nextButton;            

    [TextArea]
    public string[] lines;               

    private int currentIndex = 0;
    private bool isPlayerNear = false;

    void Start()
    {
        hintText.SetActive(false);
        dialoguePanel.SetActive(false);
        nextButton.onClick.AddListener(OnNextClicked);
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.H))
        {
            StartDialogue();
        }
    }

    private void StartDialogue()
    {
        currentIndex = 0;
        dialoguePanel.SetActive(true);
        hintText.SetActive(false);
        ShowLine(currentIndex);
    }

    private void ShowLine(int index)
    {
        if (index < lines.Length)
        {
            dialogueText.text = lines[index];
            nextButton.gameObject.SetActive(true);
        }
        else
        {
            BeginSceneTransition();
        }
    }

    private void OnNextClicked()
    {
        currentIndex++;
        ShowLine(currentIndex);
    }

    private void BeginSceneTransition()
    {
        dialoguePanel.SetActive(false);
        nextButton.gameObject.SetActive(false);
        SceneManager.LoadScene(scene_name);
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        nextButton.gameObject.SetActive(false);
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
            EndDialogue();
        }
    }
}
