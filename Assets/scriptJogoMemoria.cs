using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptJogoMemoria : MonoBehaviour
{
    public int colunas;
    public GameObject carta;
    public Sprite[] cartas;
    public bool mostrarDica;
    public bool podeJogar;
    GameObject textoFimDeJogo;
    int jogadas;
    int pontos;
    GameObject[] cartasClicadas;
    scripCarta[] listaCartas;

    ArrayList listaNumerosRandom = new ArrayList();



    private void Start()
    {
        cartasClicadas = new GameObject[2];

        float larguraCarta = 3.0f;
        float alturaCarta = 4.0f;

        //textoFimDeJogo = GameObject.Find("TEXTO FIM DE JOGO");
        //textoFimDeJogo.gameObject.SetActive(false);

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

        for (int i = 0; i< cartas.Length*2; i++)
        {
            GameObject cartaNova = Instantiate(carta);
            cartaNova.GetComponent<scripCarta>().indiceCarta = (int) listaNumerosRandom[i];
            cartaNova.gameObject.transform.position = new Vector3(i%colunas*larguraCarta, Mathf.Floor(i / colunas)*alturaCarta , 0);
        }

        listaCartas = FindObjectsOfType<scripCarta>();

        if (mostrarDica)
        {
            Invoke("MostrarCartas", 1.0f);
        }
        else
        {
            podeJogar = true;
        }
    }

    public void ClicouCarta(GameObject carta)
    {
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
        if (cartasClicadas[0].GetComponent<scripCarta>().indiceCarta ==
        cartasClicadas[1].GetComponent<scripCarta>().indiceCarta)
        {
            pontos++;
            Destroy(cartasClicadas[0]);
            Destroy(cartasClicadas[1]);
            VerificaFimDeJogo();
        }
        else
        {
            cartasClicadas[0].GetComponent<Animator>().Play("AnimacaoFecha"
            );
            cartasClicadas[1].GetComponent<Animator>().Play("AnimacaoFecha"
            );

            cartasClicadas[0].GetComponent<scripCarta>().VoltaTexturaCartaVerso();
            cartasClicadas[1].GetComponent<scripCarta>().VoltaTexturaCartaVerso();
        }
        jogadas = 0;
        cartasClicadas = new GameObject[2];
    }

    public void MostrarCartas()
    {
        foreach (scripCarta c in listaCartas)
        {
            c.MostrarCartasNoInicio();
        }
        Invoke("PodeJogar", 5.0f);
    }

    public void VerificaFimDeJogo()
    {
        if (pontos == cartas.Length)
        {
            //textoFimDeJogo.gameObject.SetActive(true);
        }
    }
     public void PodeJogar()
    {
        podeJogar = true;
    }
}
