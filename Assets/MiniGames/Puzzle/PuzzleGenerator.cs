using UnityEngine;
using System.Collections.Generic;

public class PuzzleGenerator : MonoBehaviour
{
    [Header("Configuracao do Puzzle")]
    public int rows = 2;
    public int columns = 3;

    [Header("Imagens do Puzzle")]
    public Sprite[] puzzleSprites;

    [Header("Referencias")]
    public GameObject piecePrefab;

    private Sprite selectedSprite;

    public void GeneratePuzzle()
    {
        ConfigurarDificuldade();
        LoadRandomSprite();

        if (selectedSprite == null)
        {
            Debug.LogError("Nenhuma imagem foi selecionada para o puzzle.");
            return;
        }

        CreatePuzzleGuide();
        SliceAndCreatePieces();
    }

    void ConfigurarDificuldade()
    {
        int fase = PuzzleGameManager.FaseSelecionada;

        switch (fase)
        {
            case 1: rows = 2; columns = 2; break;
            case 2: rows = 2; columns = 3; break;
            case 3: rows = 2; columns = 5; break;
            default: rows = 2; columns = 2; break;
        }

        Debug.Log($"Configurando Dificuldade: Fase {fase} ({rows}x{columns})");
    }

    void LoadRandomSprite()
    {
        if (puzzleSprites == null || puzzleSprites.Length == 0)
        {
            Debug.LogError("Nenhuma imagem foi atribuida no array Puzzle Sprites no Inspector.");
            selectedSprite = null;
            return;
        }

        selectedSprite = puzzleSprites[Random.Range(0, puzzleSprites.Length)];

        Debug.Log("Imagem selecionada para o puzzle: " + selectedSprite.name);
    }

    void CreatePuzzleGuide()
    {
        GameObject guide = new GameObject("PuzzleGuide_Background");
        guide.transform.SetParent(transform);
        guide.transform.localPosition = Vector3.zero;

        SpriteRenderer sr = guide.AddComponent<SpriteRenderer>();
        sr.sprite = selectedSprite;
        sr.color = new Color(1, 1, 1, 0.15f);
        sr.sortingOrder = -2;
    }

    void SliceAndCreatePieces()
    {
        Texture2D tex = selectedSprite.texture;
        float pieceWidth = tex.width / columns;
        float pieceHeight = tex.height / rows;

        int totalPiecesCount = rows * columns;

        var manager = Object.FindAnyObjectByType<PuzzleGameManager>();
        if (manager != null)
        {
            manager.totalPieces = totalPiecesCount;
        }

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                Rect rect = new Rect(c * pieceWidth, r * pieceHeight, pieceWidth, pieceHeight);

                Sprite pieceSprite = Sprite.Create(
                    tex,
                    rect,
                    new Vector2(0.5f, 0.5f),
                    selectedSprite.pixelsPerUnit
                );

                GameObject pieceObj = Instantiate(piecePrefab, transform);
                pieceObj.name = $"Piece_{r}_{c}";

                SpriteRenderer sr = pieceObj.GetComponent<SpriteRenderer>();
                sr.sprite = pieceSprite;
                sr.sortingOrder = 5;

                if (pieceObj.GetComponent<Collider2D>() == null)
                {
                    pieceObj.AddComponent<BoxCollider2D>();
                }

                float posX = (c - (columns / 2f) + 0.5f) * (pieceWidth / selectedSprite.pixelsPerUnit);
                float posY = (r - (rows / 2f) + 0.5f) * (pieceHeight / selectedSprite.pixelsPerUnit);
                Vector3 targetPos = new Vector3(posX, posY, 0);

                PuzzlePiece pieceScript = pieceObj.GetComponent<PuzzlePiece>();
                pieceScript.SetTarget(targetPos);

                float side = (Random.value > 0.5f) ? 1f : -1f;
                float spawnX = side * Random.Range(8.5f, 11.0f);
                float spawnY = Random.Range(-6.0f, 6.0f);

                pieceObj.transform.localPosition = new Vector3(spawnX, spawnY, 0);
            }
        }
    }
}
