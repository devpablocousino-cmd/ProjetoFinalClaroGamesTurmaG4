
using UnityEngine;

public class Pontuacao : MonoBehaviour
{
    public int pontos;

    public void AdicionarPontos(int quantidade)
    {
        pontos += quantidade;
    }

    public void ResetarPontos()
    {
        pontos = 0;
    }
}