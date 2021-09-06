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
using System.Text.Json;

namespace IkenBako.Pages
{
  public class UserMaintenanceModel : PageModel
  {
    public const string KEY_USER_LIST = "KEY_USER_LIST";

    private readonly ILogger<UserMaintenanceModel> _logger;

    /// <summary>
    /// ���[�U�[�T�[�r�X
    /// </summary>
    private readonly UserService userService;

    /// <summary>
    /// ���M�҃T�[�r�X
    /// </summary>
    private readonly ReceiverService receiverService;

    public List<UserViewModel> Users { get; private set; } = new List<UserViewModel>();

    [BindProperty]
    public string RemoveItemsJson { set; get; }

    /// <summary>
    /// �R���X�g���N�^
    /// </summary>
    /// <param name="logger">���O�C���X�^���X</param>
    /// <param name="userService">���[�U�[���b�Z�[�W�T�[�r�X</param>
    /// <param name="receiverService">���M�҃T�[�r�X</param>
    public UserMaintenanceModel(ILogger<UserMaintenanceModel> logger, UserService userService, ReceiverService receiverService)
    {
      _logger = logger;
      this.userService = userService;
      this.receiverService = receiverService;
    }

    public void OnGet()
    {
      // TODO:�y�[�W�̌����`�F�b�N

      // ���[�U�[�ꗗ�擾
      var users = userService.GetList();
      var receivers = receiverService.GetList();
      var userMaintenanceModels = users.Select(user =>
      {
        var receiver = receivers.FirstOrDefault(receiver=>receiver.ID == user.ID);
        return new UserViewModel() {
          ID = user.ID,
          IsReceiver = receiver != null,
          DisplayName = receiver != null ? receiver.DisplayName : string.Empty,
          DisplayList = receiver != null && receiver.DisplayList,
          IsAdminRole = receiver != null && receiver.IsAdminRole
        };
      });

      // �Z�b�V�����Ɉꗗ���i�[
      var userList = userMaintenanceModels.ToList();
      HttpContext.Session.Set(KEY_USER_LIST, JsonSerializer.SerializeToUtf8Bytes(userList));

      Users.Clear();
      Users.AddRange(userList);

      // �[�����̏ꍇ�̓G���[
      if (!Users.Any())
      {
        ViewData["Message"] = "���[�U�[�͂���܂���B";
        return;
      }
    }

    public void OnPost()
    {
      // TODO:�y�[�W�̌����`�F�b�N

      // �ꗗ����
      if (HttpContext.Session.Keys.Contains(KEY_USER_LIST))
      {
        Users.Clear();
        var bytes = HttpContext.Session.Get(KEY_USER_LIST);
        Users.AddRange(JsonSerializer.Deserialize<List<UserViewModel>>(bytes));
      }

      if(!string.IsNullOrEmpty(RemoveItemsJson))
      {
        var json = this.RemoveItemsJson.Replace("\\", string.Empty).Replace("\"{", "{").Replace("}\"","}");
        var removeItems = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json);
      }
    }
  }
}
