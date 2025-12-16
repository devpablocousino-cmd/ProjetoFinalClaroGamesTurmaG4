/*
================================================================================
    GUIA DE CONFIGURAÇÃO - Sistema Integrado de Quests, Minigames e Moedas
    Claro Party Games - ProjetoFinal
================================================================================

Este documento descreve como configurar a hierarquia de objetos no Unity
para que todos os sistemas funcionem de forma integrada.

================================================================================
    ARQUITETURA DO SISTEMA
================================================================================

    ┌─────────────────────────────────────────────────────────────────┐
    │                        GAME MANAGER                              │
    │  - Coordena minigames                                           │
    │  - Conecta QuestTrigger → Minigame → Conclusão da Quest         │
    └─────────────────────────────────────────────────────────────────┘
                                    │
            ┌───────────────────────┼───────────────────────┐
            ▼                       ▼                       ▼
    ┌───────────────┐      ┌───────────────┐      ┌───────────────┐
    │ QUEST MANAGER │      │CURRENCY MANAGER│      │ANTENNA MANAGER │
    │ - Estado das  │      │ - Moedas do    │      │ - Progresso    │
    │   quests      │      │   jogador      │      │   antenas      │
    │ - Progresso   │      │ - AddCoins()   │      │ - 300 moedas   │
    └───────────────┘      │ - SpendCoins() │      │   por antena   │
                           └───────────────┘      └───────────────┘
                                    │
            ┌───────────────────────┼───────────────────────┐
            ▼                       ▼                       ▼
    ┌───────────────┐      ┌───────────────┐      ┌───────────────┐
    │  SCORE MANAGER │      │  QUEST TRIGGER │      │COIN COLLECTIBLE│
    │ - Pontuação    │      │ - Detecta player│      │ - Moedas no   │
    │   dos minigames│      │ - Inicia quest │      │   cenário     │
    └───────────────┘      │ - 3 modos      │      └───────────────┘
                           └───────────────┘

================================================================================
    PASSO 1: CRIAR OS GAME OBJECTS NA HIERARQUIA
================================================================================

Na raiz da cena (ou em uma cena "Managers" que persiste), crie:

    Hierarchy/
    ├── [MANAGERS]                    ← GameObject vazio, pai dos managers
    │   ├── GameManager              ← Script: GameManager.cs
    │   ├── QuestManager             ← Script: QuestManager.cs
    │   ├── ScoreManager             ← Script: ScoreManager.cs
    │   ├── CurrencyManager          ← Script: CurrencyManager.cs
    │   └── AntennaManager           ← Script: AntennaManager.cs
    │
    ├── [UI]
    │   ├── Canvas
    │   │   ├── CurrencyDisplay      ← Script: CurrencyUI.cs
    │   │   ├── AntennaProgress      ← Script: AntennaProgressUI.cs
    │   │   └── QuestUI              ← (seu script de UI de quests)
    │
    ├── [MINIGAMES]
    │   ├── MiniRaceArea             ← Área do MiniRace (desativada inicialmente)
    │   └── MazeArea                 ← Área do Labirinto (desativada inicialmente)
    │
    ├── [ANTENAS]
    │   ├── Antena1                  ← Script: QuestTrigger (modo: RequiresCoins)
    │   ├── Antena2                  ← Script: QuestTrigger (modo: RequiresCoins)
    │   └── Antena3                  ← Script: QuestTrigger (modo: RequiresCoins)
    │
    └── [QUEST_TRIGGERS]
        ├── QuestTrigger_Missao1     ← Script: QuestTrigger (modo: RequiresMinigame)
        └── QuestTrigger_Missao2     ← Script: QuestTrigger (modo: Instant)

================================================================================
    PASSO 2: CONFIGURAR GAMEMANAGER
================================================================================

No Inspector do GameManager:

    [Minigame Selection]
    ├── Selection Mode: Sequential (ou Random, PlayerChoice, Specific)
    
    [Minigame References]
    ├── Mini Race Area: → [arrastar MiniRaceArea]
    ├── Maze Area: → [arrastar MazeArea]
    └── Minigame Selection UI: → [arrastar UI de seleção, se usar PlayerChoice]

    [Player Reference]
    ├── Player: → [arrastar o Player]
    └── Player Spawn Point: → [ponto de spawn após minigame]

================================================================================
    PASSO 3: CONFIGURAR ANTENNA MANAGER
================================================================================

No Inspector do AntennaManager:

    [Antenna Settings]
    ├── Total Antennas Required: 3
    ├── Cost Per Antenna: 300
    └── Antenna Quest Name: "quest_antenas"

    [Visual References]
    ├── Antenna Visuals: (lista com os 3 modelos 3D das antenas)
    ├── Antenna On Material: → [material quando ligada]
    └── Antenna Off Material: → [material quando desligada]

================================================================================
    PASSO 4: CONFIGURAR CADA QUEST TRIGGER
================================================================================

--- PARA QUESTS QUE REQUEREM MINIGAME ---

    [Quest Settings]
    ├── Quest Name: "quest_exemplo"
    └── Completion Mode: RequiresMinigame
    
    [Minigame Settings]
    └── Minigame Type: MiniRace (ou Maze, ou None para usar seleção do GameManager)

--- PARA ANTENAS (REQUEREM MOEDAS) ---

    [Quest Settings]
    ├── Quest Name: "antena_1"
    └── Completion Mode: RequiresCoins
    
    [Coin Settings]
    └── Coin Cost: 300
    
    [Antenna Settings]
    ├── Is Antenna: ✓ (marcado)
    └── Antenna Manager: → [arrastar AntennaManager]

--- PARA QUESTS INSTANTÂNEAS ---

    [Quest Settings]
    ├── Quest Name: "quest_simples"
    └── Completion Mode: Instant

================================================================================
    PASSO 5: CONFIGURAR MAZE EXIT POINT
================================================================================

O MazeExitPoint agora notifica o GameManager automaticamente.
Verifique se está configurado corretamente:

    [Scoring]
    └── Time Scoring: → [arrastar TimeBasedScoring]

Quando o jogador sair do labirinto:
1. TimeBasedScoring calcula as estrelas
2. ScoreManager recebe os pontos
3. GameManager.OnMinigameComplete() é chamado
4. Se havia uma quest pendente, ela é completada

================================================================================
    PASSO 6: ADICIONAR MOEDAS NO CENÁRIO
================================================================================

Para cada moeda coletável:

1. Criar um GameObject com Collider (Is Trigger: ✓)
2. Adicionar script: CoinCollectible
3. Configurar:
    ├── Coin Value: 10 (ou outro valor)
    ├── Collect Effect: → [prefab de partículas]
    └── Collect Sound: → [audio clip]

================================================================================
    PASSO 7: CONFIGURAR A UI
================================================================================

--- CURRENCY UI ---
Adicionar em um objeto filho do Canvas:

    [UI References]
    └── Coins Text: → [TextMeshProUGUI para mostrar moedas]
    
    [Format]
    └── Format: "Moedas: {0}"

--- ANTENNA PROGRESS UI ---
Adicionar em um objeto filho do Canvas:

    [UI References]
    ├── Progress Text: → [TextMeshProUGUI]
    ├── Progress Slider: → [Slider opcional]
    └── Antenna Icons: → [lista de Images para cada antena]

================================================================================
    FLUXO DE UMA QUEST COM MINIGAME
================================================================================

    1. Jogador entra no trigger de uma quest
                    │
                    ▼
    2. QuestTrigger detecta (OnTriggerEnter)
                    │
                    ▼
    3. completionMode == RequiresMinigame?
                    │ SIM
                    ▼
    4. GameManager.StartMinigameForQuest("quest_exemplo")
                    │
                    ▼
    5. GameManager seleciona minigame (Random/Sequential/etc)
                    │
                    ▼
    6. Minigame é ativado (MazeArea ou MiniRaceArea)
                    │
                    ▼
    7. Jogador completa o minigame
                    │
                    ▼
    8. MazeExitPoint chama GameManager.OnMinigameComplete(true, moedas)
                    │
                    ▼
    9. GameManager adiciona moedas via CurrencyManager.AddCoins()
                    │
                    ▼
    10. GameManager completa a quest via QuestManager.CompleteQuest()
                    │
                    ▼
    11. Eventos OnQuestCompleted são disparados

================================================================================
    FLUXO DE ATIVAÇÃO DE ANTENA
================================================================================

    1. Jogador entra no trigger da antena
                    │
                    ▼
    2. QuestTrigger detecta (completionMode == RequiresCoins)
                    │
                    ▼
    3. CurrencyManager.HasEnoughCoins(300)?
                    │
            ┌───────┴───────┐
            │ NÃO           │ SIM
            ▼               ▼
    4a. OnInsufficientCoins  4b. CurrencyManager.SpendCoins(300)
        (UI mostra aviso)           │
                                    ▼
                            5. AntennaManager.ActivateAntenna()
                                    │
                                    ▼
                            6. antennasActivated++ (1/3, 2/3, 3/3)
                                    │
                                    ▼
                            7. Se todas ativadas → CompleteQuest("quest_antenas")

================================================================================
    ASSINATURAS DOS PRINCIPAIS MÉTODOS
================================================================================

// ----- CurrencyManager -----
void AddCoins(int amount)              // Adiciona moedas
bool SpendCoins(int amount)            // Gasta moedas (retorna sucesso)
bool HasEnoughCoins(int amount)        // Verifica saldo
int GetCoins()                         // Retorna total

// ----- GameManager -----
void StartMinigameForQuest(string questName, MinigameType type)
void OnMinigameComplete(bool success, int coinsEarned)
void CancelMinigame()

// ----- QuestManager -----
void StartQuest(string name, int requiredProgress)
void CompleteQuest(string name)
void UpdateQuestProgress(string name, int amount)
bool IsQuestCompleted(string name)

// ----- AntennaManager -----
bool ActivateAntenna(QuestTrigger trigger)
bool CanActivateAntenna()
int GetActivatedCount()
string GetProgressString()              // "2/3"

================================================================================
    EVENTOS DISPONÍVEIS PARA UI E OUTROS SISTEMAS
================================================================================

// CurrencyManager
OnCoinsChanged(int totalCoins)
OnCoinsAdded(int amount)
OnCoinsSpent(int amount)
OnInsufficientFunds()

// GameManager
OnMinigameStarted(MinigameType type)
OnMinigameCompleted(MinigameType type, bool success)
OnQuestCompletedByMinigame(string questName)

// QuestManager
OnQuestStarted(string questName)
OnQuestCompleted(string questName)
OnQuestProgressUpdated(string name, int current, int total)

// AntennaManager
OnAntennaActivated(int current, int total)
OnAllAntennasActivated()
OnAntennaProgress(int percentage)

================================================================================
*/

// Este arquivo é apenas documentação e não precisa compilar
// Ele serve como referência para configurar o projeto no Unity
