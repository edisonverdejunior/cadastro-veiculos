using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace CadastroVeiculos.Maui.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiService(HttpClient httpClient, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    private void ConfigurarAutorizacao()
    {
        var token = _authService.ObterToken();
        _httpClient.DefaultRequestHeaders.Authorization =
            string.IsNullOrEmpty(token) ? null : new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        ConfigurarAutorizacao();
        var response = await _httpClient.GetAsync(endpoint);
        await TratarErroAutenticacao(response);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
    }

    public async Task<T?> PostAsync<T>(string endpoint, object data)
    {
        ConfigurarAutorizacao();
        var response = await _httpClient.PostAsJsonAsync(endpoint, data);
        await TratarErroAutenticacao(response);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
    }

    public async Task<T?> PostSemAuthAsync<T>(string endpoint, object data)
    {
        var response = await _httpClient.PostAsJsonAsync(endpoint, data);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
    }

    public async Task<T?> PutAsync<T>(string endpoint, object data)
    {
        ConfigurarAutorizacao();
        var response = await _httpClient.PutAsJsonAsync(endpoint, data);
        await TratarErroAutenticacao(response);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
    }

    public async Task DeleteAsync(string endpoint)
    {
        ConfigurarAutorizacao();
        var response = await _httpClient.DeleteAsync(endpoint);
        await TratarErroAutenticacao(response);
        response.EnsureSuccessStatusCode();
    }

    private async Task TratarErroAutenticacao(HttpResponseMessage response)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await _authService.Logout();
            throw new UnauthorizedAccessException("Sessao expirada. Faca login novamente.");
        }
    }
}
