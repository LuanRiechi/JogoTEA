# Documentação Técnica Oficial: JogoTEA

## 1. Introdução
O **JogoTEA** é uma plataforma educacional modular desenvolvida em Unity, projetada para auxiliar no desenvolvimento cognitivo de crianças com Transtorno do Espectro Autista (TEA). O projeto prioriza acessibilidade, feedback visual claro e persistência de dados para acompanhamento de progresso pedagógico.

O sistema é estruturado em três camadas fundamentais:
*   **Apresentação (UI)**: Construída com uGUI e TextMesh Pro, focada em interfaces limpas e previsíveis.
*   **Lógica de Jogo (Managers)**: Controladores de cena que gerenciam o fluxo, cronômetros e regras de vitória.
*   **Persistência (Dados)**: Engine SQL local para armazenamento resiliente de perfis e métricas de desempenho.

---

## 2. Estrutura do Projeto

*   **Engine**: Unity 6000.4.5f1 (Unity 6). Plataforma alvo: Windows.
*   **Cenas** (ordem no Build Settings): `MenuInicial` → `JogoMemoria` → `PuzzleGame` → `JogoLabirinto`.

```
Assets/
├── Scenes/                  # As 4 cenas do jogo
├── Scripts/                 # Núcleo do sistema
│   ├── DataManager.cs       # Fachada de dados (cache + delegação)
│   ├── SQLiteService.cs     # Camada de acesso ao SQLite
│   ├── DatabaseModels.cs    # Entidades (namespace JogoTEA.Persistence)
│   ├── MenuController.cs    # Navegação e gestão de alunos no menu
│   └── PauseMenuController.cs # Menu de pausa das cenas de jogo
├── MiniGames/
│   ├── Maze/                # Scripts, Sprites, Prefabs e Audio do labirinto
│   └── Puzzle/              # Scripts, PuzzlePiece.prefab e Sprites/
├── Plugins/                 # sqlite3.dll, Mono.Data.Sqlite.dll, System.Data.dll
├── scriptJogoMemoria.cs     # Manager do jogo da memória (raiz de Assets)
└── scripCarta.cs            # Comportamento da carta (raiz de Assets)
```

---

## 3. Arquitetura e Fluxo de Dados

O projeto adota uma arquitetura orientada a **Managers**, com separação clara entre lógica de jogo, interface e persistência.

### A. Gestão de Dados Centralizada (`DataManager.cs`)
Classe estática que atua como o ponto central (Facade) para todas as operações de dados.
*   **Cache em Memória**: A propriedade `Data` carrega todo o banco (alunos, progressos e configurações) para um objeto `GameData` na primeira leitura. O cache é **invalidado** (`_cachedData = null`) após criação de aluno, deleção de aluno e salvamento de progresso, forçando recarregamento na próxima leitura. `SelecionarAluno` é a exceção: atualiza o campo do cache diretamente, sem invalidá-lo.
*   **Integridade**: `SalvarProgresso` só persiste se houver um aluno selecionado no banco (`AppConfig`). `DeletarAluno` limpa a chave `alunoSelecionado` caso o aluno removido fosse o ativo.
*   **Legado**: O método `Salvar()` é vazio e mantido apenas por compatibilidade — o SQLite persiste automaticamente a cada operação.

### B. Camada SQL (`SQLiteService.cs`)
Responsável pela comunicação direta com o arquivo `jogotea.db`, criado em `Application.persistentDataPath` na primeira execução.
*   **Esquema de Tabelas** (criadas via `CREATE TABLE IF NOT EXISTS`):
    *   `Alunos`: `Id` (PK autoincremento) e `Nome` (`UNIQUE NOT NULL`). A unicidade do nome é garantida pelo banco: `CreateAluno` retorna `false` em caso de duplicata.
    *   `Progressos`: `AlunoId` (FK com `ON DELETE CASCADE`), `MiniGame`, `Fase`, `Completou` e `MelhorTempo`.
    *   `AppConfig`: pares Chave/Valor para estado global (atualmente apenas `alunoSelecionado`), gravados com `INSERT OR REPLACE`.
