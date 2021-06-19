using Domain.Application;
using IkenBako.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IkenBako.Pages
{
  public class MessageListsModel : PageModel
  {
    private readonly ILogger<MessageListsModel> _logger;

    /// <summary>
    /// 意見メッセージサービス
    /// </summary>
    private readonly MessageService messageService;

    /// <summary>
    /// 送信者サービス
    /// </summary>
    private readonly ReceiverService receiverService;

    public List<MessageViewModel> Messages { get; private set; } = new List<MessageViewModel>();

    /// <summary>
    /// 確認者
    /// </summary>
    [BindProperty]
    public string Target { set; get; }

    /// <summary>
    /// すべての意見メッセージを表示するか否か
    /// </summary>
    public bool ShowAllMessage { get; private set; } = false;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="logger">ログインスタンス</param>
    /// <param name="messageService">意見メッセージサービス</param>
    /// <param name="receiverService">送信者サービス</param>
    public MessageListsModel(ILogger<MessageListsModel> logger, MessageService messageService, ReceiverService receiverService)
    {
      _logger = logger;
      this.messageService = messageService;
      this.receiverService = receiverService;
    }

    /// <summary>
    /// 送信対象者リストを取得
    /// </summary>
    public List<SelectListItem> SendTargetList
    {
      get
      {
        var target = HttpContext.Session.GetString(LoginModel.KEY_LOGIN_ID);
        if(target is null) target = string.Empty;
        var sendTargetViewModels = receiverService.GetList(true).
          Where(item=>item.ID == target).
          Select(item => new SendTargetViewModel { DisplayName = item.DisplayName, ID = item.ID });

        return sendTargetViewModels.Select(target => new SelectListItem(target.DisplayName, target.ID, false)).ToList();
      }
    }

    public void OnGet()
    {
      if (!HttpContext.Session.Keys.Contains(LoginModel.KEY_LOGIN_ID))
      {
        Response.Redirect("/Login");
        return;
      }
      var target = HttpContext.Session.GetString(LoginModel.KEY_LOGIN_ID);
      if(receiverService.GetReceiver(target) is null){
        Response.Redirect("/Login");
        return;
      }
    }

    public void OnPost()
    {
      if (Target is null)
      {
        Response.Redirect("/Login");
        return;
      }
      // 対象の意見メッセージを取得
      Messages = messageService.Find(Target).Select(item => new MessageViewModel
      {
        SendTo = item.SendTo,
        SendTargetName = item.SendTargetName,
        Detail = item.Detail
      }).ToList();

      // メッセージがない場合はその旨のエラーメッセージを設定
      if (!Messages.Any())
      {
        ViewData["Message"] = "意見メッセージはありません。";
        return;
      }

      // すべての意見メッセージを表示するか否か
      ShowAllMessage = receiverService.IsAllReceiverId(Target);

    }
  }
}