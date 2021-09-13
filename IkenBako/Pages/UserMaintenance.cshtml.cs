using Domain.Application;
using Domain.Domain.Receivers;
using Domain.Domain.Users;
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
    private const string KEY_USER_LIST = "KEY_USER_LIST";
    private const int VERSION_NONE = -1;

    private readonly ILogger<UserMaintenanceModel> _logger;

    /// <summary>
    /// ���[�U�[�T�[�r�X
    /// </summary>
    private readonly UserService userService;

    /// <summary>
    /// ���M�҃T�[�r�X
    /// </summary>
    private readonly ReceiverService receiverService;

    /// <summary>
    /// ���[�U�[���X�g
    /// </summary>
    public List<UserViewModel> Users { get; private set; } = new List<UserViewModel>();

    /// <summary>
    /// �ҏW���[�U�[���
    /// </summary>
    [BindProperty]
    public UserViewModel EditTarget { set; get; } = new UserViewModel();

    /// <summary>
    /// �ҏW���ۂ�
    /// </summary>
    [BindProperty]
    public bool IsEdit { set; get; } = false;

    /// <summary>
    /// �p�X���[�h�ݒ�����{���邩�ۂ�
    /// </summary>
    [BindProperty]
    public bool EditIsSetPassword { get; set; } = false;

    /// <summary>
    /// �p�X���[�h
    /// </summary>
    [BindProperty]
    public string EditPassword { get; set; } = "";

    /// <summary>
    /// �ҏW���̃��[�U�[�}�X�^�̃o�[�W����
    /// </summary>
    [BindProperty]
    public int EditTargetUserVersion { set; get; } = VERSION_NONE;

    /// <summary>
    /// �ҏW���̎�M�҃}�X�^�̃o�[�W����
    /// </summary>
    [BindProperty]
    public int EditTargetReceiverVersion { set; get; } = VERSION_NONE;

    /// <summary>
    /// �ύX/�V�K�{�^���̃{�^����
    /// </summary>
    public string SaveButtonText { get { return IsEdit ? "�ύX" : "�V�K�쐬"; } }

    /// <summary>
    /// �폜�Ώۃ��[�U�[���X�g�pJSON
    /// </summary>
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

    /// <summary>
    /// �y�[�W�\��
    /// </summary>
    public void OnGet()
    {
      // TODO:�y�[�W�̌����`�F�b�N

      // ���[�U�[�ꗗ�擾
      var userMaintenanceModels = userService.GetList().Select(user =>
      {
        return new UserViewModel() {
          ID = user.ID,
          IsReceiver = user.Receiver != null,
          DisplayName = user.Receiver != null ? user.Receiver.DisplayName : string.Empty,
          DisplayList = user.Receiver != null && user.Receiver.DisplayList,
          IsAdminRole = user.Receiver != null && user.Receiver.IsAdminRole
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

    /// <summary>
    /// ���[�U�[�폜
    /// </summary>
    public void OnPostRemove()
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

        // TODO �폜
      }
    }

    /// <summary>
    /// ���[�U�[�ꗗ����ҏW�N���b�N
    /// </summary>
    /// <param name="id">���[�U�[ID</param>
    public void OnPostEdit(string id)
    {
      // TODO:�y�[�W�̌����`�F�b�N

      // �ꗗ����
      if (HttpContext.Session.Keys.Contains(KEY_USER_LIST))
      {
        Users.Clear();
        var bytes = HttpContext.Session.Get(KEY_USER_LIST);
        Users.AddRange(JsonSerializer.Deserialize<List<UserViewModel>>(bytes));
      }

      if (string.IsNullOrEmpty(id))
      {
        return;
      }

      // ���[�U�[�𔽉f
      IsEdit = true;
      var editTarget = userService.GetUser(id);
      var receiver = receiverService.GetReceiver(id);
      EditTarget.ID = editTarget.ID;
      EditTargetUserVersion = editTarget.Version;
      EditTargetReceiverVersion = -1;
      if (receiver != null)
      {
        EditTarget.IsReceiver = true;
        EditTarget.DisplayName = receiver.DisplayName;
        EditTarget.DisplayList = receiver.DisplayList;
        EditTarget.IsAdminRole= receiver.IsAdminRole;
        EditTargetReceiverVersion = receiver.Version;
      }
    }

    /// <summary>
    /// �ҏW���ڃN���A
    /// </summary>
    public void OnPostClear()
    {
      // TODO:�y�[�W�̌����`�F�b�N

      // �ꗗ����
      if (HttpContext.Session.Keys.Contains(KEY_USER_LIST))
      {
        Users.Clear();
        var bytes = HttpContext.Session.Get(KEY_USER_LIST);
        Users.AddRange(JsonSerializer.Deserialize<List<UserViewModel>>(bytes));
      }

      // �ҏW���ڂ��N���A
      IsEdit = false;
      EditPassword = string.Empty;
      EditIsSetPassword = false;
      EditTarget.ID = string.Empty;
      EditTarget.IsReceiver = false;
      EditTarget.DisplayName = string.Empty;
      EditTarget.DisplayList = false;
      EditTarget.IsAdminRole = false;
      EditTargetUserVersion = VERSION_NONE;
      EditTargetReceiverVersion = VERSION_NONE;
    }

    /// <summary>
    /// �ۑ�
    /// </summary>
    public IActionResult OnPostSave()
    {
      // TODO:�y�[�W�̌����`�F�b�N

      // �ꗗ����
      if (HttpContext.Session.Keys.Contains(KEY_USER_LIST))
      {
        Users.Clear();
        var bytes = HttpContext.Session.Get(KEY_USER_LIST);
        Users.AddRange(JsonSerializer.Deserialize<List<UserViewModel>>(bytes));
      }

      // ���̓`�F�b�N
      var errorMessages = new List<string>();
      if (IsEdit)
      {
        // �������[�U�[
        if(string.IsNullOrEmpty(EditTarget.ID) || EditTargetUserVersion == VERSION_NONE)
        {
          errorMessages.Add("���[�U�[���������擾����܂���ł����B");
        }
      }
      else
      {
        // �V�K�o�^
        if(string.IsNullOrEmpty(EditTarget.ID))
        {
          errorMessages.Add("ID����͂��Ă��������B");
        }
        else
        {
          // �����Ƀ��[�U�[�Ɠ������m�F
          if(userService.GetUser(EditTarget.ID) != null)
          {
            errorMessages.Add("ID��ύX���Ă��������B���łɃ��[�U�[�����݂��܂��B");
          }
        }

        // �p�X���[�h
        if (!EditIsSetPassword || string.IsNullOrEmpty(EditPassword))
        {
          errorMessages.Add("�p�X���[�h�͕K�{�ł��B");
        }
      }
      // ����
      if (!string.IsNullOrEmpty(EditTarget.ID) && EditTarget.ID.Trim() == ReceiverId.AllReceiverId)
      {
        errorMessages.Add($"ID��{ReceiverId.AllReceiverId}�͎g���܂���B");
      }
      if (EditIsSetPassword && string.IsNullOrEmpty(EditPassword))
      {
        errorMessages.Add("�p�X���[�h����͂��Ă��������B");
      }
      if (EditTarget.IsReceiver)
      {
        if (string.IsNullOrEmpty(EditTarget.DisplayName))
        {
          errorMessages.Add("��M�Җ�����͂��Ă��������B");
        }
      }
      if (errorMessages.Any())
      {
        ViewData["ErrorMessages"] = errorMessages;
        return Page();
      }

      // TODO �ۑ�����

      // �o�^�������͈ꗗ�̍ĕ\��
      return RedirectToPage();
    }
  }
}
