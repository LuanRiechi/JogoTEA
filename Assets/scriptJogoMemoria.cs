using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class scriptJogoMemoria : MonoBehaviour
{
    public int colunas;
    public GameObject carta;
    public Sprite[] cartas;
    public bool mostrarDica;
    public bool podeJogar;
    public TextMeshProUGUI textoTempo;
    public TextMeshProUGUI textoPontos;
    public TextMeshProUGUI textoAlerta;
    public GameObject painelFinal;
    public TextMeshProUGUI textoTempoFinal;
    public TextMeshProUGUI textoParesFinal;

    int jogadas;
    int pontos;
    float tempoDecorrido;
    bool jogoFinalizado;
    GameObject[] cartasClicadas;
    scripCarta[] listaCartas;

    ArrayList listaNumerosRandom = new ArrayList();

    private void Start()
    {
        cartasClicadas = new GameObject[2];

        float larguraCarta = 3.0f;
        float alturaCarta = 4.0f;

        if (painelFinal != null) painelFinal.SetActive(false);
        if (textoAlerta != null) textoAlerta.gameObject.SetActive(false);

        AtualizarInterface();

        Camera.main.transform.position = new Vector3(colunas *
        larguraCarta / 2.0f - larguraCarta / 2.0f,
        cartas.Length * 2 / colunas * alturaCarta / 2.0f - alturaCarta / 2.0f,
        -1.0f);

        for (int i = 0; i < cartas.Length * 2; i++)
        {
            int indiceRandom = Random.Range(0,
            listaNumerosRandom.Count);
            int indice = (int)(Mathf.Floor(i / 2.0f));
            listaNumerosRandom.Insert(indiceRandom, indice);
        }

        for (int i = 0; i < cartas.Length * 2; i++)
        {
            GameObject cartaNova = Instantiate(carta);
            cartaNova.GetComponent<scripCarta>().indiceCarta = (int)listaNumerosRandom[i];
            cartaNova.gameObject.transform.position = new Vector3(i % colunas * larguraCarta, Mathf.Floor(i / colunas) * alturaCarta, 0);
        }

        listaCartas = Object.FindObjectsByType<scripCarta>(FindObjectsInactive.Include);

        if (mostrarDica)
        {
            Invoke("MostrarCartas", 1.0f);
        }
        else
        {
            podeJogar = true;
        }
    }

    private void Update()
    {
        if (podeJogar && !jogoFinalizado)
        {
            tempoDecorrido += Time.deltaTime;
            ExibirTempo();
        }
    }

    void ExibirTempo()
    {
        if (textoTempo != null)
        {
            textoTempo.text = FormatarTempo(tempoDecorrido);
        }
    }

    string FormatarTempo(float tempo)
    {
        int minutos = Mathf.FloorToInt(tempo / 60);
        int segundos = Mathf.FloorToInt(tempo % 60);
        return string.Format("{0:00}:{1:00}", minutos, segundos);
    }

    void AtualizarInterface()
    {
        if (textoPontos != null)
        {
            textoPontos.text = "Pares: " + pontos;
        }
        ExibirTempo();
    }

    public void ClicouCarta(GameObject carta)
    {
        if (jogadas >= 2) return;
        
        cartasClicadas[jogadas] = carta;
        jogadas++;
        if (jogadas > 1)
        {
            Invoke("JogouSegundaCarta", 2.0f);
            podeJogar = false;
        }
    }

    public void JogouSegundaCarta()
    {
        podeJogar = true;
        if (cartasClicadas[0] != null && cartasClicadas[1] != null &&
            cartasClicadas[0].GetComponent<scripCarta>().indiceCarta ==
            cartasClicadas[1].GetComponent<scripCarta>().indiceCarta)
        {
            pontos++;
            AtualizarInterface();
            MostrarAlertaPar();
            Destroy(cartasClicadas[0]);
            Destroy(cartasClicadas[1]);
            VerificaFimDeJogo();
        }
        else
        {
            if (cartasClicadas[0] != null) cartasClicadas[0].GetComponent<Animator>().Play("AnimacaoFecha");
            if (cartasClicadas[1] != null) cartasClicadas[1].GetComponent<Animator>().Play("AnimacaoFecha");

            if (cartasClicadas[0] != null) cartasClicadas[0].GetComponent<scripCarta>().VoltaTexturaCartaVerso();
            if (cartasClicadas[1] != null) cartasClicadas[1].GetComponent<scripCarta>().VoltaTexturaCartaVerso();
        }
        jogadas = 0;
        cartasClicadas = new GameObject[2];
    }

    void MostrarAlertaPar()
    {
        if (textoAlerta != null)
        {
            textoAlerta.gameObject.SetActive(true);
            textoAlerta.text = "Par Encontrado!";
            CancelInvoke("EsconderAlerta");
            Invoke("EsconderAlerta", 1.5f);
        }
    }

    void EsconderAlerta()
    {
        if (textoAlerta != null) textoAlerta.gameObject.SetActive(false);
    }

    public void MostrarCartas()
    {
        foreach (scripCarta c in listaCartas)
        {
            if (c != null) c.MostrarCartasNoInicio();
        }
        Invoke("PodeJogar", 5.0f);
    }

    public int numeroDaFase = 1;

    public void VerificaFimDeJogo()
    {
        if (pontos == cartas.Length)
        {
            jogoFinalizado = true;
            DataManager.SalvarProgresso("JogoMemoria", numeroDaFase, true, tempoDecorrido);

            if (painelFinal != null)
            {
                painelFinal.SetActive(true);
                if (textoTempoFinal != null) textoTempoFinal.text = "Tempo: " + FormatarTempo(tempoDecorrido);
                if (textoParesFinal != null) textoParesFinal.text = "Pares: " + pontos;

                var aluno = DataManager.GetAlunoAtivo();
                if (aluno != null)
                {
                    var prog = aluno.progressos.Find(p => p.miniGame == "JogoMemoria" && p.fase == numeroDaFase);
                    if (prog != null && prog.melhorTempo == tempoDecorrido)
                    {
                        if (textoTempoFinal != null) textoTempoFinal.text += " (NOVO RECORDE!)";
                    }
                }
            }
        }
    }

    public void VoltarAoMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuInicial");
    }

    public void PodeJogar()
    {
        podeJogar = true;
    }
}


