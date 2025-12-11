using UnityEngine;
using UnityEngine.InputSystem;

public class QuestTrigger : MonoBehaviour
{
    [Header("Quest Settings")]
    public string questName = "buscar_ervas";

    [Header("Objects")]
    public GameObject objectToSpawn; // Objeto que vai aparecer no lugar
    public Transform spawnPosition; // Posição específica para spawnar (opcional)
    public GameObject objectToDestroy; // Deixe vazio para destruir o próprio objeto

    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame)
        {
            CompleteQuest();
        }
    }

    public void CompleteQuest()
    {
        Debug.Log("========================================");
        Debug.Log($"CompleteQuest chamado para: '{questName}'");

        if (QuestManager.instance == null)
        {
            Debug.LogError("QuestManager.instance é NULL! Crie um GameObject 'QuestManager' na cena!");
            return;
        }

        QuestManager.instance.CompleteQuest(questName);
        Debug.Log($"✓ Quest '{questName}' completada no QuestManager!");

        // Verifica se realmente foi salva
        bool isCompleted = QuestManager.instance.IsQuestCompleted(questName);
        Debug.Log($"✓ Verificação: Quest '{questName}' está completada? {isCompleted}");

        // Spawna o novo objeto (se configurado)
        if (objectToSpawn != null)
        {
            Vector3 spawnPos;
            Quaternion spawnRot;

            // Usa posição específica se definida, senão usa a posição deste objeto
            if (spawnPosition != null)
            {
                spawnPos = spawnPosition.position;
                spawnRot = spawnPosition.rotation;
                Debug.Log($"✓ Spawnando na posição específica: {spawnPos}");
            }
            else
            {
                spawnPos = transform.position;
                spawnRot = transform.rotation;
                Debug.Log($"✓ Spawnando na posição do objeto: {spawnPos}");
            }

            Instantiate(objectToSpawn, spawnPos, spawnRot);
            Debug.Log("✓ Novo objeto spawnado!");
        }

        // Destrói o objeto especificado ou o próprio objeto
        if (objectToDestroy != null)
        {
            Destroy(objectToDestroy);
        }
        else
        {
            Destroy(gameObject);
        }
        Debug.Log("========================================");
    }

    // PARA JOGOS 3D
    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Pressione Q para coletar");
        }
    }

    void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    // PARA JOGOS 2D - descomente e comente os de cima
    /*
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Pressione Q para coletar");
        }
    }
    
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
    */
}

/* ==================== COMO USAR ====================

SETUP:
1. Adicione este script no objeto que será coletado/destruído
2. Adicione um Collider (marque "Is Trigger")
3. Configure no Inspector:
   - Quest Name: "Ativar antena 1"
   - Object To Spawn: arraste o prefab da antena
   - Spawn Position: (OPCIONAL) arraste um GameObject vazio na posição onde quer spawnar
   - Object To Destroy: deixe vazio (destrói o próprio) OU arraste outro objeto

SPAWN POSITION:
- Se VAZIO: spawna na posição do objeto destruído
- Se PREENCHIDO: spawna na posição exata do GameObject configurado

COMO CONFIGURAR SPAWN POSITION:
1. Crie um GameObject vazio (botão direito → Create Empty)
2. Renomeie para "SpawnPoint_Antena1"
3. Posicione onde quer que a antena apareça
4. Arraste esse GameObject para o campo "Spawn Position"

FUNCIONA ASSIM:
1. Player entra na área do trigger
2. Aparece no console: "Pressione Q para coletar"
3. Player aperta Q
4. Quest é completada
5. Novo objeto spawna na mesma posição
6. Objeto antigo é destruído
7. Diálogo do NPC muda!

IMPORTANTE:
O "Quest Name" PRECISA ser IGUAL ao "Required Quest" do NPC!

*/