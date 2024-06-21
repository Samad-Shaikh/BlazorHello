using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using WasmJWTAuth.Models;

namespace WasmJWTAuth.Services;

public class HttpService : IHttpService
{
  private HttpClient _httpClient;
  private NavigationManager _navigationManager;
  private ILocalStorageService _localStorageService;

  public HttpService(HttpClient httpClient,
    NavigationManager navigationManager,
    ILocalStorageService localStorageService)
  {
    _httpClient = httpClient;
    _navigationManager = navigationManager;
    _localStorageService = localStorageService;
  }

  public async Task<T> Get<T>(string uri)
  {
    var request = new HttpRequestMessage(HttpMethod.Get, uri);
    return await sendRequest<T>(request);
  }

  public async Task<T> Post<T>(string uri, object value)
  {
    var request = new HttpRequestMessage(HttpMethod.Post, uri);
    request.Content = new StringContent(
      JsonSerializer.Serialize(value),
      Encoding.UTF8, "application/json");
    return await sendRequest<T>(request);
  }

  private async Task<T> sendRequest<T>(HttpRequestMessage request)
  {
    var user = await _localStorageService.GetItem<User>("user");
    var isApiUrl = !request.RequestUri.IsAbsoluteUri;
    if (user != null && isApiUrl)
      request.Headers.Authorization = new AuthenticationHeaderValue(
        "Bearer", user.Token);                                      // add jwt auth header if user is logged

    using var response = await _httpClient.SendAsync(request);
    
    if(response.StatusCode == HttpStatusCode.Unauthorized)                // if Http 401
    {
      _navigationManager.NavigateTo("logout");                         // redirect to logout
      return default;
    }

    if (!response.IsSuccessStatusCode)                                    // Handle Error response
    {
      var error = await response.Content
        .ReadFromJsonAsync<Dictionary<string, String>>();
      throw new Exception(error["message"]);
    }

    return await response.Content.ReadFromJsonAsync<T>();
  }
}

public interface IHttpService
{
  Task<T> Get<T>(string uri);
  Task<T> Post<T>(string uri, object value);
}