using UnityEngine;

public class CarIdleDetector : MonoBehaviour
{
    public Rigidbody rb;

    [Header("Configurações")]
    public float speedThreshold = 0.1f; // km/h
    public float idleTimeRequired = 30f; // segundos

    private float idleTimer = 0f;
    private bool isIdle = false;

    private Vector3 startPosition;
    private Quaternion startRotation;

    void Start()
    {
        startPosition = rb.position;
        startRotation = rb.rotation;
    }

    void Update()
    {
        float speedKmh = rb.linearVelocity.magnitude * 3.6f;

        if (speedKmh < speedThreshold)
        {
            idleTimer += Time.deltaTime;

            if (!isIdle && idleTimer >= idleTimeRequired)
            {
                isIdle = true;
                OnCarIdle();
            }
        }
        else
        {
            idleTimer = 0f;
            isIdle = false;
        }
    }

    void OnCarIdle()
    {
        Debug.Log($"🚗 Carro parado por mais de {idleTimeRequired} segundos");
        RespawnCar();
    }

    void RespawnCar()
    {
        var pontuacao = GetComponent<Pontuacao>();

        Vector3 targetPos;
        Quaternion targetRot;

        if (pontuacao != null && pontuacao.TemUltimoCheckpoint())
        {
            pontuacao.ObterUltimoCheckpoint(out targetPos, out targetRot);
        }
        else
        {
            targetPos = startPosition;
            targetRot = startRotation;
        }

        // 🔥 ZERA FÍSICA (Unity 6.x)
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // 🔥 TELEPORTE FÍSICO CORRETO
        rb.position = targetPos;
        rb.rotation = targetRot;

        // 🔥 GARANTE ESTABILIDADE
        rb.Sleep();
        rb.WakeUp();

        Debug.Log("🚗 Carro respawnado corretamente!");
    }
}
