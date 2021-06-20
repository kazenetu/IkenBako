using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace IkenBako.Pages
{
  public class SendSuccessModel : PageModel
  {
    private readonly ILogger<SendSuccessModel> _logger;

    public SendSuccessModel(ILogger<SendSuccessModel> logger)
    {
      _logger = logger;
    }

    public void OnGet()
    {
      if (!HttpContext.Session.Keys.Contains(LoginModel.KEY_LOGIN_ID))
      {
        Response.Redirect("/Login");
        return;
      }
    }
  }
}