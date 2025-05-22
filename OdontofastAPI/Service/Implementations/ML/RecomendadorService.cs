// OdontofastAPI/Service/Implementations/ML/RecomendadorService.cs
using Microsoft.Extensions.Logging;
using OdontofastAPI.DTO.ML;
using OdontofastAPI.Repository.Interfaces;
using OdontofastAPI.Service.Interfaces.ML;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OdontofastAPI.Service.Implementations.ML
{
    /// <summary>
    /// Implementação do serviço de recomendações personalizadas
    /// </summary>
    public class RecomendadorService : IRecomendadorService
    {
        private readonly ILogger<RecomendadorService> _logger;
        private readonly IUsuarioRepository _usuarioRepository;

        public RecomendadorService(
            ILogger<RecomendadorService> logger,
            IUsuarioRepository usuarioRepository)
        {
            _logger = logger;
            _usuarioRepository = usuarioRepository;
        }

        /// <inheritdoc/>
        public async Task<RecomendacaoResponseDto> GerarRecomendacoesPersonalizadasAsync(RecomendacaoRequestDto requestDto)
        {
            try
            {
                // Verifica se o usuário existe
                var usuario = await _usuarioRepository.GetByIdAsync(requestDto.IdUsuario);
                if (usuario == null)
                {
                    throw new ArgumentException($"Usuário com ID {requestDto.IdUsuario} não encontrado");
                }

                // Determina o estágio do tratamento com base no progresso
                var estagioTratamento = DeterminarEstagio(requestDto.ProgressoAtual);

                // Gera recomendações personalizadas baseadas no tipo de tratamento e estágio
                var recomendacoesCuidados = GerarRecomendacoesCuidados(requestDto.TipoTratamento, estagioTratamento);
                var recomendacoesProximasEtapas = GerarRecomendacoesProximasEtapas(requestDto.TipoTratamento, estagioTratamento);

                // Cria uma mensagem personalizada
                var mensagemPersonalizada = CriarMensagemPersonalizada(
                    usuario.NomeUsuario,
                    requestDto.TipoTratamento,
                    estagioTratamento,
                    requestDto.ProgressoAtual);

                return new RecomendacaoResponseDto
                {
                    RecomendacoesCuidados = recomendacoesCuidados,
                    RecomendacoesProximasEtapas = recomendacoesProximasEtapas,
                    MensagemPersonalizada = mensagemPersonalizada
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar recomendações personalizadas");

                // Retorna recomendações genéricas em caso de erro
                return new RecomendacaoResponseDto
                {
                    RecomendacoesCuidados = new List<string>
                    {
                        "Mantenha uma boa higiene bucal",
                        "Escove os dentes após as refeições"
                    },
                    RecomendacoesProximasEtapas = new List<string>
                    {
                        "Continue seguindo o plano de tratamento",
                        "Compareça às consultas agendadas"
                    },
                    MensagemPersonalizada = "Continue cuidando da sua saúde bucal!"
                };
            }
        }

        /// <summary>
        /// Determina o estágio do tratamento com base no progresso
        /// </summary>
        private string DeterminarEstagio(float progresso)
        {
            if (progresso < 0.25f)
                return "inicial";
            else if (progresso < 0.75f)
                return "intermediário";
            else
                return "final";
        }

        /// <summary>
        /// Gera recomendações de cuidados baseadas no tipo e estágio do tratamento
        /// </summary>
        private List<string> GerarRecomendacoesCuidados(string tipoTratamento, string estagioTratamento)
        {
            var recomendacoes = new List<string>();

            // Recomendações comuns a todos os tratamentos
            recomendacoes.Add("Mantenha uma boa higiene bucal escovando os dentes após as refeições");
            recomendacoes.Add("Use fio dental diariamente para remover resíduos entre os dentes");

            // Recomendações específicas por tipo de tratamento
            switch (tipoTratamento.ToLower())
            {
                case "ortodontia":
                    recomendacoes.Add("Use escova interdental para limpar ao redor do aparelho");
                    recomendacoes.Add("Evite alimentos duros, pegajosos ou muito açucarados");

                    if (estagioTratamento == "inicial")
                        recomendacoes.Add("Use cera ortodôntica se sentir desconforto com os fios ou bráquetes");
                    else if (estagioTratamento == "intermediário")
                        recomendacoes.Add("Mantenha os elásticos conforme orientação do ortodontista");
                    else // final
                        recomendacoes.Add("Prepare-se para o uso de contenção após a remoção do aparelho");
                    break;

                case "implante":
                    if (estagioTratamento == "inicial")
                    {
                        recomendacoes.Add("Evite alimentos duros ou pegajosos na área do implante");
                        recomendacoes.Add("Tome os antibióticos e anti-inflamatórios conforme prescrição");
                    }
                    else if (estagioTratamento == "intermediário")
                    {
                        recomendacoes.Add("Evite fumar durante todo o tratamento");
                        recomendacoes.Add("Mantenha a área do implante muito limpa para evitar infecções");
                    }
                    else // final
                    {
                        recomendacoes.Add("Escove ao redor da coroa do implante com cuidado");
                        recomendacoes.Add("Utilize escovas interproximais para limpeza adequada");
                    }
                    break;

                case "canal":
                    if (estagioTratamento == "inicial")
                    {
                        recomendacoes.Add("Evite mastigar com o dente tratado até a restauração definitiva");
                        recomendacoes.Add("Tome analgésicos conforme orientação para controle da dor");
                    }
                    else if (estagioTratamento == "intermediário")
                    {
                        recomendacoes.Add("Mantenha boa higiene para evitar reinfecção do canal");
                        recomendacoes.Add("Informe qualquer sintoma persistente ao dentista");
                    }
                    else // final
                    {
                        recomendacoes.Add("Proteja o dente tratado evitando morder alimentos muito duros");
                        recomendacoes.Add("Mantenha visitas regulares para acompanhamento");
                    }
                    break;

                default:
                    recomendacoes.Add("Siga as orientações específicas do seu dentista");
                    recomendacoes.Add("Mantenha as consultas de acompanhamento regulares");
                    break;
            }

            return recomendacoes;
        }

        /// <summary>
        /// Gera recomendações para próximas etapas baseadas no tipo e estágio do tratamento
        /// </summary>
        private List<string> GerarRecomendacoesProximasEtapas(string tipoTratamento, string estagioTratamento)
        {
            var recomendacoes = new List<string>();

            // Recomendações comuns a todos os tratamentos
            recomendacoes.Add("Compareça às consultas agendadas para o melhor resultado");

            // Recomendações específicas por tipo e estágio
            switch (tipoTratamento.ToLower())
            {
                case "ortodontia":
                    if (estagioTratamento == "inicial")
                    {
                        recomendacoes.Add("Prepare-se para ajustes mensais no aparelho");
                        recomendacoes.Add("Consiga os materiais de higiene específicos para aparelho");
                    }
                    else if (estagioTratamento == "intermediário")
                    {
                        recomendacoes.Add("Mantenha o uso dos elásticos conforme orientação");
                        recomendacoes.Add("Programe os próximos ajustes com antecedência");
                    }
                    else // final
                    {
                        recomendacoes.Add("Prepare-se para a remoção do aparelho");
                        recomendacoes.Add("Discuta o uso de contenção após o tratamento");
                    }
                    break;

                case "implante":
                    if (estagioTratamento == "inicial")
                    {
                        recomendacoes.Add("Programe a avaliação de cicatrização inicial");
                        recomendacoes.Add("Organize o tempo para os retornos necessários");
                    }
                    else if (estagioTratamento == "intermediário")
                    {
                        recomendacoes.Add("Prepare-se para a fase de colocação do cicatrizador");
                        recomendacoes.Add("Discuta as opções para a prótese definitiva");
                    }
                    else // final
                    {
                        recomendacoes.Add("Prepare-se para a moldagem e instalação da coroa definitiva");
                        recomendacoes.Add("Programe consultas de manutenção periódicas");
                    }
                    break;

                case "canal":
                    if (estagioTratamento == "inicial")
                    {
                        recomendacoes.Add("Prepare-se para concluir o tratamento endodôntico");
                        recomendacoes.Add("Discuta as opções de restauração definitiva");
                    }
                    else if (estagioTratamento == "intermediário")
                    {
                        recomendacoes.Add("Prepare-se para a avaliação final do canal");
                        recomendacoes.Add("Programe a restauração definitiva");
                    }
                    else // final
                    {
                        recomendacoes.Add("Realize uma consulta de acompanhamento em 6 meses");
                        recomendacoes.Add("Considere uma proteção adicional (ex: coroa) se recomendado");
                    }
                    break;

                default:
                    recomendacoes.Add("Solicite ao dentista um detalhamento das próximas etapas");
                    recomendacoes.Add("Programe as próximas consultas com base no plano de tratamento");
                    break;
            }

            return recomendacoes;
        }

        /// <summary>
        /// Cria uma mensagem personalizada com base no perfil e progresso
        /// </summary>
        private string CriarMensagemPersonalizada(string nomeUsuario, string tipoTratamento, string estagioTratamento, float progresso)
        {
            string saudacao = $"Olá, {nomeUsuario}! ";
            string corpo = string.Empty;

            // Personalização por estágio
            if (estagioTratamento == "inicial")
            {
                corpo = $"Você está dando os primeiros passos no seu tratamento de {tipoTratamento}. " +
                        "É importante manter a disciplina desde o início para obter os melhores resultados.";
            }
            else if (estagioTratamento == "intermediário")
            {
                corpo = $"Você já alcançou {(progresso * 100):F0}% do seu tratamento de {tipoTratamento}! " +
                        "Continue com o bom trabalho e mantenha a constância nos cuidados recomendados.";
            }
            else // final
            {
                corpo = $"Você está muito próximo de concluir seu tratamento de {tipoTratamento}! " +
                        "Esta fase final é crucial para garantir resultados duradouros.";
            }

            // Personalização adicional por tipo de tratamento
            string adicional = string.Empty;
            switch (tipoTratamento.ToLower())
            {
                case "ortodontia":
                    adicional = "Lembre-se que um sorriso bem alinhado traz benefícios estéticos e funcionais para toda a vida.";
                    break;
                case "implante":
                    adicional = "Os implantes dentários têm alta taxa de sucesso e podem durar uma vida inteira com os cuidados adequados.";
                    break;
                case "canal":
                    adicional = "O tratamento de canal salva seu dente natural, permitindo que você mantenha sua mordida natural e sorriso saudável.";
                    break;
                default:
                    adicional = "Sua saúde bucal é parte importante da sua saúde geral. Estamos aqui para apoiá-lo nessa jornada.";
                    break;
            }

            return $"{saudacao}{corpo} {adicional}";
        }
    }
}