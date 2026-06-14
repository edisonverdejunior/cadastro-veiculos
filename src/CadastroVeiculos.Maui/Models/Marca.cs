namespace CadastroVeiculos.Maui.Models;

public class MarcaItem
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;

    public override string ToString() => Nome;

    public static List<MarcaItem> ObterTodas() =>
    [
        new() { Id = 1, Nome = "Toyota" },
        new() { Id = 2, Nome = "Honda" },
        new() { Id = 3, Nome = "Hyundai" },
        new() { Id = 4, Nome = "Volkswagen" },
        new() { Id = 5, Nome = "Chevrolet" },
        new() { Id = 6, Nome = "Ford" },
        new() { Id = 7, Nome = "BMW" },
        new() { Id = 8, Nome = "Mercedes" },
        new() { Id = 9, Nome = "Audi" },
        new() { Id = 10, Nome = "Fiat" },
        new() { Id = 11, Nome = "Renault" },
        new() { Id = 12, Nome = "Peugeot" },
        new() { Id = 13, Nome = "Nissan" },
        new() { Id = 14, Nome = "Kia" },
        new() { Id = 15, Nome = "Jeep" }
    ];

    public static string ObterNome(int id)
    {
        var marca = ObterTodas().Find(m => m.Id == id);
        return marca?.Nome ?? "Desconhecida";
    }
}
