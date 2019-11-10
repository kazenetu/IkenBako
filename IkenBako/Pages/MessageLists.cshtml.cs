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
  public class MessageListsModel : PageModel
  {
    private readonly ILogger<MessageListsModel> _logger;

    public List<Message> Messages { get; private set; } = new List<Message>();

    /// <summary>
    /// 確認者
    /// </summary>
    [BindProperty]
    public string Target { set; get; }

    /// <summary>
    /// 送信対象者リスト
    /// </summary>
    public List<SelectListItem> SendTargetList
    {
      get
      {
        var targets = SendTargets.GetInstance().Targets.Select(item => new { item.DisplayName, item.ID });
        return targets.Select(target => new SelectListItem(target.DisplayName, target.ID, false)).ToList();
      }
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="logger"></param>
    public MessageListsModel(ILogger<MessageListsModel> logger)
    {
      _logger = logger;
    }

    public void OnGet()
    {
    }

    public void OnPost()
    {
      var target = SendTargets.GetInstance().Targets.Where(item => item.ID == Target).FirstOrDefault();
      if(target is null)
      {
        return;
      }

      // 対象の意見メッセージを取得
      Messages = (new MessageRepository()).FindMessage(target);

      // メッセージがない場合はその旨のエラーメッセージを設定
      if (!Messages.Any())
      {
        ViewData["Message"] = "意見メッセージはありません。";
      }
    }
  }
}