using TMPro;
using UnityEngine;

public class TimerRegressivo : MonoBehaviour
{
    public float tempoInicial = 60f;
    private float tempoAtual;
    private bool tempoAtivo = true;

    [Header("UI")]
    public TMP_Text uiTempo;
    public Color corNormal = Color.white;
    public Color corCritica = Color.red;

    [Header("Efeitos")]
    public float tempoCritico = 10f;
    public AudioSource audioSource;
    public AudioClip somTick;

    private int ultimoSegundoTocado = -1;

    void Start()
    {
        tempoAtual = tempoInicial;
        AtualizarUI();
    }

    void Update()
    {
        if (!tempoAtivo) return;

        tempoAtual -= Time.deltaTime;

        if (tempoAtual <= 0f)
        {
            tempoAtual = 0f;
            tempoAtivo = false;
            TempoAcabou();
        }

        EfeitosUltimosSegundos();
        AtualizarUI();
    }

    void AtualizarUI()
    {
        if (uiTempo == null) return;

        int tempoInteiro = Mathf.CeilToInt(tempoAtual);
        uiTempo.text = tempoInteiro.ToString();
    }

    void EfeitosUltimosSegundos()
    {
        if (uiTempo == null) return;

        int segundoAtual = Mathf.CeilToInt(tempoAtual);

        if (tempoAtual <= tempoCritico)
        {
            // muda cor
            uiTempo.color = corCritica;

            // pisca
            float alpha = Mathf.Abs(Mathf.Sin(Time.time * 6f));
            uiTempo.alpha = alpha;

            // som nos últimos 5 segundos
            if (segundoAtual <= 5 && segundoAtual != ultimoSegundoTocado)
            {
                ultimoSegundoTocado = segundoAtual;

                if (audioSource != null && somTick != null)
                    audioSource.PlayOneShot(somTick);
            }
        }
        else
        {
            uiTempo.color = corNormal;
            uiTempo.alpha = 1f;
        }
    }

    void TempoAcabou()
    {
        Debug.Log("Tempo acabou!");
        // aqui você chama Game Over
        // Time.timeScale = 0f;
    }
}
