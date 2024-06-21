using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using WasmJWTAuth.Services;

namespace WasmJWTAuth.Components;

public class AppRouteView : RouteView
{
  [Inject] public NavigationManager NavigationManager { get; set; }
  
  [Inject] public IAuthenticationService AuthenticationService { get; set; }

  protected override void Render(RenderTreeBuilder builder)
  {
    var authorize = Attribute.GetCustomAttribute(
      RouteData.PageType, typeof(AuthorizeAttribute)) != null;
    var path = WebUtility.UrlDecode(new Uri(NavigationManager.Uri)
      .AbsolutePath);

    if (authorize
        && AuthenticationService.User == null
        && path != "/login")
    {
      var returnUrl = WebUtility.UrlEncode(new Uri(
        NavigationManager.Uri).PathAndQuery);
      NavigationManager.NavigateTo($"login?returnUrl={returnUrl}");
    } else 
      base.Render(builder);
  }
}