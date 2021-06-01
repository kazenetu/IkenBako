﻿using Infrastructures.DB;
using Microsoft.Extensions.Options;
using System;

namespace Infrastructures
{
  /// <summary>
  /// Repositoryのスーパークラス
  /// </summary>
  public class RepositoryBase : IRepositoryBase, IDisposable
  {
    /// <summary>
    /// データを永続化するDB用インターフェース
    /// </summary>
    protected IDatabase db;

    /// <summary>
    /// DB情報
    /// </summary>
    private IOptions<DatabaseConfigModel> config;

    /// <summary>
    /// DB設定取得用コンストラクタ
    /// </summary>
    /// <param name="config">DB設定取得</param>
    public RepositoryBase(IOptions<DatabaseConfigModel> config)
    {
      this.config = config;
      Initialize();
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Initialize()
    {
      // 設定されていない場合は終了
      if(config == null)
      {
        return;
      }

      // DatabaseFactoryから永続化対象のDBインスタンスを取得
      db = DatabaseFactory.Create(config.Value);
    }

    /// <summary>
    /// DBインスタンス設定用コンストラクタ
    /// </summary>
    /// <param name="db">外部から設定されたDBインスタンス</param>
    public RepositoryBase(IDatabase db)
    {
      this.db = db;
    }

    /// <summary>
    /// 破棄
    /// </summary>
    public void Dispose()
    {
      db?.Dispose();
    }

    /// <summary>
    /// トランザクション設定
    /// </summary>
    public void BeginTransaction()
    {
      db.BeginTransaction();
    }

    /// <summary>
    /// コミット
    /// </summary>
    public void Commit()
    {
      db.Commit();
    }

    /// <summary>
    /// ロールバック
    /// </summary>
    public void Rollback()
    {
      db.Rollback();
    }


    /// <summary>
    /// サブクラスへのキャスト
    /// </summary>
    /// <typeparam name="T">サブクラス</typeparam>
    /// <returns>サブクラスのインスタンス</returns>
    public virtual T Cast<T>() where T : IRepositoryBase
    {
      // インスタンスを作成して返す
      return (T)Activator.CreateInstance(typeof(T), db);
    }
  }
}
