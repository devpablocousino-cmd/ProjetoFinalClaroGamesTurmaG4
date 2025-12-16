using UnityEngine;

/// <summary>
/// Script auxiliar para posicionar o painel de diálogo corretamente na tela.
/// Adicione este script ao mesmo GameObject que contém o DialogueControl.
/// Execute uma vez no Editor (botão direito no componente > "Setup Dialogue Panel")
/// ou deixe configurar automaticamente no Start().
/// </summary>
[RequireComponent(typeof(DialogueControl))]
public class DialoguePanelSetup : MonoBehaviour
{
    [Header("Panel Settings")]
    [Tooltip("Altura do painel de diálogo em pixels")]
    public float panelHeight = 180f;
    
    [Tooltip("Margem lateral do painel")]
    public float horizontalMargin = 40f;
    
    [Tooltip("Distância do fundo da tela")]
    public float bottomMargin = 30f;

    [Header("Auto Setup")]
    [Tooltip("Configurar automaticamente ao iniciar")]
    public bool setupOnStart = true;

    private DialogueControl dialogueControl;

    void Start()
    {
        dialogueControl = GetComponent<DialogueControl>();
        
        if (setupOnStart && dialogueControl != null && dialogueControl.dialogueObj != null)
        {
            SetupDialoguePanel();
        }
    }

    /// <summary>
    /// Configura o painel de diálogo para ficar na parte inferior da tela
    /// </summary>
    [ContextMenu("Setup Dialogue Panel")]
    public void SetupDialoguePanel()
    {
        if (dialogueControl == null)
            dialogueControl = GetComponent<DialogueControl>();

        if (dialogueControl == null || dialogueControl.dialogueObj == null)
        {
            Debug.LogError("[DialoguePanelSetup] DialogueControl ou dialogueObj não encontrado!");
            return;
        }

        RectTransform panelRect = dialogueControl.dialogueObj.GetComponent<RectTransform>();
        if (panelRect == null)
        {
            Debug.LogError("[DialoguePanelSetup] O dialogueObj não tem RectTransform!");
            return;
        }

        // Configura as âncoras para bottom-stretch (parte inferior, esticado horizontalmente)
        panelRect.anchorMin = new Vector2(0, 0);  // Canto inferior esquerdo
        panelRect.anchorMax = new Vector2(1, 0);  // Canto inferior direito (esticado em X)
        
        // Configura o pivot para o centro inferior
        panelRect.pivot = new Vector2(0.5f, 0);
        
        // Configura posição e tamanho
        panelRect.anchoredPosition = new Vector2(0, bottomMargin);
        panelRect.sizeDelta = new Vector2(-horizontalMargin * 2, panelHeight);
        
        // Configura as margens (offsetMin e offsetMax)
        panelRect.offsetMin = new Vector2(horizontalMargin, bottomMargin);
        panelRect.offsetMax = new Vector2(-horizontalMargin, bottomMargin + panelHeight);

        Debug.Log("[DialoguePanelSetup] Painel de diálogo configurado com sucesso!");
        
        // Também tenta configurar o texto se existir
        SetupTextAlignment();
    }

    /// <summary>
    /// Configura o alinhamento do texto de diálogo
    /// </summary>
    private void SetupTextAlignment()
    {
        if (dialogueControl.speetchText != null)
        {
            // Para UI.Text legado
            dialogueControl.speetchText.alignment = TextAnchor.UpperLeft;
            
            RectTransform textRect = dialogueControl.speetchText.GetComponent<RectTransform>();
            if (textRect != null)
            {
                // Estica o texto para preencher o painel com margens
                textRect.anchorMin = new Vector2(0, 0);
                textRect.anchorMax = new Vector2(1, 1);
                textRect.offsetMin = new Vector2(20, 20);  // Padding esquerda/baixo
                textRect.offsetMax = new Vector2(-20, -40); // Padding direita/cima (espaço para nome)
            }
        }

        if (dialogueControl.actorNameText != null)
        {
            // Alinha o nome do ator no topo esquerdo
            dialogueControl.actorNameText.alignment = TextAnchor.UpperLeft;
            
            RectTransform nameRect = dialogueControl.actorNameText.GetComponent<RectTransform>();
            if (nameRect != null)
            {
                nameRect.anchorMin = new Vector2(0, 1);
                nameRect.anchorMax = new Vector2(1, 1);
                nameRect.pivot = new Vector2(0.5f, 1);
                nameRect.anchoredPosition = new Vector2(0, -10);
                nameRect.sizeDelta = new Vector2(-40, 30);
            }
        }
    }
}
