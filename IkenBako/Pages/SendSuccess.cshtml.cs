using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

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

    }
  }
}