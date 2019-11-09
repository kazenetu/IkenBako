using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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