using UnityEngine;

/// <summary>
/// Moeda coletável que adiciona ao CurrencyManager.
/// Pode ser usada no mundo aberto ou em minigames.
/// </summary>
public class CoinCollectible : MonoBehaviour
{
    [Header("Coin Settings")]
    [SerializeField] private int coinValue = 10;
    [SerializeField] private bool destroyOnCollect = true;

    [Header("Visual/Audio")]
    [SerializeField] private GameObject collectEffect;
    [SerializeField] private AudioClip collectSound;
    
    [Header("Animation")]
    [SerializeField] private bool rotate = true;
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private bool bounce = true;
    [SerializeField] private float bounceHeight = 0.2f;
    [SerializeField] private float bounceSpeed = 2f;

    private Vector3 startPosition;
    private AudioSource audioSource;

    private void Start()
    {
        startPosition = transform.position;
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // Animação de rotação
        if (rotate)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }

        // Animação de bounce
        if (bounce)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    // Para 2D
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    /// <summary>
    /// Coleta a moeda
    /// </summary>
    private void Collect()
    {
        // Adicionar moedas ao CurrencyManager
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddCoins(coinValue);
            Debug.Log($"[CoinCollectible] +{coinValue} moedas coletadas!");
        }
        else
        {
            // Fallback: adicionar ao ScoreManager se não houver CurrencyManager
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddMazeScore(coinValue);
            }
            Debug.LogWarning("[CoinCollectible] CurrencyManager não encontrado!");
        }

        // Efeitos visuais
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        // Som
        if (collectSound != null)
        {
            // Tocar som em posição (o objeto será destruído)
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        // Destruir ou desativar
        if (destroyOnCollect)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
