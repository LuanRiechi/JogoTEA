using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    // Posicao correta da peca
    public Vector3 targetPosition;
    // Distancia para o "snap" (grudar no lugar)
    public float snapDistance = 0.4f;
    
    private bool isDragging = false;
    private bool isLocked = false;
    private Vector3 offset;
    private PuzzleGameManager gameManager;
    private SpriteRenderer spriteRenderer;
    private Collider2D pieceCollider;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        pieceCollider = GetComponent<Collider2D>();
    }

    void Start()
    {
        if (gameManager == null)
            gameManager = Object.FindAnyObjectByType<PuzzleGameManager>();
            
        // No inicio, as pecas ficam um pouco menores para facilitar a visualizacao
        // transform.localScale = Vector3.one * 0.9f; 
    }

    void Update()
    {
        // Se ja esta travada, nao faz nada
        if (isLocked) return;

        if (isDragging)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            transform.position = mousePos + offset;
        }
    }

    private void OnMouseDown()
    {
        // Bloqueio extra para garantir que pecas travadas nao sejam selecionadas
        if (isLocked || !enabled) return;

        isDragging = true;
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset.z = 0;
        
        // Traz a peca para a frente de tudo durante o arraste
        if (spriteRenderer != null)
            spriteRenderer.sortingOrder = 20;
            
        // Efeito visual de selecao (aumenta um pouco)
        transform.localScale = Vector3.one * 1.05f;
    }

    private void OnMouseUp()
    {
        if (isLocked) return;

        isDragging = false;
        
        // Volta ao tamanho normal e ordem de camada padrao
        transform.localScale = Vector3.one;
        if (spriteRenderer != null)
            spriteRenderer.sortingOrder = 5;

        // Verifica se esta na posicao correta
        // Note: targetPosition e unica para cada peca, garantindo que ela so encaixe no seu lugar
        if (Vector3.Distance(transform.position, targetPosition) < snapDistance)
        {
            SnapIntoPlace();
        }
    }

    void SnapIntoPlace()
    {
        isLocked = true;
        isDragging = false;
        
        // Posicionamento exato
        transform.position = targetPosition;
        transform.localScale = Vector3.one;

        // DESABILITA O COLLIDER: Isso impede que a peca seja clicada novamente
        if (pieceCollider != null) pieceCollider.enabled = false;

        // Ajusta a ordem para ficar no fundo (ja que esta encaixada)
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 0;
            // Opcional: Efeito visual de "confirmado" (ex: piscar verde ou ficar 100% opaco)
            spriteRenderer.color = Color.white; 
        }

        if (gameManager == null) gameManager = Object.FindAnyObjectByType<PuzzleGameManager>();
        if (gameManager != null)
        {
            gameManager.PiecePlaced();
        }
    }

    public void SetTarget(Vector3 pos)
    {
        targetPosition = pos;
    }
}
