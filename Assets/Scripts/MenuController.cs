using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class MenuController : MonoBehaviour
{
    [Header("Paineis")]
    public GameObject painelPrincipal;
    public GameObject painelSelecao;
    public GameObject painelOpcoes;
    public GameObject painelListaAlunos;
    public GameObject painelDetalhesAluno;

    [Header("Configuracoes")]
    public Toggle toggleMudo;

    [Header("Gestao de Alunos")]
    public TMP_InputField inputNomeAluno;
    public TextMeshProUGUI textoAlunoAtivo;
    public TextMeshProUGUI textoDetalhesAluno;
    public TextMeshProUGUI textoProgressoAluno;
    public Transform containerListaAlunos;
    public GameObject prefabBotaoAluno;
    public List<Button> gameButtons = new List<Button>();

    private string nomeAlunoSendoVisualizado;

    private void Start()
    {
        DataManager.Carregar();
        AbrirMenuPrincipal();
        AtualizarInterfaceAlunos();
        
        if (toggleMudo != null)
        {
            bool estaMudo = PlayerPrefs.GetInt("Mudo", 0) == 1;
            toggleMudo.isOn = estaMudo;
            AudioListener.pause = estaMudo;
            toggleMudo.onValueChanged.AddListener(SetMudo);
        }
    }

    public void AbrirMenuPrincipal()
    {
        painelPrincipal.SetActive(true);
        painelSelecao.SetActive(false);
        painelOpcoes.SetActive(false);
        if (painelListaAlunos != null) painelListaAlunos.SetActive(false);
        if (painelDetalhesAluno != null) painelDetalhesAluno.SetActive(false);
    }

    public void AbrirSelecaoFase()
    {
        painelPrincipal.SetActive(false);
        painelSelecao.SetActive(true);
        painelOpcoes.SetActive(false);
        if (painelListaAlunos != null) painelListaAlunos.SetActive(false);
        if (painelDetalhesAluno != null) painelDetalhesAluno.SetActive(false);
        AtualizarInterfaceAlunos();
    }

    public void AbrirOpcoes()
    {
        painelPrincipal.SetActive(false);
        painelSelecao.SetActive(false);
        painelOpcoes.SetActive(true);
        if (painelListaAlunos != null) painelListaAlunos.SetActive(false);
        if (painelDetalhesAluno != null) painelDetalhesAluno.SetActive(false);
    }

    public void AbrirListaAlunos()
    {
        if (painelListaAlunos != null)
        {
            painelListaAlunos.SetActive(true);
            PopularListaAlunos();
        }
    }

    public void FecharListaAlunos()
    {
        if (painelListaAlunos != null) painelListaAlunos.SetActive(false);
    }

    public void AbrirDetalhesAluno(string nome)
    {
        nomeAlunoSendoVisualizado = nome;
        if (painelDetalhesAluno != null)
        {
            painelDetalhesAluno.SetActive(true);
            if (textoDetalhesAluno != null) textoDetalhesAluno.text = "Aluno: " + nome;
            
            Aluno aluno = DataManager.Data.alunos.FirstOrDefault(a => a.nome == nome);
            if (aluno != null && textoProgressoAluno != null)
            {
                string info = "Progresso:\n";
                if (aluno.progressos == null || aluno.progressos.Count == 0)
                {
                    info += "Nenhum jogo concluido.";
                }
                else
                {
                    foreach (var p in aluno.progressos)
                    {
                        info += $"- {p.miniGame} (Fase {p.fase}): {(p.completou ? "Concluido" : "Em andamento")}\n";
                        if (p.melhorTempo > 0) info += $"  Melhor Tempo: {FormatarTempo(p.melhorTempo)}\n";
                    }
                }
                textoProgressoAluno.text = info;
            }
        }
    }

    private string FormatarTempo(float tempo)
    {
        int minutos = Mathf.FloorToInt(tempo / 60);
        int segundos = Mathf.FloorToInt(tempo % 60);
        return string.Format("{0:00}:{1:00}", minutos, segundos);
    }

    public void FecharDetalhesAluno()
    {
        if (painelDetalhesAluno != null) painelDetalhesAluno.SetActive(false);
    }

    public void ConfirmarSelecaoAluno()
    {
        SelecionarAluno(nomeAlunoSendoVisualizado);
        FecharDetalhesAluno();
    }

    public void DeletarAlunoVisualizado()
    {
        DataManager.DeletarAluno(nomeAlunoSendoVisualizado);
        AtualizarInterfaceAlunos();
        PopularListaAlunos();
        FecharDetalhesAluno();
    }

    public void CadastrarAluno()
    {
        if (inputNomeAluno != null && !string.IsNullOrEmpty(inputNomeAluno.text))
        {
            if (DataManager.CriarAluno(inputNomeAluno.text))
            {
                DataManager.SelecionarAluno(inputNomeAluno.text);
                inputNomeAluno.text = "";
                AtualizarInterfaceAlunos();
            }
        }
    }

    private void PopularListaAlunos()
    {
        if (containerListaAlunos == null || prefabBotaoAluno == null) return;

        // Limpar lista atual
        foreach (Transform child in containerListaAlunos)
        {
            Destroy(child.gameObject);
        }

        // Criar botoes para cada aluno
        if (DataManager.Data != null && DataManager.Data.alunos != null)
        {
            foreach (var aluno in DataManager.Data.alunos)
            {
                if (aluno == null) continue;

                GameObject novoBotao = Instantiate(prefabBotaoAluno, containerListaAlunos);
                novoBotao.SetActive(true); // Garante que o item esta visivel
                
                // Procura por TMP ou Text
                var tmp = novoBotao.GetComponentInChildren<TextMeshProUGUI>();
                if (tmp != null) tmp.text = aluno.nome;
                else {
                    var txt = novoBotao.GetComponentInChildren<Text>();
                    if (txt != null) txt.text = aluno.nome;
                }

                string nomeDoAluno = aluno.nome;
                novoBotao.GetComponent<Button>().onClick.AddListener(() => {
                    AbrirDetalhesAluno(nomeDoAluno);
                });
            }
        }
    }

    public void SelecionarAluno(string nome)
    {
        DataManager.SelecionarAluno(nome);
        AtualizarInterfaceAlunos();
        FecharListaAlunos();
    }

    private void AtualizarInterfaceAlunos()
    {
        if (DataManager.Data == null) return;

        if (!string.IsNullOrEmpty(DataManager.Data.alunoSelecionado))
        {
            if (textoAlunoAtivo != null) textoAlunoAtivo.text = "Aluno Ativo: " + DataManager.Data.alunoSelecionado;
        }
        else
        {
            if (textoAlunoAtivo != null) textoAlunoAtivo.text = "Selecione ou Cadastre um Aluno";
        }

        ValidarJogos();
    }

    private void ValidarJogos()
    {
        bool temAluno = !string.IsNullOrEmpty(DataManager.Data.alunoSelecionado);
        foreach (var btn in gameButtons)
        {
            if (btn != null) btn.interactable = temAluno;
        }
    }

    public void JogarMemoria()
    {
        SceneManager.LoadScene("JogoMemoria");
    }

    public void SetMudo(bool mudo)
    {
        AudioListener.pause = mudo;
        PlayerPrefs.SetInt("Mudo", mudo ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SairDoJogo()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
