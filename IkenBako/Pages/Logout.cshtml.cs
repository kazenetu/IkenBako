using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IkenBako.Pages
{
  public class LogoutModel : PageModel
  {
    private readonly ILogger<LogoutModel> _logger;

    /// コンストラクタ
    /// </summary>
    /// <param name="logger">ログインスタンス</param>
    public LogoutModel(ILogger<LogoutModel> logger)
    {
      _logger = logger;
    }

    public IActionResult OnGet()
    {
      foreach (var cookie in Request.Cookies.Keys)
      {
        Response.Cookies.Delete(cookie);
      }
      return RedirectToPage("/Login");
    }
  }
}
