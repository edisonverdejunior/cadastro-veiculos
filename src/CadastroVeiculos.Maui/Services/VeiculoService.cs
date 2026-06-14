using CadastroVeiculos.Maui.Models;

namespace CadastroVeiculos.Maui.Services;

public class VeiculoService
{
    private readonly ApiService _api;

    public VeiculoService(ApiService api)
    {
        _api = api;
    }

    public async Task<VeiculoResponse?> Criar(VeiculoRequest request)
    {
        return await _api.PostAsync<VeiculoResponse>("api/Veiculos", request);
    }

    public async Task<List<VeiculoResponse>> Listar()
    {
        return await _api.GetAsync<List<VeiculoResponse>>("api/Veiculos") ?? [];
    }

    public async Task<List<VeiculoResponse>> Buscar(string? descricao, int? marca, string? modelo, string? opcionais)
    {
        var queryParams = new List<string>();

        if (!string.IsNullOrWhiteSpace(descricao))
            queryParams.Add($"descricao={Uri.EscapeDataString(descricao)}");
        if (marca.HasValue && marca.Value > 0)
            queryParams.Add($"marca={marca.Value}");
        if (!string.IsNullOrWhiteSpace(modelo))
            queryParams.Add($"modelo={Uri.EscapeDataString(modelo)}");
        if (!string.IsNullOrWhiteSpace(opcionais))
            queryParams.Add($"opcionais={Uri.EscapeDataString(opcionais)}");

        var query = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
        return await _api.GetAsync<List<VeiculoResponse>>($"api/Veiculos/buscar{query}") ?? [];
    }

    public async Task<VeiculoResponse?> Obter(string id)
    {
        return await _api.GetAsync<VeiculoResponse>($"api/Veiculos/{id}");
    }

    public async Task<VeiculoResponse?> Atualizar(string id, VeiculoRequest request)
    {
        return await _api.PutAsync<VeiculoResponse>($"api/Veiculos/{id}", request);
    }

    public async Task Excluir(string id)
    {
        await _api.DeleteAsync($"api/Veiculos/{id}");
    }
}
