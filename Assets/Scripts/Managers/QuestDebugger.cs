using UnityEngine;
using UnityEngine.InputSystem;

public class QuestDebugger : MonoBehaviour
{
    void Update()
    {
        // Pressione T para testar
        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
        {
            bool isCompleted = QuestManager.instance.IsQuestCompleted("Ativar antena 1");
            Debug.Log($"[DEBUG] Quest 'Ativar antena 1' está completada? {isCompleted}");
        }
    }
}