using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scripCarta : MonoBehaviour
{
    Sprite cartaVerso;
    Sprite[] cartas;
    AudioClip locucao;
    Animator meuAnimador;
    public int indiceCarta;
    bool virou = false;
    // Start is called before the first frame update
    void Start()
    {
        cartas = GameObject.Find("MeuGameEngine").GetComponent<scriptJogoMemoria>().cartas;
        cartaVerso = GetComponent<SpriteRenderer>().sprite;
        meuAnimador = GetComponent<Animator>();
    }

    public void OnMouseDown()
    {
        bool podeJogar = GameObject.Find("MeuGameEngine").GetComponent<scriptJogoMemoria>().podeJogar;
        if (!virou && podeJogar)
        {
            virou = true;
            meuAnimador.Play("AnimacaoFecha");
            Invoke("MudaTexturaCarta", 0.5f);
            GameObject.Find("MeuGameEngine").GetComponent<scriptJogoMemoria>().ClicouCarta(this.gameObject);
        }

    }

    public void MudaTexturaCarta()
    {
        GetComponent<SpriteRenderer>().sprite = cartas[indiceCarta];
    }
    public void VoltaTexturaCartaVerso()
    {
        Invoke("MudaTexturaVerso", 0.5f);
    }
    public void MostrarCartasNoInicio()
    {
        meuAnimador.Play("AnimacaoFecha");
        Invoke("MudaTexturaCarta", 0.5f);
        Invoke("OcultarCartasNoInicio", 5.0f);
    }
    public void OcultarCartasNoInicio()
    {
        meuAnimador.Play("AnimacaoFecha");
        Invoke("MudaTexturaVerso", 1.0f);
    }
    public void MudaTexturaVerso()
    {
        GetComponent<SpriteRenderer>().sprite = cartaVerso;
        virou = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
