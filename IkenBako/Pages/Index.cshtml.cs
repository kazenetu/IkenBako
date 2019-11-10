using IkenBako.Infrastructures;
using IkenBako.Models;
using Microsoft.AspNetCore.Mvc;
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
        var targets = SendTargets.GetInstance().Targets.Select(item => new { item.DisplayName,item.ID});
        return targets.Select(target => new SelectListItem(target.DisplayName, target.ID, false)).ToList();
      }
    }

    public IndexModel(ILogger<IndexModel> logger)
    {
      _logger = logger;
    }

    public void OnGet()
    {
    }

    public IActionResult OnPost(Message message)
    {
      // 意見メッセージ保存
      try
      {
        message.Save(new MessageRepository());

        // 保存OKの場合は完了メッセージページへ
        return RedirectToPage("/SendSuccess");
      }
      catch(System.IO.FileLoadException ex)
      {
        // 例外エラーログ追記
        _logger.LogError(ex.Message);

        // 保存NG
        ViewData["ErrorMessages"] = new List<string> { "保存に失敗しました。", "時間を置いて再送信してください。" };
        return Page();
      }

    }

  }
}
