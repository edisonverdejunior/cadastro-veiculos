using CadastroVeiculos.Maui.Models;

namespace CadastroVeiculos.Maui.Services;

public class UsuarioService
{
    private readonly ApiService _api;

    public UsuarioService(ApiService api)
    {
        _api = api;
    }

    public async Task<UsuarioResponse?> Criar(CadastrarUsuarioRequest request)
    {
        return await _api.PostSemAuthAsync<UsuarioResponse>("api/Usuarios", request);
    }

    public async Task<List<UsuarioResponse>> Listar()
    {
        return await _api.GetAsync<List<UsuarioResponse>>("api/Usuarios") ?? [];
    }

    public async Task<UsuarioResponse?> Obter(string id)
    {
        return await _api.GetAsync<UsuarioResponse>($"api/Usuarios/{id}");
    }

    public async Task<UsuarioResponse?> Atualizar(string id, AtualizarUsuarioRequest request)
    {
        return await _api.PutAsync<UsuarioResponse>($"api/Usuarios/{id}", request);
    }

    public async Task Excluir(string id)
    {
        await _api.DeleteAsync($"api/Usuarios/{id}");
    }
}
