using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class DialogueControl : MonoBehaviour
{
    [Header("Components")]
    public GameObject dialogueObj;
    public Text actorNameText;
    public Text speetchText;

    [Header("Variables")]
    public float typingSpeed;
    private string[] sentences;
    private int index;
    private Coroutine typingCoroutine;


    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            NextSentence();
        }
    }

    public void Speech(string[] txt, string actorName)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        if (dialogueObj != null)
        {
            dialogueObj.SetActive(true);
            Debug.Log("Quest View ativado!");
        }
        
        if (speetchText != null)
        {
            speetchText.text = "";
            Debug.Log("Texto iniciado. Primeira frase: " + (txt.Length > 0 ? txt[0] : "VAZIO"));
        }
        
        if (actorNameText != null)
        {
            actorNameText.text = actorName;
            Debug.Log("Nome do ator: " + actorName);
        }
        
        sentences = txt;
        index = 0;
        typingCoroutine = StartCoroutine(TypeSentence());

    }

    IEnumerator TypeSentence()
    {
        speetchText.text = "";
        foreach (char letter in sentences[index].ToCharArray())
        {
            speetchText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        typingCoroutine = null;
    }

    public void NextSentence()
    {
        if (speetchText.text == sentences[index])
        {
            if (index < sentences.Length - 1)
            {
                index++;
                speetchText.text = "";
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                }
                typingCoroutine = StartCoroutine(TypeSentence()); //armazenar a coroutine na variavel
            }
            else
            {
                EndDialogue();
            }
        }
    }

    public void PrevSentence(InputAction.CallbackContext context)
    {

    }

    public void HidePanel()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        
        if (speetchText != null)
            speetchText.text = "";
        
        if (actorNameText != null)
            actorNameText.text = "";
        
        index = 0;
        
        if (dialogueObj != null)
            dialogueObj.SetActive(false);
    }

    public void EndDialogue()
    {
        speetchText.text = "";
        index = 0;
        dialogueObj.SetActive(false);
    }

}
