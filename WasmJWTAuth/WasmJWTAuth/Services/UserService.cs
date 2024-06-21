using WasmJWTAuth.Models;

namespace WasmJWTAuth.Services;

public class UserService : IUserService
{
  private IHttpService _httpService;

  public UserService(IHttpService httpService)
  {
    _httpService = httpService;
  }

  public async Task<IEnumerable<User>> GetAllUsers()
  {
    return await _httpService.Get<IEnumerable<User>>("/users");
  }
}

public interface IUserService
{
  Task<IEnumerable<User>> GetAllUsers();
}