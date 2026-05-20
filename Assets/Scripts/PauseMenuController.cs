using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI do Menu")]
    public GameObject painelPausa;
    public Button botaoPausa;
    
    [Header("Botoes do Menu")]
    public Button botaoRetomar;
    public Button botaoMudo;
    public Button botaoMenuInicial;
    public TextMeshProUGUI textoBotaoMudo;

    private bool jogoPausado = false;

    void Start()
    {
        // Garante que o menu comece fechado e o tempo esteja normal
        if (painelPausa != null) painelPausa.SetActive(false);
        Time.timeScale = 1f;

        // Configura os eventos dos botoes
        if (botaoPausa != null) botaoPausa.onClick.AddListener(AlternarPausa);
        if (botaoRetomar != null) botaoRetomar.onClick.AddListener(RetomarJogo);
        if (botaoMudo != null) botaoMudo.onClick.AddListener(AlternarMudo);
        if (botaoMenuInicial != null) botaoMenuInicial.onClick.AddListener(VoltarAoMenuInicial);

        // Inicializa o estado do texto do botao de som
        AtualizarTextoMudo();
    }

    // Alterna o estado de pausa do jogo
    public void AlternarPausa()
    {
        jogoPausado = !jogoPausado;

        if (jogoPausado)
        {
            // Para o tempo do jogo
            Time.timeScale = 0f;
            if (painelPausa != null) painelPausa.SetActive(true);
        }
        else
        {
            // Retoma o tempo do jogo
            Time.timeScale = 1f;
            if (painelPausa != null) painelPausa.SetActive(false);
        }
    }

    // Função especifica para o botao de fechar o menu
    public void RetomarJogo()
    {
        jogoPausado = false;
        Time.timeScale = 1f;
        if (painelPausa != null) painelPausa.SetActive(false);
    }

    // Alterna o som global (Mudo)
    public void AlternarMudo()
    {
        // Inverte o estado atual do AudioListener
        bool estaMudo = !AudioListener.pause;
        AudioListener.pause = estaMudo;

        // Salva a preferencia do usuario
        PlayerPrefs.SetInt("Mudo", estaMudo ? 1 : 0);
        PlayerPrefs.Save();

        // Atualiza o texto do botao
        AtualizarTextoMudo();
    }

    // Atualiza o texto do botao de som baseado no estado atual
    private void AtualizarTextoMudo()
    {
        if (textoBotaoMudo != null)
        {
            textoBotaoMudo.text = AudioListener.pause ? "Som: Desligado" : "Som: Ligado";
        }
    }

    // Sai da cena atual e volta para o menu inicial
    public void VoltarAoMenuInicial()
    {
        // Garante que o tempo volte ao normal antes de sair
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuInicial");
    }
}
