using IkenBako.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace IkenBako.Pages
{
  public class IndexModel : PageModel
  {
    private readonly ILogger<IndexModel> _logger;

    public Message Message { get; set; }

    public List<SelectListItem> SendTargetList { 
      get
      {
        var names = SendTargets.GetInstance().Targets.Select(item => item.Name);
        return names.Select(name => new SelectListItem(name, name,false)).ToList();
      }
    }

    public IndexModel(ILogger<IndexModel> logger)
    {
      _logger = logger;
    }

    public void OnGet()
    {
    }

    public void OnPost(Message message)
    {
      // メッセージインスタンスの処理
    }

  }
}
