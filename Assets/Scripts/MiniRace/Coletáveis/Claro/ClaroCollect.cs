using TMPro;
using UnityEngine;

public class CoinCollect : MonoBehaviour
{
    [Header("Components")]
    public TextMeshProUGUI PontuacaoText;
    
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Coin triggered!");
        Debug.Log("Collided with: " + other.gameObject.name);
        Debug.Log("Collided object's tag: " + other.gameObject.tag);
        if (other.CompareTag("Player") || other.GetComponentInParent<Rigidbody>().CompareTag("Player"))
        {
            Pontuacao pontuacao = other.GetComponentInParent<Pontuacao>();
            pontuacao.AdicionarPontos(1);
            PontuacaoText.text = "Pontuação: " + pontuacao.pontos.ToString();
            Destroy(gameObject);
        }
    }
}
