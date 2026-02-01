using FluentValidation;

namespace CadastroVeiculos.Application.Validators
{
    public class CadastrarUsuarioValidator : AbstractValidator<(string nome, string login, string senha)>
    {
        public CadastrarUsuarioValidator()
        {
            RuleFor(x => x.nome)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres")
                .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres");

            RuleFor(x => x.login)
                .NotEmpty().WithMessage("Login é obrigatório")
                .MinimumLength(3).WithMessage("Login deve ter no mínimo 3 caracteres")
                .MaximumLength(50).WithMessage("Login deve ter no máximo 50 caracteres");

            RuleFor(x => x.senha)
                .NotEmpty().WithMessage("Senha é obrigatória")
                .MinimumLength(6).WithMessage("Senha deve ter no mínimo 6 caracteres");
        }
    }

    public class CadastrarVeiculoValidator : AbstractValidator<(string descricao, int marca, string modelo, string? opcionais, decimal? valor)>
    {
        public CadastrarVeiculoValidator()
        {
            RuleFor(x => x.descricao)
                .NotEmpty().WithMessage("Descrição é obrigatória")
                .MaximumLength(100).WithMessage("Descrição deve ter no máximo 100 caracteres");

            RuleFor(x => x.marca)
                .GreaterThan(0).WithMessage("Marca é obrigatória");

            RuleFor(x => x.modelo)
                .NotEmpty().WithMessage("Modelo é obrigatório")
                .MaximumLength(30).WithMessage("Modelo deve ter no máximo 30 caracteres");

            RuleFor(x => x.valor)
                .GreaterThan(0).When(x => x.valor.HasValue).WithMessage("Valor deve ser maior que zero");
        }
    }
}