*   **Validação de Recorde**: Implementada em `SaveProgresso`. Se já existe registro para o trio (aluno, mini-game, fase), o `MelhorTempo` só é atualizado quando o registro anterior não tinha tempo válido (`<= 0`) ou quando o novo tempo é menor que o recorde atual. Caso contrário, apenas `Completou` é atualizado e o recorde é preservado.
*   **Segurança**: Todas as consultas usam parâmetros (`SqliteParameter`), evitando SQL injection.

### C. Navegação e UI (`MenuController.cs`)
Orquestra a transição entre painéis no menu inicial (todos na mesma cena, alternados via `SetActive`).
*   **Fluxo de Painéis**: Principal → Seleção de Jogo → Seleção de Dificuldade → Cena de Jogo. Há ainda os painéis de Opções, Lista de Alunos e Detalhes do Aluno.
*   **Regra de Acesso**: Os botões de início de jogo (lista `gameButtons`) só ficam interativos quando há um aluno ativo (`ValidarJogos` seta `interactable`).
*   **Gestão de Alunos**: Cadastro via `TMP_InputField` (o aluno recém-criado é selecionado automaticamente); a lista de alunos é populada dinamicamente instanciando `prefabBotaoAluno`; o painel de detalhes exibe o progresso formatado (mini-game, fase, status e melhor tempo `mm:ss`) e permite selecionar ou deletar o aluno.
*   **Seleção de Jogo**: O jogo escolhido é guardado no enum privado `TipoJogo` (`Memoria`, `Labirinto`, `Puzzle`). `SelecionarFase(int fase)` grava a fase na variável estática do manager correspondente e carrega a cena via `SceneManager.LoadScene`.
*   **Resiliência**: A inicialização do banco no `Start` é envolvida em `try/catch` — se as DLLs do SQLite faltarem, o menu não trava.

---

## 4. Estrutura de Dados e Modelos

Os dados são persistidos no namespace `JogoTEA.Persistence` utilizando classes mapeadas para o SQLite.

### Entidades (`DatabaseModels.cs`):
1.  **`AlunoEntity`**: Define o perfil do estudante (Id, Nome).
2.  **`ProgressoEntity`**: Registro histórico de desempenho (AlunoId, MiniGame, Fase, Completou, MelhorTempo).
3.  **`ConfigEntity`**: Persistência de estado da aplicação (Chave, Valor). *Observação: definida no modelo, mas o `SQLiteService` lê/grava as configurações diretamente como strings.*

### Identificadores no banco:
Cada mini-game grava seu progresso com uma string fixa em `Progressos.MiniGame`:

| Mini-game | Valor gravado |
|---|---|
| Jogo da Memória | `"JogoMemoria"` |
| Labirinto | `"Labirinto"` |
| Quebra-Cabeça | `"JogoPuzzle"` |

---

## 5. Detalhamento dos Mini-Games

Todos os mini-games seguem o mesmo contrato: recebem a dificuldade pela variável estática `FaseSelecionada` (definida pelo menu antes do `LoadScene`), cronometram a partida e, na vitória, chamam `DataManager.SalvarProgresso(nome, fase, true, tempo)` e exibem um painel de conclusão.

### A. Jogo da Memória (`scriptJogoMemoria.cs`)
*   **Dificuldade** (`ConfigurarFase`):

    | Fase | Pares | Grade |
    |---|---|---|
    | 1 (Fácil) | 2 | 2x2 |
    | 2 (Médio) | 4 | 4x2 |
    | 3 (Difícil) | 8 | 4x4 |

