using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PuzzleGameManager : MonoBehaviour
{
    public static int FaseSelecionada = 1;

    [Header("UI")]
public GameObject painelSucesso;
    public TextMeshProUGUI textoTempo;
    public TextMeshProUGUI textoProgresso;
    
    [Header("Configuracao")]
public int totalPieces;
    private int piecesPlacedCount = 0; // Nome alterado para evitar conflitos
    private float tempoDecorrido;
    private bool jogoFinalizado = false;

    void Start()
    {
        // Reset de estado ao iniciar
        piecesPlacedCount = 0;
        jogoFinalizado = false;
        tempoDecorrido = 0;
        
        if (painelSucesso != null) painelSucesso.SetActive(false);
        
        // Pequeno atraso para garantir que o gerador esteja pronto
        Invoke("IniciarGeracao", 0.1f);
    }

    void IniciarGeracao()
    {
        PuzzleGenerator generator = Object.FindAnyObjectByType<PuzzleGenerator>();
        if (generator != null)
        {
            generator.GeneratePuzzle();
        }
    }

    void Update()
    {
        if (!jogoFinalizado)
        {
            tempoDecorrido += Time.deltaTime;
            AtualizarInterface();
        }
    }

    void AtualizarInterface()
    {
        if (textoTempo != null)
        {
            int minutos = Mathf.FloorToInt(tempoDecorrido / 60);
            int segundos = Mathf.FloorToInt(tempoDecorrido % 60);
            textoTempo.text = "Tempo: " + string.Format("{0:00}:{1:00}", minutos, segundos);
        }

        if (textoProgresso != null)
        {
            textoProgresso.text = string.Format("Progresso: {0}/{1}", piecesPlacedCount, totalPieces);
        }
    }

    public void PiecePlaced()
    {
        if (jogoFinalizado) return;

        piecesPlacedCount++;
        Debug.Log($"Progresso: {piecesPlacedCount} / {totalPieces}");

        // Condicao de vitoria: todas as pecas em seus lugares corretos
        if (piecesPlacedCount >= totalPieces && totalPieces > 0)
        {
            FinalizarJogo();
        }
    }

    void FinalizarJogo()
    {
        if (jogoFinalizado) return;
        
        jogoFinalizado = true;
        
        // Salva o progresso
        DataManager.SalvarProgresso("JogoPuzzle", FaseSelecionada, true, tempoDecorrido);

        // Ativa o painel de parabens
        if (painelSucesso != null)
        {
            painelSucesso.SetActive(true);
        }
    }

    public void VoltarAoMenu()
    {
        SceneManager.LoadScene("MenuInicial");
    }

    public void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
