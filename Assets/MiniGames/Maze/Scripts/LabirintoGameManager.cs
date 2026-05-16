using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LabirintoGameManager : MonoBehaviour
{
    // Configuracao estatica para passar entre cenas
    public static int FaseSelecionada = 1;
    public static bool jogoFinalizado = false;

    // Referencia para o painel de sucesso (vitoria)
    public GameObject painelSucesso;
    public TextMeshProUGUI textoTempoFinal;
    public TextMeshProUGUI textoTempoReal;
    
    // Referencias para os mapas das fases
public GameObject[] mapasFases;
    
    private float tempoInicio;

    // Qual fase estamos (para salvar no progresso)
    private int numeroDaFase = 1;

    void Start()
    {
        jogoFinalizado = false;
        numeroDaFase = FaseSelecionada;

        // Garante que o painel comece escondido
        if (painelSucesso != null) painelSucesso.SetActive(false);
        
        // Ativa apenas o mapa da fase selecionada
        ConfigurarMapa();

        // Marca o tempo de inicio
        tempoInicio = Time.time;
        }

        void Update()
        {
        if (!jogoFinalizado)
        {
            float tempoDecorrido = Time.time - tempoInicio;
            if (textoTempoReal != null)
            {
                int minutos = Mathf.FloorToInt(tempoDecorrido / 60);
                int segundos = Mathf.FloorToInt(tempoDecorrido % 60);
                textoTempoReal.text = string.Format("Tempo: {0:00}:{1:00}", minutos, segundos);
            }
        }
        }

        void ConfigurarMapa()
{
        if (mapasFases == null || mapasFases.Length == 0) return;

        for (int i = 0; i < mapasFases.Length; i++)
        {
            if (mapasFases[i] != null)
            {
                mapasFases[i].SetActive(i == (numeroDaFase - 1));
            }
        }
    }

    // Chamado quando o jogador chega na area final
    public void FinalizarJogo()
    {
        if (jogoFinalizado) return;
        
        try 
        {
            jogoFinalizado = true;
            float tempoTotal = Time.time - tempoInicio;

            // Salva o progresso do aluno no sistema global
            DataManager.SalvarProgresso("Labirinto", numeroDaFase, true, tempoTotal);

            // Ativa a interface de vitoria
            if (painelSucesso != null)
            {
                painelSucesso.SetActive(true);
                if (textoTempoFinal != null)
                {
                    int minutos = Mathf.FloorToInt(tempoTotal / 60);
                    int segundos = Mathf.FloorToInt(tempoTotal % 60);
                    textoTempoFinal.text = string.Format("Parabens!\nTempo: {0:00}:{1:00}", minutos, segundos);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erro ao finalizar jogo: " + e.Message);
            // Pelo menos tenta mostrar o painel se a vitoria falhar no salvamento
            if (painelSucesso != null) painelSucesso.SetActive(true);
        }
    }

    // Volta para o menu principal
    public void VoltarAoMenu()
    {
        try 
        {
            Debug.Log("Voltando ao menu principal...");
            SceneManager.LoadScene("MenuInicial");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erro ao voltar ao menu: " + e.Message);
        }
    }
}
