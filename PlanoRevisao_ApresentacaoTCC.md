# Plano de Revisão — Defesa do TCC (JogoTEA)

Preparação para a apresentação do TCC2 "Serious Game para Desenvolvimento Cognitivo de Crianças com TEA" (UTFPR-CP, Engenharia de Software).

---

## 1. O que esperar da defesa na UTFPR

Formato regulamentado pela UTFPR (Regulamento de TCC dos Cursos de Graduação + normas complementares de cada curso):

*   **Banca**: no mínimo 3 membros — o orientador (Prof. Eduardo Cotrin Teixeira), que preside a sessão, + 2 avaliadores com afinidade ao tema. A folha de aprovação da sua v3 já prevê 3 membros.
*   **Dinâmica**: sessão pública → exposição oral do aluno → arguição pelos membros da banca → deliberação.
*   **Tempo**: varia por curso/campus. O padrão nos cursos da UTFPR fica entre **20 e 25 min de apresentação**, com **arguição de ~10 min por membro** (em alguns cursos até 20). **Confirme com seu orientador/PRATCC o tempo exato em Cornélio Procópio** e prepare a apresentação para caber com folga (se forem 20 min, mire em 17–18).
*   **Avaliação em duas frentes**: trabalho escrito + apresentação oral (domínio do conteúdo, clareza, capacidade de responder à arguição). Média ≥ 6,0 aprova.
*   **Logística**: entregar a versão do texto à banca com antecedência (10–15 dias, conforme a norma local); após a defesa, incorporar as correções sugeridas e submeter a versão final (repositório RIUT).
*   **Em ES/computação é comum (e bem-visto) demonstrar o sistema funcionando** — TCCs de serious games na UTFPR tipicamente mostram o jogo ao vivo ou em vídeo.

---

## 2. Estrutura sugerida da apresentação (~18 min)

