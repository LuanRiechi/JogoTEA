using System;

namespace JogoTEA.Persistence
{
    // Classe que representa a tabela de Alunos no banco de dados
    [Serializable]
    public class AlunoEntity
    {
        public int Id;        // ID único (Chave Primária)
        public string Nome;   // Nome do aluno
    }

    // Classe que representa o progresso de um aluno em um mini-game
    [Serializable]
    public class ProgressoEntity
    {
        public int Id;            // ID único
        public int AlunoId;       // ID do aluno relacionado (Chave Estrangeira)
        public string MiniGame;   // Nome do mini-game (Memoria, Labirinto, etc)
        public int Fase;          // Número da fase
        public bool Completou;    // Se a fase foi concluída
        public float MelhorTempo; // Menor tempo registrado para esta fase
    }

    // Classe para armazenar configurações globais do aplicativo
    [Serializable]
    public class ConfigEntity
    {
        public string Chave; // Nome da configuração (ex: alunoSelecionado)
        public string Valor; // Valor da configuração
    }
}
