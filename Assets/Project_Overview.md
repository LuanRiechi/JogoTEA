# Documentação Técnica Oficial: JogoTEA

## 1. Introdução
O **JogoTEA** é uma plataforma educacional modular desenvolvida em Unity, projetada para auxiliar no desenvolvimento cognitivo de crianças com Transtorno do Espectro Autista (TEA). O projeto prioriza acessibilidade, feedback visual claro e persistência de dados para acompanhamento de progresso pedagógico.

O sistema é estruturado em três camadas fundamentais:
*   **Apresentação (UI)**: Construída com uGUI e TextMesh Pro, focada em interfaces limpas e previsíveis.
*   **Lógica de Jogo (Managers)**: Controladores de cena que gerenciam o fluxo, cronômetros e regras de vitória.
*   **Persistência (Dados)**: Engine SQL local para armazenamento resiliente de perfis e métricas de desempenho.

---

## 2. Arquitetura e Fluxo de Dados

O projeto adota uma arquitetura orientada a **Managers**, com separação clara entre lógica de jogo, interface e persistência.

### A. Gestão de Dados Centralizada (`DataManager.cs`)
Atua como o ponto central (Facade) para todas as operações de dados.
*   **Sincronização**: Implementa um sistema de **Cache em Memória** (`GameData`) que é invalidado automaticamente após qualquer operação de escrita (Criação/Deleção de aluno ou Salvamento de progresso).
*   **Integridade**: Garante que os mini-games recebam apenas o aluno ativo e que recordes (melhor tempo) sejam validados antes de serem persistidos.

### B. Camada SQL (`SQLiteService.cs`)
Responsável pela comunicação direta com o arquivo `jogotea.db`.
*   **Esquema de Tabelas**:
    *   `Alunos`: ID único e Nome (Unique).
    *   `Progressos`: Registra mini-game, fase, estado de conclusão e o melhor tempo. Utiliza `ON DELETE CASCADE` para limpar dados quando um aluno é removido.
    *   `AppConfig`: Armazena variáveis globais de estado (ex: último aluno selecionado).
*   **Localização**: O banco é salvo em `Application.persistentDataPath`, garantindo persistência mesmo após atualizações do app.

### C. Navegação e UI (`MenuController.cs`)
Orquestra a transição entre painéis no menu inicial.
*   **Fluxo de Paineis**: Principal -> Seleção de Jogo -> Seleção de Dificuldade -> Cena de Jogo.
*   **Regra de Acesso**: Os botões de início de jogo são monitorados e tornam-se interativos apenas quando um aluno está "Ativo" no sistema via `gameButtons.interactable`.

---

## 3. Estrutura de Dados e Modelos

Os dados são persistidos no namespace `JogoTEA.Persistence` utilizando classes mapeadas para o SQLite.

### Entidades (`DatabaseModels.cs`):
1.  **`AlunoEntity`**: Define o perfil do estudante (Id, Nome).
2.  **`ProgressoEntity`**: Registro histórico de desempenho (AlunoId, MiniGame, Fase, Completou, MelhorTempo).
3.  **`ConfigEntity`**: Persistência de estado da aplicação (Chave, Valor).

---

## 4. Detalhamento dos Mini-Games

### A. Jogo da Memória (`scriptJogoMemoria.cs`)
*   **Dinâmica**: Geração de grade baseada no nível (2x2, 4x4, etc).
*   **Mecânicas**: 
    *   **`scripCarta.cs`**: Gerencia o estado da face da carta, disparando animações de flip.
    *   **Dica Inicial**: Sistema opcional que revela todas as cartas por 5 segundos no início da partida.
*   **Dificuldade**: Escala conforme a variável estática `FaseSelecionada`.

### B. Jogo de Labirinto (`LabirintoGameManager.cs`)
*   **Navegação**: 2D top-down focada em precisão motora.
*   **Física (`LabirintoPlayerController.cs`)**: Utiliza o novo sistema de física do Unity 6 (`linearVelocity`) para movimento fluido e constante, ignorando rotações indesejadas.
*   **Feedback**: Captura o tempo exato desde o movimento inicial até o trigger de chegada com a tag "Finish".

### C. Jogo de Quebra-Cabeça (Puzzle) (`PuzzleGameManager.cs`)
*   **Geração Dinâmica (`PuzzleGenerator.cs`)**: Sorteia imagens da pasta `Assets/MiniGames/Puzzle/Sprites` e as fatia em peças em tempo de execução via `Sprite.Create`.
*   **Interação (`PuzzlePiece.cs`)**:
    *   **Arraste**: Usa `OnMouseDown` para capturar a peça e `OnMouseUp` para soltar.
    *   **Snap**: Implementa um sistema de atração magnética quando a peça está próxima da sua `targetPosition`.
*   **Guia Visual**: Exibe uma imagem "fantasma" ao fundo para reduzir a carga cognitiva da criança durante a montagem.

---

## 5. Sistemas Auxiliares

### Gerenciamento de Áudio
*   **Implementação**: O estado de "Mudo" é controlado globalmente através do `AudioListener.pause`.
*   **Persistência**: Utiliza `PlayerPrefs` para garantir que a preferência de áudio seja aplicada instantaneamente no `Awake` de qualquer cena, independente do banco de dados.

### Sistema de Dificuldade
*   Cada mini-game possui 3 níveis de dificuldade (Fase 1, 2 e 3).
*   O nível selecionado no menu é transmitido para as cenas de jogo através de variáveis estáticas em cada Manager de jogo (ex: `scriptJogoMemoria.FaseSelecionada`).

---

## 6. Guia de Manutenção e Expansão

### Requisitos Técnicos:
*   **Unity 6.0+**: Necessário para suporte à propriedade `linearVelocity`.
*   **Plugins**: Requer `sqlite3.dll`, `Mono.Data.Sqlite.dll` e `System.Data.dll` em `Assets/Plugins`.

### Como adicionar um novo Mini-Game:
1.  **Criação da Cena**: Desenvolva a cena do jogo de forma independente.
2.  **Integração de Dados**: Ao finalizar a fase, utilize `DataManager.SalvarProgresso("NomeDoJogo", fase, true, tempo)`.
3.  **Registro no Menu**: 
    *   Adicione o novo tipo ao enum `TipoJogo` no `MenuController.cs`.
    *   Adicione o botão de início à lista `gameButtons` no Inspetor para validação automática de aluno ativo.
4.  **Imagens (Puzzle)**: Para adicionar novas imagens ao Puzzle, basta colocar arquivos `.png` em `Assets/MiniGames/Puzzle/Sprites`.

---

## 7. Localização de Dados
*   **Banco de Dados**: `%AppData%/LocalLow/[Empresa]/JogoTEA/jogotea.db`
*   **Cache**: O sistema invalida o cache automaticamente em cada operação de escrita para garantir integridade.