1.  **Contexto e motivação** (2–3 min): TEA, ausência de cura, urgência de intervenções precoces.
2.  **Problema e objetivo** (1–2 min): recitar o objetivo com precisão — *jogo digital em fases como apoio às intervenções, focado no aprimoramento do aprendizado e da resolução de problemas* (alvo 2 de Bosa).
3.  **Fundamentação enxuta** (2–3 min): os 4 alvos de Bosa (2006), definição de serious game, jogos digitais na educação infantil e TEA.
4.  **Metodologia** (3 min): Scrum adaptado a projeto individual, ordem dos incrementos, tecnologias (Unity/C#/SQLite), diagramas (ER, casos de uso, atividades).
5.  **Desenvolvimento + DEMO** (5–6 min): arquitetura em 3 camadas, gestão de alunos, os 3 minigames com dificuldades — demo ao vivo ou vídeo.
6.  **Considerações finais** (2 min): limitações, desafios técnicos superados, trabalhos futuros.

---

## 3. Revisão da MONOGRAFIA (por capítulo)

### Cap. 1 — Introdução
- [ ] Definição de TEA segundo **CID-11 e DSM-5** (a banca pode pedir para explicar as siglas — estão na lista de siglas da v3).
- [ ] O que é o método **ABA** e por que é citado (reforço de bons comportamentos com recompensas).
- [ ] Citação de Bosa sobre preditores de desfecho (nível cognitivo, linguagem, habilidades adaptativas).
- [ ] Delimitação: jogo 2D em fases; **objetivo** (memorizar a frase exata da seção 1.3).

### Cap. 2 — Fundamentação Teórica
- [ ] **Os 4 alvos de Bosa (2006)** — saber de cor; seu trabalho ataca o alvo 2.
- [ ] **TEACCH** (organização do ambiente + pistas visuais) — conecte com o guia fantasma do puzzle e o feedback visual do jogo.
- [ ] Definição de **serious game** (Savi; Ulbricht, 2008) e diferença para jogo comum: objetivo pedagógico definido.
- [ ] Benefícios dos jogos digitais: atenção e flexibilidade cognitiva (Ramos; Segundo, 2018); papel do professor (Silva, 2010); CTS e inclusão (Silva; Artuso; Tortato, 2020).

### Cap. 3 — Metodologia
- [ ] Justificativa **científica de cada minigame** (pergunta muito provável):
    - Memória → atenção e memória visual (Mori et al., 2017);
    - Labirinto → planejamento, antecipação e tomada de decisão (Macedo; Carvalho; Petty, 2009);
    - Quebra-cabeça → habilidades espaciais e rotação mental (Levine et al., 2012).
- [ ] **Temática de dinossauros** → interesses restritos e intensos do DSM-5 + estratégia pedagógica (Dias; Costa, 2024).
- [ ] **Scrum adaptado**: por que "inspirado" e não Scrum pleno — papéis acumulados por 1 dev, sem Daily; o que foi aproveitado: incrementos sucessivos + avaliação ao fim de cada ciclo. Duração: ~2 meses.
- [ ] **Ordem dos incrementos**: memória → menus/seleção de aluno → puzzle → labirinto → fases de dificuldade → banco de dados.
- [ ] Testes: pelo desenvolvedor a cada incremento + disponibilização para terceiros usarem (percepções orientaram ajustes) → build final Windows.
- [ ] Por que **Unity** (2D/3D, C#, documentação, Asset Store, multiplataforma) e por que **SQLite** (embarcado, sem servidor, arquivo único, facilita instalação).
- [ ] **Diagramas**: saber explicar o ER (Alunos 1—N Progressos + AppConfig), o de casos de uso e os de atividade (fluxo geral e do jogo da memória). A banca costuma pedir para "ler" um diagrama.

### Cap. 4 — Desenvolvimento
- [ ] Fluxo completo de telas: menu inicial (Iniciar/Configurações/Sair) → menu principal → lista/detalhes de aluno → seleção de jogo → dificuldade → gameplay → fase concluída.
- [ ] Regras de negócio: sem login; **minigames bloqueados sem aluno ativo**; aluno recém-cadastrado é selecionado automaticamente; último aluno selecionado persiste entre execuções.
- [ ] Números exatos das dificuldades (a banca confere com o texto):
    - Memória: 2 / 4 / 8 pares (4, 8 e 16 cartas);
    - Puzzle: **4 / 6 / 10 peças** (corrigido na v3);
    - Labirinto: mapas maiores e com mais bifurcações por nível.
- [ ] Menu de pausa: pausa congela o jogo, retomar continua do mesmo ponto, sair perde o progresso da partida.
- [ ] Tela de fase concluída com desempenho + mensagem de novo recorde.
- [ ] Assets: 2D Casual UI HD, 371 Simple Buttons Pack, Casual & Relaxing Game Music Pack.

### Cap. 5 — Considerações Finais
- [ ] **Limitação central** (pergunta quase certa): o jogo **não foi validado com crianças com TEA nem com profissionais** — o trabalho se limita a compilar atividades em jogo digital; o uso é a critério dos profissionais. Tenha uma resposta pronta e tranquila para "por que não validou?" e aponte como trabalho futuro.
- [ ] Desafios técnicos relatados: integração do SQLite na Unity (DLLs) e fatiamento da imagem em runtime no puzzle.
- [ ] Lições: desenvolvimento incremental com teste a cada funcionalidade.

---

## 4. Revisão do SISTEMA (perguntas técnicas de banca de ES)

Use o README atualizado como guia de estudo. Pontos quentes:

### Arquitetura
- [ ] 3 camadas: UI (uGUI/TMP) → Managers → Persistência.
- [ ] **`DataManager` como Facade estático** com cache em memória invalidado nas escritas. Saber justificar: acesso global simples para um jogo pequeno; alternativa seria singleton/injeção de dependência.
- [ ] Fluxo de um salvamento: minigame → `DataManager.SalvarProgresso` → busca aluno ativo no `AppConfig` → `SQLiteService.SaveProgresso` → invalidação do cache.

### Banco de dados
- [ ] Esquema das 3 tabelas; `Nome UNIQUE`; **`ON DELETE CASCADE`** (o que acontece ao deletar um aluno); `INSERT OR REPLACE` no AppConfig.
- [ ] **Validação de recorde** no `SaveProgresso`: atualiza `MelhorTempo` só se não havia tempo válido ou se o novo tempo é menor.
- [ ] Consultas parametrizadas (`SqliteParameter`) → prevenção de SQL injection.
- [ ] Por que SQLite e não JSON/PlayerPrefs para o progresso: integridade relacional, consultas, cascata, escalabilidade de relatórios.

### Decisões da Unity (esteja pronto para "por que assim e não X?")
- [ ] **Passagem de dados entre cenas por variáveis estáticas** (`FaseSelecionada`): simples e suficiente; alternativas que você conhece: `DontDestroyOnLoad`, ScriptableObjects, PlayerPrefs.
- [ ] **Labirinto**: input no `Update` (`GetAxisRaw`), física no `FixedUpdate` com `rb.linearVelocity` (API Unity 6); `movimento.normalized` → velocidade constante na diagonal; `freezeRotation` e `gravityScale = 0`; chegada por trigger com tag `"Finish"`.
- [ ] **Puzzle**: `Sprite.Create` fatia a textura em runtime; peças nascem nas laterais; snap por `Vector3.Distance < 0.4`; peça encaixada tem o collider desabilitado (não sai mais do lugar); guia fantasma com 15% de opacidade.
- [ ] **Memória**: embaralhamento por inserção aleatória; ciclo de 2 cartas com `Invoke` de 2s; pares são destruídos; dica inicial de ~5s (ativada na cena); cronômetro não conta durante a dica.
- [ ] **Pausa**: `Time.timeScale = 0` congela físicas e cronômetros baseados em `Time.deltaTime`/`Time.time`.
- [ ] **Áudio**: `AudioListener.pause` global + persistência em `PlayerPrefs` (chave "Mudo").

### ⚠️ Inconsistência a resolver ANTES da defesa
A monografia (seção do SQLite) diz que o banco guarda **"se o som está ligado ou desligado"**, mas o código salva isso em **PlayerPrefs** — o banco só guarda o aluno selecionado. Ou ajuste a frase no texto, ou prepare a resposta honesta: "o texto simplificou; na implementação final a preferência de som ficou em PlayerPrefs por ser um dado de dispositivo, não de aluno".

---

## 5. Perguntas prováveis da banca (ensaie respostas curtas)

1.  **"O jogo foi testado com crianças com TEA?"** → Não; limitação declarada no Cap. 5. Testes foram do desenvolvedor + terceiros (usabilidade geral). Validação clínica/pedagógica é trabalho futuro com profissionais.
2.  **"Por que essas 3 atividades e não outras?"** → Base na literatura (Mori, Macedo, Levine) + alvo 2 de Bosa.
3.  **"O que torna o jogo adequado a TEA especificamente?"** → Previsibilidade das interfaces, feedback visual claro, dica visual (TEACCH), temática de interesse (dinossauros), dificuldade progressiva, música relaxante, opção de mudo (sensibilidade sonora).
4.  **"Como o profissional acompanha o progresso?"** → Relatório por aluno: fase, status e melhor tempo por minigame, persistidos em SQLite.
5.  **"Por que Windows e não Android/tablet?"** → Escopo do TCC; arquitetura permite portar (Unity multiplataforma), input de mouse/teclado precisaria virar touch — trabalho futuro.
6.  **"Houve testes automatizados?"** → Não; testes manuais incrementais a cada funcionalidade. Reconheça o valor de testes de unidade na camada de dados como melhoria.
7.  **"E os dados das crianças (LGPD)?"** → Armazenamento 100% local, sem rede, apenas o primeiro nome é cadastrado; sem coleta de dados sensíveis.
8.  **"Serious game vs. gamificação?"** → Serious game é um jogo completo com objetivo pedagógico; gamificação aplica elementos de jogo em contexto não-jogo.
9.  **"Como adicionar um novo minigame?"** → Cena independente + `SalvarProgresso` + registro no menu (enum, método, botão) — mostre que a arquitetura foi pensada para expansão.
10. **"Por que Scrum se você era um só?"** → Resposta já está na v3: aproveitou incrementos e avaliação contínua; papéis e cerimônias de equipe não se aplicavam.

---

## 6. Demo — checklist

- [ ] Build final testada **no computador/projetor da sala** (ou notebook próprio + adaptador).
- [ ] Banco pré-populado com 1–2 alunos com progresso (para mostrar o relatório sem perder tempo cadastrando tudo ao vivo).
- [ ] Roteiro de 3–5 min: cadastrar aluno novo (mostra bloqueio/desbloqueio dos botões) → jogar memória fácil com dica inicial → mostrar tela de conclusão com recorde → abrir relatório do aluno → mostrar rapidamente puzzle e labirinto.
- [ ] **Plano B obrigatório**: vídeo gravado da gameplay completa, caso o ao-vivo falhe.
- [ ] Verificar som do ambiente (ou deixar mudo e avisar que há música relaxante).
- [ ] Deletar dados de teste estranhos do banco (`%USERPROFILE%\AppData\LocalLow\LuanRiechi\JogoTEA\jogotea.db`).

---

## 7. Cronograma sugerido

| Quando | O quê |
|---|---|
| D-7 | Reler a monografia inteira (v3) marcando frases-chave; montar slides |
| D-5 | Revisar Caps. 2–3 (autores e justificativas) + diagramas; treinar explicação do ER e casos de uso |
| D-4 | Revisar o código com o README ao lado (seção 4 deste plano); resolver a inconsistência do som |
| D-3 | 1º ensaio cronometrado completo (apresentação + demo) |
| D-2 | Ensaiar respostas da seção 5 em voz alta; 2º ensaio cronometrado |
| D-1 | Testar build/vídeo no equipamento; ensaio final leve; descansar |
| D-0 | Chegar cedo, testar projetor e som, respirar |

---

*Confirme com o orientador/PRATCC: tempo oficial de apresentação e arguição em CP, se a demo ao vivo é permitida/esperada, e o prazo de entrega do texto à banca.*
