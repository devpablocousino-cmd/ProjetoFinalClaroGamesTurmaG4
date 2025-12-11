using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class DialogueControl : MonoBehaviour
{
    [Header("Components")]
    public GameObject dialogueObj;
    public Text actorNameText;
    public Text sentenceText;

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

        dialogueObj.SetActive(true);
        sentenceText.text = "";
        actorNameText.text = actorName;
        sentences = txt;
        index = 0;
        typingCoroutine = StartCoroutine(TypeSentence());

    }

    IEnumerator TypeSentence()
    {
        sentenceText.text = "";
        foreach (char letter in sentences[index].ToCharArray())
        {
            sentenceText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        typingCoroutine = null;
    }

    public void NextSentence()
    {
        if (sentenceText.text == sentences[index])
        {
            if (index < sentences.Length - 1)
            {
                index++;
                sentenceText.text = "";
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

    public void HidePanel()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        sentenceText.text = "";
        actorNameText.text = "";
        index = 0;
        dialogueObj.SetActive(false);
    }

    public void EndDialogue()
    {
        sentenceText.text = "";
        index = 0;
        dialogueObj.SetActive(false);
    }

}