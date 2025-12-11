using UnityEngine;
using UnityEngine.InputSystem;

// Classe auxiliar (não é MonoBehaviour, fica no mesmo arquivo)
[System.Serializable]
public class NPCDialogueData
{
    public string actorName;
    [TextArea(3, 10)]
    public string[] sentences;
    public string requiredQuest = "";
    public bool questMustBeCompleted = false;
}

public class NPCDialogue : MonoBehaviour
{
    [Header("Dialogue System")]
    public DialogueControl dialogueControl; // ARRASTE SEU DialogueControl AQUI NO INSPECTOR

    [Header("Dialogue States")]
    public NPCDialogueData defaultDialogue;
    public NPCDialogueData[] conditionalDialogues;

    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange)
        {
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                Debug.Log("E pressionado!");
                StartDialogue();
            }
        }
    }

    void StartDialogue()
    {
        Debug.Log("StartDialogue chamado!");

        if (dialogueControl == null)
        {
            Debug.LogError("DialogueControl não está configurado no Inspector!");
            return;
        }

        NPCDialogueData dialogueToUse = GetCurrentDialogue();

        if (dialogueToUse != null)
        {
            Debug.Log("Iniciando diálogo: " + dialogueToUse.actorName);
            dialogueControl.Speech(dialogueToUse.sentences, dialogueToUse.actorName);
        }
        else
        {
            Debug.LogError("Nenhum diálogo encontrado!");
        }
    }

    NPCDialogueData GetCurrentDialogue()
    {
        // Verifica os diálogos condicionais primeiro
        foreach (NPCDialogueData dialogue in conditionalDialogues)
        {
            if (string.IsNullOrEmpty(dialogue.requiredQuest))
                continue;

            bool questCompleted = QuestManager.instance.IsQuestCompleted(dialogue.requiredQuest);

            if (dialogue.questMustBeCompleted == questCompleted)
            {
                return dialogue;
            }
        }

        return defaultDialogue;
    }

    // PARA JOGOS 3D - use estes métodos
    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("OnTriggerEnter detectado! Tag: " + collision.tag);

        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("✓ Player entrou na área do NPC!");
        }
    }

    void OnTriggerExit(Collider collision)
    {
        Debug.Log("OnTriggerExit detectado!");

        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("✓ Player saiu da área do NPC!");

            if (dialogueControl != null)
            {
                dialogueControl.HidePanel();
            }
        }
    }

    // PARA JOGOS 2D - descomente estes métodos e comente os de cima
    /*
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnTriggerEnter2D detectado! Tag: " + collision.tag);
        
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("✓ Player entrou na área do NPC!");
        }
    }
    
    void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("OnTriggerExit2D detectado!");
        
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("✓ Player saiu da área do NPC!");
            
            if (dialogueControl != null)
            {
                dialogueControl.HidePanel();
            }
        }
    }
    */
}