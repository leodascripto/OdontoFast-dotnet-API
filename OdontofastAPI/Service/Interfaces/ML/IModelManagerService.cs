using Microsoft.ML;
using Microsoft.ML.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OdontofastAPI.Service.Interfaces.ML
{
    /// <summary>
    /// Interface para o serviço de gerenciamento de modelos de ML
    /// </summary>
    public interface IModelManagerService
    {
        /// <summary>
        /// Carrega um modelo treinado a partir do armazenamento
        /// </summary>
        /// <typeparam name="TInput">Tipo de entrada do modelo</typeparam>
        /// <typeparam name="TOutput">Tipo de saída do modelo</typeparam>
        /// <param name="modelName">Nome do modelo a ser carregado</param>
        /// <returns>Status da operação</returns>
        Task<bool> CarregarModeloAsync<TInput, TOutput>(string modelName)
            where TInput : class
            where TOutput : class, new();

        /// <summary>
        /// Salva um modelo treinado no armazenamento
        /// </summary>
        /// <typeparam name="TInput">Tipo de entrada do modelo</typeparam>
        /// <typeparam name="TOutput">Tipo de saída do modelo</typeparam>
        /// <param name="modelName">Nome do modelo a ser salvo</param>
        /// <param name="model">Modelo treinado</param>
        /// <param name="schema">Esquema de dados do modelo</param>
        /// <returns>Status da operação</returns>
        Task<bool> SalvarModeloAsync<TInput, TOutput>(string modelName, ITransformer model, DataViewSchema schema)
            where TInput : class
            where TOutput : class, new();

        /// <summary>
        /// Verifica se um modelo está treinado e disponível
        /// </summary>
        /// <param name="modelName">Nome do modelo a verificar</param>
        /// <returns>True se o modelo está disponível</returns>
        Task<bool> VerificarModeloExisteAsync(string modelName);

        /// <summary>
        /// Obtém um modelo carregado em memória
        /// </summary>
        /// <param name="modelName">Nome do modelo</param>
        /// <returns>O modelo carregado ou null se não existir</returns>
        ITransformer? GetLoadedModel(string modelName);

        /// <summary>
        /// Cria um motor de predição para o modelo especificado
        /// </summary>
        /// <typeparam name="TInput">Tipo de entrada do modelo</typeparam>
        /// <typeparam name="TOutput">Tipo de saída do modelo</typeparam>
        /// <param name="modelName">Nome do modelo</param>
        /// <returns>Motor de predição ou null se o modelo não estiver carregado</returns>
        PredictionEngine<TInput, TOutput>? CreatePredictionEngine<TInput, TOutput>(string modelName)
            where TInput : class
            where TOutput : class, new();

        /// <summary>
        /// Remove um modelo do cache em memória
        /// </summary>
        /// <param name="modelName">Nome do modelo</param>
        /// <returns>True se o modelo foi removido, False caso contrário</returns>
        bool UnloadModel(string modelName);

        /// <summary>
        /// Lista todos os modelos disponíveis (tanto em memória quanto em disco)
        /// </summary>
        /// <returns>Lista de nomes de modelos</returns>
        Task<List<string>> ListarModelosDisponiveisAsync();
    }
}