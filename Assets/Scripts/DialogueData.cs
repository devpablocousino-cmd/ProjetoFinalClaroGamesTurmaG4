using UnityEngine;

[System.Serializable]
public class DialogueData
{
    public string actorName;
    [TextArea(3, 10)]
    public string[] sentences;
    public string requiredQuest = ""; // Quest necessária para este diálogo (deixe vazio para diálogo padrão)
    public bool questMustBeCompleted = false; // True = quest deve estar completa, False = quest não pode estar completa
}
