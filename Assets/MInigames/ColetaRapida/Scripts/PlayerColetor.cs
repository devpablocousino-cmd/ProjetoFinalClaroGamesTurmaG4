using UnityEngine;

public class PlayerColetor : MonoBehaviour
{
    public EntregadorManager gerente; // Precisa saber quem manda no jogo

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se bateu na tag "Pacote"
        if (other.CompareTag("Pacote"))
        {
            gerente.PegouPacote();
        }
        // Verifica se bateu na tag "Cliente"
        else if (other.CompareTag("Cliente"))
        {
            gerente.EntregouPacote();
        }
    }
}