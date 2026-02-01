using CadastroVeiculo.Domain.Entities;
using CadastroVeiculo.Domain.Interface.Repository;
using CadastroVeiculos.Infra.Data.Context;

namespace CadastroVeiculos.Infra.Data.Repository
{
    public class VeiculoRepository(CadastroVeiculosContext context) : Repository<Veiculo>(context), IVeiculoRepository
    {
    }
}
