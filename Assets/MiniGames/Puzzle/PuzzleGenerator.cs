using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class PuzzleGenerator : MonoBehaviour
{
    [Header("Configuracao do Puzzle")]
    public int rows = 2;
    public int columns = 3;
    public string spritesPath = "Assets/MiniGames/Puzzle/Sprites";
    
    [Header("Referencias")]
    public GameObject piecePrefab;
    
    private Sprite selectedSprite;

    public void GeneratePuzzle()
    {
        ConfigurarDificuldade();
        LoadRandomSprite();
        if (selectedSprite == null) return;

        CreatePuzzleGuide();
        SliceAndCreatePieces();
    }

    void ConfigurarDificuldade()
    {
        int fase = PuzzleGameManager.FaseSelecionada;
        switch (fase)
        {
            case 1: rows = 2; columns = 2; break; // 4 pecas
            case 2: rows = 2; columns = 3; break; // 6 pecas
            case 3: rows = 2; columns = 5; break; // 10 pecas
            default: rows = 2; columns = 2; break;
        }
        Debug.Log($"Configurando Dificuldade: Fase {fase} ({rows}x{columns})");
    }

    void LoadRandomSprite()
    {
        string fullPath = Path.Combine(Application.dataPath, spritesPath.Replace("Assets/", ""));
        if (!Directory.Exists(fullPath)) return;

        string[] files = Directory.GetFiles(fullPath, "*.png");
        if (files.Length == 0) return;

        string randomFile = files[Random.Range(0, files.Length)];
        byte[] fileData = File.ReadAllBytes(randomFile);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);

        selectedSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    void CreatePuzzleGuide()
    {
        GameObject guide = new GameObject("PuzzleGuide_Background");
        guide.transform.SetParent(transform);
        guide.transform.localPosition = Vector3.zero;
        
        SpriteRenderer sr = guide.AddComponent<SpriteRenderer>();
        sr.sprite = selectedSprite;
        // Cor de guia "fantasma" para a crianca saber onde montar
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
                // Usando 100 pixels por unidade como padrao do Unity
                Sprite pieceSprite = Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f), 100);

                GameObject pieceObj = Instantiate(piecePrefab, transform);
                pieceObj.name = $"Piece_{r}_{c}";
                
                SpriteRenderer sr = pieceObj.GetComponent<SpriteRenderer>();
                sr.sprite = pieceSprite;
                sr.sortingOrder = 5; // Pecas soltas ficam acima do guia

                if (pieceObj.GetComponent<Collider2D>() == null)
                {
                    pieceObj.AddComponent<BoxCollider2D>();
                }

                // Posicao alvo (onde a peca deve encaixar)
                float posX = (c - (columns / 2f) + 0.5f) * (pieceWidth / 100f);
                float posY = (r - (rows / 2f) + 0.5f) * (pieceHeight / 100f);
                Vector3 targetPos = new Vector3(posX, posY, 0);

                PuzzlePiece pieceScript = pieceObj.GetComponent<PuzzlePiece>();
                pieceScript.SetTarget(targetPos);

                // Embaralha as pecas nas laterais extremas da tela (fora da area do guia central)
                // Usamos localPosition para garantir que fiquem relativas ao PuzzleSystem
                // Com Camera Orthographic Size 8, a largura da tela eh aproximadamente 14-16 unidades para cada lado (dependendo do aspect ratio)
                float side = (Random.value > 0.5f) ? 1f : -1f;
                
                // Garantimos que o X comece depois da largura do puzzle (que tem aprox 5-6 unidades)
                // O valor de 8.5f a 11f garante que fiquem nas bordas laterais
                float spawnX = side * Random.Range(8.5f, 11.0f); 
                float spawnY = Random.Range(-6.0f, 6.0f); // Distribuicao vertical
                
                pieceObj.transform.localPosition = new Vector3(spawnX, spawnY, 0);
}
        }
    }
}
