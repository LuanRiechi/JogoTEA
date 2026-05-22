using UnityEngine;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.IO;
using JogoTEA.Persistence;
using System;
using System.Linq;

/*
 * Este serviço gerencia a persistência de dados local usando SQLite.
 * Ele lida com a criação de tabelas, inserção, deleção e consultas SQL.
 * Requer sqlite3.dll na pasta Plugins para funcionar no Windows.
 */
public class SQLiteService
{
    private string dbPath;
    private string connectionString;

    public SQLiteService()
    {
        // Define o caminho do banco de dados na pasta de dados persistentes do usuário
        dbPath = Path.Combine(Application.persistentDataPath, "jogotea.db");
        connectionString = "URI=file:" + dbPath;

        InitializeDatabase();
    }

    // Inicializa o banco de dados e cria as tabelas se não existirem
    private void InitializeDatabase()
    {
        try
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    // Tabela de Alunos
                    command.CommandText = "CREATE TABLE IF NOT EXISTS Alunos (" +
                                         "Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                         "Nome TEXT UNIQUE NOT NULL);";
                    command.ExecuteNonQuery();

                    // Tabela de Progressos com deleção em cascata
                    command.CommandText = "CREATE TABLE IF NOT EXISTS Progressos (" +
                                         "Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                         "AlunoId INTEGER NOT NULL, " +
                                         "MiniGame TEXT NOT NULL, " +
                                         "Fase INTEGER NOT NULL, " +
                                         "Completou INTEGER NOT NULL, " +
                                         "MelhorTempo REAL, " +
                                         "FOREIGN KEY (AlunoId) REFERENCES Alunos(Id) ON DELETE CASCADE);";
                    command.ExecuteNonQuery();

                    // Tabela de Configurações globais
                    command.CommandText = "CREATE TABLE IF NOT EXISTS AppConfig (" +
                                         "Chave TEXT PRIMARY KEY, " +
                                         "Valor TEXT);";
                    command.ExecuteNonQuery();
                }
            }
            Debug.Log("<color=green>Banco de Dados SQLite Inicializado com sucesso em: </color>" + dbPath);
        }
        catch (Exception e)
        {
            Debug.LogError("Falha ao inicializar Banco de Dados SQLite: " + e.Message);
            throw; // Repassa o erro para ser tratado no DataManager/MenuController
        }
    }

    #region Operações de Alunos
    public List<AlunoEntity> GetAllAlunos()
    {
        var list = new List<AlunoEntity>();
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Alunos";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new AlunoEntity { Id = reader.GetInt32(0), Nome = reader.GetString(1) });
                    }
                }
            }
        }
        return list;
    }

    public bool CreateAluno(string nome)
    {
        try
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO Alunos (Nome) VALUES (@nome)";
                    command.Parameters.Add(new SqliteParameter("@nome", nome));
                    command.ExecuteNonQuery();
                }
            }
            return true;
        }
        catch (Exception)
        {
            return false; // Nome duplicado ou erro de banco
        }
    }

    public void DeleteAluno(string nome)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM Alunos WHERE Nome = @nome";
                command.Parameters.Add(new SqliteParameter("@nome", nome));
                command.ExecuteNonQuery();
            }
        }
    }
    #endregion

    #region Operações de Progresso
    public List<ProgressoEntity> GetProgressoByAlunoId(int alunoId)
    {
        var list = new List<ProgressoEntity>();
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Progressos WHERE AlunoId = @alunoId";
                command.Parameters.Add(new SqliteParameter("@alunoId", alunoId));
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ProgressoEntity {
                            Id = reader.GetInt32(0),
                            AlunoId = reader.GetInt32(1),
                            MiniGame = reader.GetString(2),
                            Fase = reader.GetInt32(3),
                            Completou = reader.GetInt32(4) == 1,
                            MelhorTempo = reader.GetFloat(5)
                        });
                    }
                }
            }
        }
        return list;
    }

    public void SaveProgresso(int alunoId, string miniGame, int fase, bool completou, float tempo)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                // Verifica recorde existente
                command.CommandText = "SELECT Id, MelhorTempo FROM Progressos WHERE AlunoId = @alunoId AND MiniGame = @miniGame AND Fase = @fase";
                command.Parameters.Add(new SqliteParameter("@alunoId", alunoId));
                command.Parameters.Add(new SqliteParameter("@miniGame", miniGame));
                command.Parameters.Add(new SqliteParameter("@fase", fase));

                int? existingId = null;
                float currentBest = -1;
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read()) { existingId = reader.GetInt32(0); currentBest = reader.GetFloat(1); }
                }

                if (existingId.HasValue)
                {
                    // Atualiza apenas se for um tempo melhor ou se for a primeira conclusão
                    bool shouldUpdateTime = currentBest <= 0 || (completou && (tempo < currentBest));
                    float finalTime = shouldUpdateTime ? tempo : currentBest;
                    command.CommandText = "UPDATE Progressos SET Completou = @completou, MelhorTempo = @tempo WHERE Id = @id";
                    command.Parameters.Clear();
                    command.Parameters.Add(new SqliteParameter("@completou", completou ? 1 : 0));
                    command.Parameters.Add(new SqliteParameter("@tempo", finalTime));
                    command.Parameters.Add(new SqliteParameter("@id", existingId.Value));
                    command.ExecuteNonQuery();
                }
                else
                {
                    // Novo registro
                    command.CommandText = "INSERT INTO Progressos (AlunoId, MiniGame, Fase, Completou, MelhorTempo) VALUES (@alunoId, @miniGame, @fase, @completou, @tempo)";
                    command.Parameters.Clear();
                    command.Parameters.Add(new SqliteParameter("@alunoId", alunoId));
                    command.Parameters.Add(new SqliteParameter("@miniGame", miniGame));
                    command.Parameters.Add(new SqliteParameter("@fase", fase));
                    command.Parameters.Add(new SqliteParameter("@completou", completou ? 1 : 0));
                    command.Parameters.Add(new SqliteParameter("@tempo", tempo));
                    command.ExecuteNonQuery();
                }
            }
        }
    }
    #endregion

    #region Configurações
    public string GetConfig(string chave)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT Valor FROM AppConfig WHERE Chave = @chave";
                command.Parameters.Add(new SqliteParameter("@chave", chave));
                var res = command.ExecuteScalar();
                return res?.ToString() ?? "";
            }
        }
    }

    public void SaveConfig(string chave, string valor)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT OR REPLACE INTO AppConfig (Chave, Valor) VALUES (@chave, @valor)";
                command.Parameters.Add(new SqliteParameter("@chave", chave));
                command.Parameters.Add(new SqliteParameter("@valor", valor));
                command.ExecuteNonQuery();
            }
        }
    }
    #endregion
}


