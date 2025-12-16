using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    [Header("Quest Settings")]
    public string questName = "buscar_ervas";
    public bool completeOnTrigger = true;

    [Header("Optional")]
    public GameObject objectToDestroy;

    // PARA JOGOS 3D
    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player") && completeOnTrigger)
        {
            CompleteQuest();
        }
    }

    // PARA JOGOS 2D - descomente e comente o de cima
    /*
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && completeOnTrigger)
        {
            CompleteQuest();
        }
    }
    */

    public void CompleteQuest()
    {
        QuestManager.instance.CompleteQuest(questName);

        if (objectToDestroy != null)
        {
            Destroy(objectToDestroy);
        }

        Debug.Log("Quest " + questName + " completada!");
    }
}