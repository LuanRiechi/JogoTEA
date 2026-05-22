using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using JogoTEA.Persistence;

/*
 * DataManager atua como a fachada principal para o sistema de dados.
 * Ele gerencia o cache em memória e delega a persistência para o SQLiteService.
 */
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
    private static SQLiteService _sqlite;
    private static GameData _cachedData;

    // Acesso preguiçoso ao serviço SQLite
    private static SQLiteService db
    {
        get
        {
            if (_sqlite == null) _sqlite = new SQLiteService();
            return _sqlite;
        }
    }

    // Acesso aos dados globais com carregamento automático
    public static GameData Data
    {
        get
        {
            if (_cachedData == null) LoadAllFromDb();
            return _cachedData;
        }
    }

    // Carrega todos os alunos, progressos e configurações do banco para o cache
    private static void LoadAllFromDb()
    {
        _cachedData = new GameData();
        var alunosEntities = db.GetAllAlunos();
        
        foreach (var entity in alunosEntities)
        {
            var aluno = new Aluno { nome = entity.Nome };
            var progressos = db.GetProgressoByAlunoId(entity.Id);
            aluno.progressos = progressos.Select(p => new FaseProgresso {
                miniGame = p.MiniGame,
                fase = p.Fase,
                completou = p.Completou,
                melhorTempo = p.MelhorTempo
            }).ToList();
            
            _cachedData.alunos.Add(aluno);
        }

        _cachedData.alunoSelecionado = db.GetConfig("alunoSelecionado");
    }

    // Método mantido para compatibilidade, mas o SQLite salva automaticamente em cada operação
    public static void Salvar()
    {
    }

    // Inicializa o sistema (chamado no MenuController)
    public static void Carregar()
    {
        LoadAllFromDb();
    }

    // Cria um novo aluno. Retorna true se criado com sucesso.
    public static bool CriarAluno(string nome)
    {
        if (string.IsNullOrEmpty(nome)) return false;

        if (db.CreateAluno(nome))
        {
            _cachedData = null; // Invalida o cache para forçar recarregamento
            return true;
        }
        return false;
    }

    // Define qual aluno está ativo no sistema
    public static void SelecionarAluno(string nome)
    {
        db.SaveConfig("alunoSelecionado", nome);
        if (_cachedData != null) _cachedData.alunoSelecionado = nome;
    }

    // Remove um aluno e limpa a seleção se o aluno removido era o ativo
    public static void DeletarAluno(string nome)
    {
        // Se o aluno deletado for o selecionado, limpa a configuração de seleção
        string selecionado = db.GetConfig("alunoSelecionado");
        if (selecionado == nome)
        {
            db.SaveConfig("alunoSelecionado", "");
        }

        db.DeleteAluno(nome);
        _cachedData = null; // Invalida o cache
    }

    // Retorna o objeto do aluno que está atualmente selecionado
    public static Aluno GetAlunoAtivo()
    {
        string selecionado = db.GetConfig("alunoSelecionado");
        if (string.IsNullOrEmpty(selecionado)) return null;

        var alunos = Data.alunos;
        return alunos.FirstOrDefault(a => a.nome == selecionado);
    }

    // Salva o progresso de uma partida no mini-game
    public static void SalvarProgresso(string miniGame, int fase, bool completou, float tempo)
    {
        string selecionado = db.GetConfig("alunoSelecionado");
        if (string.IsNullOrEmpty(selecionado)) return;

        var alunoEntity = db.GetAllAlunos().FirstOrDefault(a => a.Nome == selecionado);
        if (alunoEntity == null) return;

        db.SaveProgresso(alunoEntity.Id, miniGame, fase, completou, tempo);
        _cachedData = null; // Invalida o cache para refletir mudanças na UI
    }
}

