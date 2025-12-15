using UnityEngine;
using System.Collections.Generic;

public class CollectableSpawner : MonoBehaviour
{
    [Header("Prefab do Coletável")]
    public GameObject collectablePrefab;

    [Header("Quantidade")]
    public int amount = 30;

    [Header("Raycast")]
    public float rayHeight = 80f;
    public LayerMask trackLayer;
    public float yOffset = 0.4f;

    [Header("Evitar Sobreposição")]
    public float checkRadius = 0.4f;

    // Lista automática contendo todos os trechos válidos da pista
    private List<BoxCollider> pistaColliders = new List<BoxCollider>();


    void Start()
    {
        DetectarPista();
    }


    // Detecta todos os BoxColliders válidos da pista
    void DetectarPista()
    {
        pistaColliders.Clear();

        var todos = FindObjectsByType<BoxCollider>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var box in todos)
        {
            // Apenas objetos no trackLayer
            if ((trackLayer.value & (1 << box.gameObject.layer)) == 0)
                continue;

            // Ignorar sidewalk
            string nome = box.gameObject.name.ToLower();
            if (nome.Contains("sidewalk_l") || nome.Contains("sidewalk_r"))
                continue;

            pistaColliders.Add(box);
        }

        Debug.Log($"Detectados {pistaColliders.Count} trechos de pista válidos.");
    }


    void OnTriggerEnter(Collider other)
    {
        // Só ativa com Player
        if (!other.CompareTag("Player") && !other.GetComponentInParent<Rigidbody>().CompareTag("Player"))
            return;

        Spawn();
    }


    void Spawn()
    {
        if (pistaColliders.Count == 0)
        {
            Debug.LogWarning("Nenhum trecho de pista detectado! Verifique o trackLayer.");
            return;
        }

        ClearAllCollectables();
        collectablePrefab.SetActive(true);

        int spawned = 0;
        int safety = 0;

        while (spawned < amount && safety < amount * 40)
        {
            safety++;

            // Escolhe um trecho da pista aleatório
            BoxCollider trecho = pistaColliders[Random.Range(0, pistaColliders.Count)];

            // Gera posição local dentro do BoxCollider
            Vector3 local = new Vector3(
                Random.Range(-trecho.size.x / 2f, trecho.size.x / 2f),
                0,
                Random.Range(-trecho.size.z / 2f, trecho.size.z / 2f)
            );

            // Converte para coordenadas do mundo
            Vector3 world = trecho.transform.TransformPoint(local);

            // Raycast para encontrar a superfície real da pista
            if (!Physics.Raycast(world + Vector3.up * rayHeight, Vector3.down, out RaycastHit hit, 200f, trackLayer))
                continue;

            Vector3 pos = hit.point + Vector3.up * yOffset;

            // Evita coletáveis empilhados
            int collectableLayerMask = LayerMask.GetMask("Collectable");
            if (Physics.CheckSphere(pos, checkRadius, collectableLayerMask))
                continue;

            // Instancia
            Instantiate(collectablePrefab, pos, Quaternion.identity);
            spawned++;
        }

        collectablePrefab.SetActive(false);

        Debug.Log($"Spawn finalizado: {spawned} coletáveis instanciados.");
    }


    public void ClearAllCollectables()
    {
        int collectableLayer = LayerMask.NameToLayer("Collectable");

        GameObject[] allObjects = FindObjectsByType<GameObject>(UnityEngine.FindObjectsSortMode.None);

        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == collectableLayer && obj != collectablePrefab)
            {
                Destroy(obj);
            }
        }
    }
}
