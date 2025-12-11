using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    private Dictionary<string, bool> completedQuests = new Dictionary<string, bool>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CompleteQuest(string questName)
    {
        if (!completedQuests.ContainsKey(questName))
        {
            completedQuests[questName] = true;
            Debug.Log("Quest completada: " + questName);
        }
    }

    public bool IsQuestCompleted(string questName)
    {
        return completedQuests.ContainsKey(questName) && completedQuests[questName];
    }

    public void ResetQuest(string questName)
    {
        if (completedQuests.ContainsKey(questName))
        {
            completedQuests[questName] = false;
        }
    }
}