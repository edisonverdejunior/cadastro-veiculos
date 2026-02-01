using CadastroVeiculo.Domain.Entities;
using CadastroVeiculo.Domain.Interface.Repository;
using CadastroVeiculo.Domain.Interface.Service;

namespace CadastroVeiculo.Domain.Service
{
    public class VeiculoService(IVeiculoRepository repository) : Service<Veiculo>(repository), IVeiculoService
    {
        //Regras de negócio específicas de Veículo podem ser adicionadas aqui
    }
}
