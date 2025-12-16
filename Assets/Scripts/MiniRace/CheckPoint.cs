using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;
public class CheckPoint : MonoBehaviour
{    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponentInParent<Rigidbody>().CompareTag("Player"))
        {
            Pontuacao pontuacao = other.GetComponentInParent<Pontuacao>();
            pontuacao.AdicionarCheckpoint();
            pontuacao.SetUltimoCheckpoint(gameObject.transform);
            Destroy(gameObject);            
        }        
    }

}
