# Documentação Técnica: JogoTEA

## 1. Descrição do Projeto
**JogoTEA** é um projeto educacional desenvolvido em Unity para apoiar crianças com Transtorno do Espectro Autista (TEA). O projeto apresenta mini-games focados no desenvolvimento cognitivo, incluindo um Jogo da Memória e um Jogo de Labirinto.

## 2. Persistência de Dados
O sistema utiliza um motor **Híbrido de Persistência**:
*   **Modo SQLite**: Ativado automaticamente se a `sqlite3.dll` for encontrada.
*   **Modo Fallback (JSON)**: Ativado caso o SQLite falhe, garantindo que o progresso seja salvo mesmo sem as bibliotecas nativas.

## 3. Correções de Bugs
*   **Som e Interface**: A inicialização do áudio foi movida para o início do `Start`, evitando que erros no banco de dados "quebrem" o botão de mudo ou façam o texto sumir.
*   **Gestão de Alunos**: Ao deletar o aluno selecionado, o sistema agora limpa a seleção global, bloqueando os mini-games até que um novo perfil seja ativado.


## 2. Fluxo de Jogo / Ciclo do Usuário
1.  **Inicialização e Autenticação**: O jogo começa no `MenuInicial`. Os usuários devem selecionar um perfil de aluno existente ou criar um novo para desbloquear o acesso aos mini-games.
2.  **Seleção de Mini-Game**: Uma vez que um aluno está ativo, o usuário pode escolher entre o Jogo da Memória (`JogoMemoria`) ou o Jogo de Labirinto (`JogoLabirinto`).
3.  **Ciclo Principal (Mini-Games)**:
    *   **Jogo da Memória**: Selecionar dificuldade (3 níveis), virar cartas para encontrar pares e completar o conjunto.
    *   **Jogo de Labirinto**: Navegar um personagem (Dinossauro) através de um labirinto 2D para chegar à linha de chegada.
4.  **Conclusão e Feedback**: Ao terminar um jogo, um painel de "Sucesso" exibe o tempo de conclusão e atualiza os dados persistentes do aluno no banco de dados se um novo recorde for alcançado.
5.  **Retorno/Sair**: O usuário pode retornar ao menu principal para trocar de aluno, ver relatórios de progresso ou sair do aplicativo.

## 3. Arquitetura
O projeto segue uma arquitetura baseada em **Managers** com uma separação clara entre persistência de dados e lógica de jogo.
*   **Persistência de Dados**: Gerenciada pelo `DataManager` (estático) e pelo `SQLiteService`, que utiliza um banco de dados SQLite local (`jogotea.db`) para armazenamento robusto e relacional.
*   **Gestão de Cenas**: `MenuController` atua como o principal orquestrador para transições de UI e carregamento de cenas.
*   **Isolamento de Mini-Games**: Cada mini-game opera em sua própria cena com um Manager dedicado (`scriptJogoMemoria`, `LabirintoGameManager`) que se comunica com o `DataManager` apenas na conclusão do jogo.
*   **Fluxo de Dados**: `UI -> MenuController -> DataManager (Estático) <-> SQLiteService <-> Banco de Dados (.db)`.

## 4. Sistemas do Jogo e Conceitos de Domínio

### Sistema de Gestão de Alunos
*   `DataManager`: Classe estática que serve como fachada para o cache de dados (`GameData`) e operações de alto nível.
*   `SQLiteService`: Serviço responsável pela lógica de baixo nível do SQLite, incluindo criação de tabelas, comandos SQL e gestão de conexões.
*   `Aluno`: Classe de dados que representa um aluno.
*   `FaseProgresso`: Armazena estatísticas específicas (nome do jogo, índice do nível, status de conclusão, melhor tempo).
*   **Localização**: `Assets/Scripts/DataManager.cs`, `Assets/Scripts/SQLiteService.cs` e `Assets/Scripts/DatabaseModels.cs`.

### Sistema de Jogo da Memória
*   `scriptJogoMemoria`: Lógica principal para embaralhamento de cartas, combinação de pares e escala de dificuldade (Grades de 2x2 a 4x4).
*   `scripCarta`: Comportamento individual da carta, lidando com eventos de clique, animações e troca de texturas.
*   **Localização**: Pasta raiz `Assets/`.

### Sistema de Navegação em Labirinto
*   `LabirintoPlayerController`: Gerencia o movimento 2D usando `Rigidbody2D.linearVelocity` (Unity 6) e detecção de entrada.
*   `LabirintoGameManager`: Monitora o tempo e detecta o gatilho de "Fim" para concluir o nível.
*   **Localização**: `Assets/MiniGames/Maze/Scripts/`.

## 5. Visão Geral das Cenas
*   `MenuInicial`: O ponto de entrada. Contém o registro de alunos, seleção de perfil e o hub de navegação principal.
*   `JogoMemoria`: Uma cena dinâmica que ajusta sua grade e contagem de cartas baseada na variável estática `FaseSelecionada`.
*   `JogoLabirinto`: Um nível de navegação 2D top-down.

## 6. Sistema de UI
O projeto utiliza **Unity UI (uGUI)** combinado com **TextMesh Pro**.
*   **Painéis de Menu**: Gerenciados via ativação/desativação de `GameObject` no `MenuController`.
*   **Listas Dinâmicas**: O método `PopularListaAlunos` instancia prefabs de botões em um layout vertical para exibir o banco de dados de alunos.
*   **UI de Feedback**: Mini-games usam Canvases de sobreposição para mensagens de "Sucesso" e cronômetros em tempo real.

## 7. Ativos e Modelo de Dados
*   **Persistência**: Os dados são armazenados em `jogotea.db` localizado em `Application.persistentDataPath`.
*   **Bibliotecas**: Utiliza `Mono.Data.Sqlite.dll` para suporte a banco de dados relacional no Windows.
*   **Convenção de Nomenclatura**: Mistura de Inglês e Português (ex: `DataManager.cs` vs `scripCarta.cs`).

## 8. Notas, Cuidados e Dicas (Gotchas)
*   **Dependência de Variável Estática**: O Jogo da Memória depende de `scriptJogoMemoria.FaseSelecionada`. Certifique-se de definir isso antes de carregar a cena.
*   **SQLite no Editor**: O sistema utiliza DLLs em `Assets/Plugins`. Se houver erros de "DllNotFound", verifique se as DLLs do SQLite estão presentes para a plataforma alvo.
*   **Bloqueio de Mini-Games**: Os mini-games são desativados no menu (`interactable = false`) se nenhum aluno estiver selecionado. A exclusão do aluno ativo limpa automaticamente a seleção para manter a integridade.
*   **Integridade de Dados**: Ao deletar um aluno, todos os seus progressos são removidos automaticamente do banco devido à restrição `ON DELETE CASCADE`.