*   **Geração da Grade**: Os índices dos pares são embaralhados por inserção em posições aleatórias de um `ArrayList`. As cartas são instanciadas em grade (espaçamento 3.0 x 4.0 unidades) e a câmera é reposicionada dinamicamente para centralizar a grade de cada fase.
*   **Ciclo de Jogada**: `ClicouCarta` armazena até 2 cartas viradas. Na segunda, o jogo bloqueia a interação (`podeJogar = false`) e agenda `JogouSegundaCarta` após 2s: se os índices coincidem, incrementa pontos, mostra o alerta "Par Encontrado!" (1,5s) e **destrói** as duas cartas; se não, dispara a animação de fechamento e restaura o verso.
*   **Carta (`scripCarta.cs`)**: Guarda o sprite do verso no `Start`, e no `OnMouseDown` (se ainda não virada e `podeJogar`) toca a animação de flip e troca a textura após 0,5s, sincronizada com a animação. Cartas viradas ficam bloqueadas até serem resolvidas.
*   **Dica Inicial**: Se o bool `mostrarDica` estiver marcado no Inspector, 1s após o início todas as cartas são reveladas e, após 5s, são ocultadas e o jogo é liberado. Se desmarcado, o jogo libera imediatamente.
*   **Cronômetro**: Acumula `Time.deltaTime` apenas enquanto `podeJogar` e o jogo não terminou (a espera da dica inicial não conta tempo).
*   **Vitória**: Quando `pontos == totalPares`, salva o progresso e exibe o painel final com tempo e pares. Se o tempo salvo virou o `melhorTempo` do aluno, anexa "(NOVO RECORDE!)" ao texto.

### B. Jogo de Labirinto (`LabirintoGameManager.cs`)
*   **Mapas por Fase**: A cena contém os mapas das 3 fases no array `mapasFases`; no `Start`, apenas o mapa de índice `FaseSelecionada - 1` é ativado.
*   **Cronômetro**: `tempoInicio = Time.time` é marcado no `Start` — o tempo conta a partir do carregamento da cena, e é exibido em `mm:ss` a cada frame até a vitória.
*   **Física (`LabirintoPlayerController.cs`)**: `Rigidbody2D` com `freezeRotation = true` (não rotaciona ao colidir com paredes) e `gravityScale = 0` (movimento livre top-down). O input é lido no `Update` via `Input.GetAxisRaw` (WASD ou setas — Input Manager clássico) e aplicado no `FixedUpdate` com `rb.linearVelocity = movimento.normalized * velocidade` (API do Unity 6). A normalização mantém a velocidade constante nas diagonais.
*   **Vitória**: O jogador (trigger 2D) detecta a área de chegada pela tag `"Finish"` e chama `FinalizarJogo`, que salva o progresso, congela o movimento (flag estática `jogoFinalizado`) e exibe o painel de sucesso. O salvamento é protegido por `try/catch` para o painel aparecer mesmo em caso de erro de banco.

### C. Jogo de Quebra-Cabeça (`PuzzleGameManager.cs`)
*   **Inicialização**: O manager agenda `IniciarGeracao` com 0,1s de atraso para garantir que o `PuzzleGenerator` esteja pronto, e acompanha `piecesPlacedCount / totalPieces` na UI junto do cronômetro.
*   **Dificuldade e Geração (`PuzzleGenerator.cs`)**:

    | Fase | Grade | Peças |
    |---|---|---|
    | 1 (Fácil) | 2x2 | 4 |
    | 2 (Médio) | 2x3 | 6 |
    | 3 (Difícil) | 2x5 | 10 |

    A imagem da partida é **sorteada do array `puzzleSprites`, configurado no Inspector** (os arquivos ficam organizados em `Assets/MiniGames/Puzzle/Sprites`, mas é o array que define quais entram no sorteio). A textura é fatiada em tempo de execução via `Sprite.Create`, cada peça recebe sua `targetPosition` calculada na grade central, e as peças nascem espalhadas aleatoriamente nas laterais da tela (x entre ±8,5 e ±11).
*   **Guia Visual**: `CreatePuzzleGuide` instancia a imagem completa ao fundo com 15% de opacidade (`sortingOrder -2`), reduzindo a carga cognitiva durante a montagem.
*   **Interação (`PuzzlePiece.cs`)**:
    *   **Arraste**: `OnMouseDown` captura a peça com offset em relação ao mouse, traz para a frente (`sortingOrder 20`) e aumenta a escala em 5% como feedback de seleção; a peça segue o mouse no `Update`; `OnMouseUp` restaura escala e ordem.
    *   **Snap**: Ao soltar, se `Vector3.Distance` até a `targetPosition` for menor que `snapDistance` (0,4), `SnapIntoPlace` fixa a peça na posição exata, **desabilita o collider** (impedindo novo clique) e notifica o manager via `PiecePlaced`.
