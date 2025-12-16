/*
================================================================================
    GUIA DE CONFIGURAÇÃO - Sistema MiniRace (Corrida de Kart)
    Claro Party Games - ProjetoFinal
================================================================================

Este documento descreve como configurar o sistema de inicialização do MiniRace
para funcionar integrado com o ecossistema do jogo.

================================================================================
    ESTRUTURA DO SISTEMA
================================================================================

    ┌─────────────────────────────────────────────────────────────────┐
    │                    MINI RACE ENTRY POINT                         │
    │  - Detecta quando jogador se aproxima                           │
    │  - Aguarda pressionar E para entrar                             │
    │  - Teleporta/ativa o minigame                                   │
    │  - Gerencia entrada/saída                                       │
    └─────────────────────────────────────────────────────────────────┘
                                    │
            ┌───────────────────────┼───────────────────────┐
            ▼                       ▼                       ▼
    ┌───────────────┐      ┌───────────────┐      ┌───────────────┐
    │MINI RACE AREA │      │MINI RACE      │      │   CHECKPOINTS  │
    │ (GameObject)  │      │  SCORING      │      │ & FINISH LINE │
    │ - Pista       │      │ - Tempo       │      │ - Registram    │
    │ - Obstáculos  │      │ - Voltas      │      │   progresso    │
    │ - Cenário     │      │ - Moedas      │      │ - Validam volta│
    └───────────────┘      └───────────────┘      └───────────────┘

================================================================================
    PASSO 1: CRIAR O PONTO DE ENTRADA NA CIDADE
================================================================================

1. Criar um GameObject na cidade onde o jogador inicia a corrida
   Nome sugerido: "MiniRaceEntryPoint"

2. Adicionar componentes:
   - Box Collider (Is Trigger: ✓)
   - Script: MiniRaceEntryPoint

3. Configurar no Inspector:

    [Player References]
    ├── Player Transform: → [arrastar o Player]
    ├── Player Character Controller: → [CharacterController do Player]
    └── Player Game Object: → [GameObject do Player]

    [MiniRace References]
    ├── Mini Race Area: → [GameObject pai da área do MiniRace]
    ├── Car Game Object: → [O carro que o jogador controla]
    ├── Race Start Point: → [Transform do ponto de largada]
    └── Mini Race Canvas: → [Canvas/UI do MiniRace]

    [Exit References]
    └── City Return Point: → [Transform onde o jogador volta após corrida]

    [Scoring]
    └── Race Scoring: → [MiniRaceScoring]

    [Visual Feedback]
    ├── Highlight Color: Cyan (ou outra cor)
    └── Interaction Prompt: → [UI "Pressione E"]

    [Settings]
    ├── Hide Player During Race: ✓
    └── Require Confirmation: ✓

================================================================================
    PASSO 2: CONFIGURAR O SISTEMA DE PONTUAÇÃO
================================================================================

1. Criar um GameObject dentro da MiniRaceArea
   Nome: "RaceScoring"

2. Adicionar script: MiniRaceScoring

3. Configurar no Inspector:

    [UI References]
    ├── Timer Display: → [TextMeshProUGUI para tempo]
    ├── Lap Display: → [TextMeshProUGUI para voltas]
    ├── Position Display: → [TextMeshProUGUI para posição] (opcional)
    └── Coins Display: → [TextMeshProUGUI para moedas]

    [Race Settings]
    ├── Total Laps: 3
    └── Total Checkpoints: 5 (por volta)

    [Time Bonus Thresholds]
    ├── Time For 3 Stars: 60 (segundos)
    ├── Time For 2 Stars: 90
    └── Time For 1 Star: 120

    [Coin Rewards]
    ├── Coins For 3 Stars: 300
    ├── Coins For 2 Stars: 200
    ├── Coins For 1 Star: 100
    ├── Coins For Completion: 50
    └── Coins Per Checkpoint: 10

    [References]
    └── Entry Point: → [MiniRaceEntryPoint]

================================================================================
    PASSO 3: CONFIGURAR OS CHECKPOINTS
================================================================================

Para cada checkpoint na pista:

1. Criar GameObject com:
   - Box Collider (Is Trigger: ✓)
   - Script: CheckPoint

2. Configurar no Inspector:

    [Settings]
    ├── Is Finish Line: ☐ (deixar desmarcado)
    └── Checkpoint Index: 0, 1, 2... (ordem)

    [References]
    └── Race Scoring: → [MiniRaceScoring]

    [Visual]
    ├── Destroy On Pass: ✓
    └── Pass Effect: → [prefab de partículas] (opcional)

================================================================================
    PASSO 4: CONFIGURAR A LINHA DE CHEGADA
================================================================================

1. Criar GameObject na linha de largada/chegada:
   - Box Collider (Is Trigger: ✓)
   - Script: MiniRaceFinishLine

2. Configurar no Inspector:

    [References]
    ├── Race Scoring: → [MiniRaceScoring]
    └── Entry Point: → [MiniRaceEntryPoint]

    [Requirements]
    └── Minimum Checkpoints: 3 (ou mais)

    [Visual]
    ├── Finish Effect: → [prefab de partículas]
    └── Finish Sound: → [audio clip]

================================================================================
    PASSO 5: CONFIGURAR O CAMERA MANAGER
================================================================================

No CameraManager, adicionar a câmera do MiniRace:

    [Mini Race Camera]
    └── Mini Race Cinemachine Camera: → [CinemachineCamera do carro]

================================================================================
    HIERARQUIA SUGERIDA
================================================================================

    Hierarchy/
    ├── [CITY]
    │   ├── MiniRaceEntryPoint          ← Script: MiniRaceEntryPoint
    │   │   └── InteractionPrompt       ← UI "Pressione E"
    │   └── CityReturnPoint             ← Transform vazio
    │
    ├── [MINIRACE]                       ← Desativado no início
    │   ├── RaceScoring                 ← Script: MiniRaceScoring
    │   ├── Track
    │   │   ├── Checkpoint_1            ← Script: CheckPoint
    │   │   ├── Checkpoint_2            ← Script: CheckPoint
    │   │   ├── Checkpoint_3            ← Script: CheckPoint
    │   │   └── FinishLine              ← Script: MiniRaceFinishLine
    │   ├── StartPoint                  ← Transform (ponto de largada)
    │   └── Car                         ← O carro do jogador
    │
    └── [UI]
        └── MiniRaceCanvas              ← Desativado no início
            ├── TimerText
            ├── LapText
            └── CoinsText

================================================================================
    FLUXO DO MINIGAME
================================================================================

    1. Jogador se aproxima do MiniRaceEntryPoint
                    │
                    ▼
    2. UI "Pressione E" aparece (Interaction Prompt)
                    │
                    ▼
    3. Jogador pressiona E
                    │
                    ▼
    4. MiniRaceEntryPoint.EnterMiniRace():
       - Esconde o jogador da cidade
       - Ativa a área do MiniRace
       - Posiciona o carro no StartPoint
       - Ativa o Canvas/UI
       - Troca para a câmera do carro (CameraManager)
       - Inicia o MiniRaceScoring.StartRace()
                    │
                    ▼
    5. CORRIDA EM ANDAMENTO:
       - Timer conta o tempo
       - Checkpoints registram progresso
       - Moedas são coletadas
                    │
                    ▼
    6. Jogador cruza a FinishLine após N voltas
                    │
                    ▼
    7. MiniRaceScoring.CompleteRace():
       - Calcula estrelas baseado no tempo
       - Calcula moedas totais (checkpoints + bônus)
       - Chama MiniRaceEntryPoint.CompleteMiniRace(moedas)
                    │
                    ▼
    8. MiniRaceEntryPoint.CompleteMiniRace():
       - Adiciona moedas ao CurrencyManager
       - Notifica GameManager.OnMinigameComplete()
       - Chama ExitMiniRace()
                    │
                    ▼
    9. MiniRaceEntryPoint.ExitMiniRace():
       - Desativa área do MiniRace
       - Reativa o jogador
       - Teleporta para CityReturnPoint
       - Restaura câmera da cidade

================================================================================
    INTEGRAÇÃO COM QUESTS
================================================================================

Para usar o MiniRace como requisito de quest:

No QuestTrigger:
    [Quest Settings]
    ├── Quest Name: "quest_corrida"
    └── Completion Mode: RequiresMinigame

    [Minigame Settings]
    └── Minigame Type: MiniRace

Assim, quando o jogador tocar no QuestTrigger:
1. GameManager.StartMinigameForQuest() é chamado
2. O MiniRace é iniciado
3. Ao completar, a quest é automaticamente marcada como completa

================================================================================
*/

// Este arquivo é apenas documentação
