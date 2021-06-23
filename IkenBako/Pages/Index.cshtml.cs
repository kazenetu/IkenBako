using Domain.Application;
using IkenBako.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace IkenBako.Pages
{
  public class IndexModel : PageModel
  {
    private readonly ILogger<IndexModel> _logger;

    /// <summary>
    /// 意見メッセージサービス
    /// </summary>
    private readonly MessageService messageService;

    /// <summary>
    /// 送信者サービス
    /// </summary>
    private readonly ReceiverService receiverService;

    /// <summary>
    /// 設定情報
    /// </summary>
    private SettingConfigModel config;

    /// <summary>
    /// 意見メッセージViewModel
    /// </summary>
    [BindProperty]
    public MessageViewModel Message { set; get; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="logger">ログインスタンス</param>
    /// <param name="messageService">意見メッセージサービス</param>
    /// <param name="receiverService">送信者サービス</param>
    /// <param name="config">設定情報</param>
    public IndexModel(ILogger<IndexModel> logger, MessageService messageService, ReceiverService receiverService, IOptions<SettingConfigModel> config)
    {
      _logger = logger;
      this.messageService = messageService;
      this.receiverService = receiverService;
      this.config = config.Value;
    }

    /// <summary>
    /// 送信対象者リストを取得
    /// </summary>
    public List<SelectListItem> SendTargetList
    {
      get
      {
        var sendTargetViewModels = receiverService.GetList().
          Select(item => new SendTargetViewModel { DisplayName = item.DisplayName, ID = item.ID });

        return sendTargetViewModels.Select(target => new SelectListItem(target.DisplayName, target.ID, false)).ToList();
      }
    }


    public void OnGet()
    {
      if (config.AllLogin && !HttpContext.Session.Keys.Contains(LoginModel.KEY_LOGIN_ID))
      {
        Response.Redirect("/Login");
        return;
      }
    }

    public IActionResult OnPost()
    {
      // 意見メッセージ保存
      try
      {
        messageService.Save(Message.SendTo, Message.Detail);

        // 保存OKの場合は完了メッセージページへ
        return RedirectToPage("/SendSuccess");
      }
      catch (System.IO.FileLoadException ex)
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