*   **Vitória**: Quando todas as peças estão travadas, salva o progresso e exibe o painel de sucesso, com opções de reiniciar a cena ou voltar ao menu.

---

## 6. Sistemas Auxiliares

### Gerenciamento de Áudio
*   **Implementação**: O estado de "Mudo" é controlado globalmente através de `AudioListener.pause`.
*   **Persistência**: A preferência é salva em `PlayerPrefs` (chave `"Mudo"`, independente do banco de dados) e aplicada no `Start` do `MenuController`. Como `AudioListener.pause` é um estado global da engine, ele permanece válido nas trocas de cena durante a sessão. O botão de mudo existe tanto no menu de opções quanto no menu de pausa, e ambos gravam no mesmo `PlayerPrefs`.

### Menu de Pausa (`PauseMenuController.cs`)
Presente nas cenas de jogo:
*   **Pausa**: Alterna `Time.timeScale` entre 0 e 1 e exibe/oculta o painel — cronômetros e físicas param junto.
*   **Botões**: Retomar (despausa e continua do mesmo ponto), Som (alterna mudo global) e Menu Inicial (restaura `Time.timeScale = 1` antes de carregar a cena `MenuInicial`; o progresso da partida em andamento é descartado).

### Sistema de Dificuldade
*   Cada mini-game possui 3 níveis (Fase 1 = Fácil, 2 = Médio, 3 = Difícil).
*   O nível selecionado no menu é transmitido às cenas por variáveis estáticas em cada manager (`scriptJogoMemoria.FaseSelecionada`, `LabirintoGameManager.FaseSelecionada`, `PuzzleGameManager.FaseSelecionada`).

---

## 7. Guia de Manutenção e Expansão

### Requisitos Técnicos:
*   **Unity 6.0+** (projeto criado na 6000.4.5f1): necessário para a propriedade `linearVelocity` do `Rigidbody2D`.
*   **Plugins**: `sqlite3.dll`, `Mono.Data.Sqlite.dll` e `System.Data.dll` em `Assets/Plugins`.

### Como adicionar um novo Mini-Game:
1.  **Criação da Cena**: Desenvolva a cena do jogo de forma independente, com uma variável estática `FaseSelecionada` no manager, e adicione-a ao Build Settings.
2.  **Integração de Dados**: Ao finalizar a fase, chame `DataManager.SalvarProgresso("NomeDoJogo", fase, true, tempo)`.
3.  **Registro no Menu (`MenuController.cs`)**:
    *   Adicione o novo tipo ao enum `TipoJogo`;
    *   Crie o método público `JogarNovoJogo()` (seta `jogoAtual` e chama `AbrirDificuldade()`) e vincule-o ao botão do painel de seleção;
    *   Adicione o ramo correspondente em `SelecionarFase()` (setar a estática e carregar a cena);
    *   Adicione o botão à lista `gameButtons` no Inspector para a validação automática de aluno ativo.

### Como adicionar imagens ao Puzzle:
1.  Importe o `.png` para `Assets/MiniGames/Puzzle/Sprites` com Texture Type **Sprite (2D and UI)**;
2.  No Inspector do objeto que contém o `PuzzleGenerator` (cena `PuzzleGame`), adicione o sprite ao array **`puzzleSprites`** — apenas colocar o arquivo na pasta não o inclui no sorteio.

---

## 8. Localização de Dados
*   **Banco de Dados**: `%USERPROFILE%\AppData\LocalLow\[Empresa]\JogoTEA\jogotea.db` (resolvido em runtime por `Application.persistentDataPath`), garantindo persistência entre atualizações do app.
*   **Preferência de Áudio**: `PlayerPrefs`.
*   **Cache**: Invalidado automaticamente nas operações de escrita (criação/deleção de aluno e salvamento de progresso) para garantir integridade entre banco e interface.
