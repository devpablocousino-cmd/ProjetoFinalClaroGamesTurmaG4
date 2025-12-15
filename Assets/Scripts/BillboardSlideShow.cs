using UnityEngine;

public class BillboardSlideShow : MonoBehaviour
{
    [Header("Index do Material a alterar (MeshRenderer)")]
    public int materialIndex = 2; // Element 2

    [Header("Slides")]
    public Texture2D[] slides;

    [Header("Configurações")]
    public float slideDuration = 4f;

    private MeshRenderer rendererTarget;
    private Material targetMaterial;
    private int current = 0;
    private float timer = 0f;

    void Awake()
    {
        // Pega o MeshRenderer automaticamente
        rendererTarget = GetComponent<MeshRenderer>();

        if (rendererTarget == null)
        {
            Debug.LogError("❌ Nenhum MeshRenderer encontrado no Billboard!");
            enabled = false;
            return;
        }
    }

    void Start()
    {
        // Instancia o material do índice correto (evita modificar o asset original)
        Material[] mats = rendererTarget.materials;

        if (materialIndex < 0 || materialIndex >= mats.Length)
        {
            Debug.LogError($"❌ materialIndex {materialIndex} está fora do range. O objeto tem {mats.Length} materiais.");
            enabled = false;
            return;
        }

        targetMaterial = mats[materialIndex];
        rendererTarget.materials = mats;

        if (slides.Length > 0)
            targetMaterial.SetTexture("_BaseMap", slides[0]);
    }

    void Update()
    {
        if (slides.Length <= 1) return;

        timer += Time.deltaTime;

        if (timer >= slideDuration)
        {
            timer = 0f;
            NextSlide();
        }
    }

    void NextSlide()
    {
        current = (current + 1) % slides.Length;
        targetMaterial.SetTexture("_BaseMap", slides[current]);
    }
}
