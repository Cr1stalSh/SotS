using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[Serializable]
public class DialogueOption
{
    public string text;        
    public string nextNodeId;  
}

[Serializable]
public class DialogueNode
{
    public string id;                    
    [TextArea] public string npcText;    
    public List<DialogueOption> options; 
    public bool isSuccessEnd = false;    
}


public class DialogueSystem : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public Button[] optionButtons; 

    [SerializeField] private GameObject npcObject;

    // Словарь узлов диалога
    private Dictionary<string, DialogueNode> nodes;
    private DialogueNode currentNode;

    void Awake()
    {
        dialoguePanel.SetActive(false);
        foreach (var b in optionButtons) b.gameObject.SetActive(false);

        nodes = new Dictionary<string, DialogueNode>();

        nodes["start"] = new DialogueNode
        {
            id = "start",
            npcText = "",
            options = new List<DialogueOption> {
                new DialogueOption { text = "Кто ты?",                nextNodeId = "whoYou" },
                new DialogueOption { text = "Что ты здесь делаешь?",   nextNodeId = "whatYouDoing" },
                new DialogueOption { text = "Я могу тебе помочь?",     nextNodeId = "endFail" },
            }
        };

        nodes["whoYou"] = new DialogueNode
        {
            id = "whoYou",
            npcText = "Лишь тень. То, что осталось от меня прошлого.",
            options = new List<DialogueOption> {
                new DialogueOption { text = "Тебе не одиноко?",      nextNodeId = "aloneQ" },
                new DialogueOption { text = "Почему ты здесь?",      nextNodeId = "noPlace" },
            }
        };

        nodes["aloneQ"] = new DialogueNode
        {
            id = "aloneQ",
            npcText = "Одиноко, но я пытался это исправить. Ничего не вышло. Смысл продолжать пытаться?",
            options = new List<DialogueOption> {
                new DialogueOption { text = "Легко же ты сдался.", nextNodeId = "endFail" },
                new DialogueOption {
                    text = "Нельзя бросать пытаться. Если не предпринять ничего, ничего не изменится.",
                    nextNodeId = "encourageSelf"
                },
            }
        };

        nodes["whatYouDoing"] = new DialogueNode
        {
            id = "whatYouDoing",
            npcText = "Хочу раствориться. Исчезнуть. Спрятаться от всех. Навсегда.",
            options = new List<DialogueOption> {
                new DialogueOption { text = "Что случилось?",              nextNodeId = "noPlace" },
                new DialogueOption { text = "Я вот тебя смог найти. Может ты плохо прячешься?", nextNodeId = "endFail" },
            }
        };

        nodes["noPlace"] = new DialogueNode
        {
            id = "noPlace",
            npcText = "Мне нет места среди других. Я лишь только мешаю.",
            options = new List<DialogueOption> {
                new DialogueOption {
                    text = "Значит ты не нашел тех, кто ценил бы тебя таким, какой ты есть.",
                    nextNodeId = "encourageSelf"
                },
                new DialogueOption { text = "Значит с тобой что-то не так.", nextNodeId = "endFail" },
            }
        };

        nodes["encourageSelf"] = new DialogueNode
        {
            id = "encourageSelf",
            npcText = "Сказал бы ты это раньше себе.",
            options = new List<DialogueOption>(),
            isSuccessEnd = true
        };

        // Узел провала
        nodes["endFail"] = new DialogueNode
        {
            id = "endFail",
            npcText = "Можешь оставить меня в покое. Уйди. Уйди и не возвращайся. Мне уже нечем помочь.",
            options = new List<DialogueOption>()
        };
    }

    public void StartDialogue()
    {
        dialoguePanel.SetActive(true);
        ShowNode("start");
    }

    private void ShowNode(string nodeId)
    {
        if (!nodes.TryGetValue(nodeId, out currentNode))
        {
            Debug.LogError($"DialogueSystem: узел «{nodeId}» не найден");
            EndDialogue();
            return;
        }

        dialogueText.text = currentNode.npcText;
        
        foreach (var b in optionButtons)
            b.gameObject.SetActive(false);

        if (currentNode.options.Count > 0)
        {
            // Отображаем кнопки-ответы
            for (int i = 0; i < currentNode.options.Count && i < optionButtons.Length; i++)
            {
                var opt = currentNode.options[i];
                var btn = optionButtons[i];
                btn.gameObject.SetActive(true);
                btn.GetComponentInChildren<TextMeshProUGUI>().text = opt.text;

                btn.onClick.RemoveAllListeners();
                string nextId = opt.nextNodeId;
                btn.onClick.AddListener(() => ShowNode(nextId));
            }
        }
        else
        {
            if (nodeId == "endFail")
            {
                Invoke(nameof(RestartLevel), 3f);
            }
            else
            {
                Invoke(nameof(EndLevel), 3f);
            }
        }
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
    }

    public void EndLevel()
    {
        CancelInvoke();
        dialoguePanel.SetActive(false);
        GameManager.Instance.CompleteLevel(LevelID.Dialogue);
        GameManager.Instance.UnlockAbility(Ability.Ghost);
        string scene_name = "battle";
        SceneManager.LoadScene(scene_name);
    }

    private void RestartLevel()
    {
        if (npcObject != null)
            npcObject.SetActive(false);
        GameManager.Instance.NotifyLevelRestart();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
