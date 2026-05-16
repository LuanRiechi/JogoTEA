using UnityEngine;

public class LabirintoPlayerController : MonoBehaviour
{
    // Velocidade de movimento do dinossauro
    public float velocidade = 5.0f;

    private Rigidbody2D rb;
    private Vector2 movimento;

    void Start()
    {
        // Pega a referencia do componente Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        
        // Garante que o jogador nao rotacione ao bater nas paredes
        rb.freezeRotation = true;
        // Define a gravidade como zero para movimentacao livre 2D
        rb.gravityScale = 0;
    }

    void Update()
    {
        if (LabirintoGameManager.jogoFinalizado)
        {
            movimento = Vector2.zero;
            return;
        }

        // Captura o input do teclado (Setas ou WASD)
        // Usando o Unity Input Manager (Old System)
        movimento.x = Input.GetAxisRaw("Horizontal");
        movimento.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        if (LabirintoGameManager.jogoFinalizado)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Aplica o movimento no Rigidbody2D
        // Usando linearVelocity (Novo no Unity 6) ou velocity
        rb.linearVelocity = movimento.normalized * velocidade;
    }

    // Detecta se o jogador entrou na area de chegada
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Finish"))
        {
            // Busca o GameManager na cena e finaliza o jogo
            LabirintoGameManager gm = Object.FindAnyObjectByType<LabirintoGameManager>();
            if (gm != null)
            {
                gm.FinalizarJogo();
            }
        }
    }
}
