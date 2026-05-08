using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[Serializable]
public class FaseProgresso
{
    public string miniGame;
    public int fase;
    public bool completou;
    public float melhorTempo;
}

[Serializable]
public class Aluno
{
    public string nome;
    public List<FaseProgresso> progressos = new List<FaseProgresso>();
}

[Serializable]
public class GameData
{
    public List<Aluno> alunos = new List<Aluno>();
    public string alunoSelecionado;
}

public static class DataManager
{
    private static string path = Path.Combine(Application.persistentDataPath, "game_data.json");
    private static GameData _data;

    public static GameData Data
    {
        get
        {
            if (_data == null) Carregar();
            return _data;
        }
    }

    public static void Salvar()
    {
        if (Data == null) return;
        string json = JsonUtility.ToJson(Data, true);
        File.WriteAllText(path, json);
    }

    public static void Carregar()
    {
        try
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                if (string.IsNullOrEmpty(json))
                {
                    _data = new GameData();
                }
                else
                {
                    _data = JsonUtility.FromJson<GameData>(json);
                    if (_data == null) _data = new GameData();
                    if (_data.alunos == null) _data.alunos = new List<Aluno>();
                }
            }
            else
            {
                _data = new GameData();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Erro ao carregar dados: " + e.Message);
            _data = new GameData();
        }
    }

    public static bool CriarAluno(string nome)
    {
        if (string.IsNullOrEmpty(nome)) return false;
        
        // Ensure data is loaded
        if (Data == null || Data.alunos == null) return false;

        if (Data.alunos.Any(a => a != null && a.nome != null && a.nome.Equals(nome, StringComparison.OrdinalIgnoreCase)))
        {
            return false; // Nome ja existe
        }

        Data.alunos.Add(new Aluno { nome = nome });
        Salvar();
        return true;
    }

    public static void SelecionarAluno(string nome)
    {
        if (Data == null) return;
        Data.alunoSelecionado = nome;
        Salvar();
    }

    public static void DeletarAluno(string nome)
    {
        if (Data == null || Data.alunos == null) return;
        
        Aluno aluno = Data.alunos.FirstOrDefault(a => a.nome == nome);
        if (aluno != null)
        {
            Data.alunos.Remove(aluno);
            if (Data.alunoSelecionado == nome)
            {
                Data.alunoSelecionado = "";
            }
            Salvar();
        }
    }

    public static Aluno GetAlunoAtivo()
{
        if (Data == null || string.IsNullOrEmpty(Data.alunoSelecionado) || Data.alunos == null) return null;
        return Data.alunos.FirstOrDefault(a => a != null && a.nome == Data.alunoSelecionado);
    }

    public static void SalvarProgresso(string miniGame, int fase, bool completou, float tempo)
    {
        Aluno aluno = GetAlunoAtivo();
        if (aluno == null) return;

        if (aluno.progressos == null) aluno.progressos = new List<FaseProgresso>();

        FaseProgresso progresso = aluno.progressos.FirstOrDefault(p => p != null && p.miniGame == miniGame && p.fase == fase);

        if (progresso == null)
        {
            progresso = new FaseProgresso { miniGame = miniGame, fase = fase };
            aluno.progressos.Add(progresso);
        }

        if (completou)
        {
            progresso.completou = true;
            // Regra: Salvar apenas se o novo tempo for menor ou se ainda nao tinha tempo
            if (progresso.melhorTempo <= 0 || tempo < progresso.melhorTempo)
            {
                progresso.melhorTempo = tempo;
            }
        }
        
        Salvar();
    }
}
