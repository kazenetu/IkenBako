﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using static Infrastructures.DB.DatabaseFactory;

namespace Infrastructures.DB
{
  /// <summary>
  /// SQLServerラッパークラス
  /// </summary>
  public class SQLServerDB : IDatabase
  {
    /// <summary>
    /// セーブポイント名
    /// </summary>
    private const string SavePointName = "SAVE";

    #region プライベートフィールド

    /// <summary>
    /// コネクションインスタンス
    /// </summary>
    private SqlConnection conn = null;

    /// <summary>
    /// トランザクションインスタンス
    /// </summary>
    private SqlTransaction tran = null;

    /// <summary>
    /// パラメータ
    /// </summary>
    private readonly Dictionary<string, object> param;

    /// <summary>
    /// トランザクションが開いているか否か
    /// </summary>
    private bool isTran = false;

    /// <summary>
    /// セーブポイントを設定しているか否か
    /// </summary>
    private bool isSetSavePoint = false;

    #endregion

    #region プロパティ

    /// <summary>
    /// DB種類
    /// </summary>
    public DatabaseTypes DBType
    {
      get
      {
        return DatabaseTypes.sqlserver;
      }
    }
    #endregion

    #region パブリックメソッド

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="connectionString">接続文字列</param>
    public SQLServerDB(string connectionString)
    {
      try
      {
        this.conn = this.GetConnection(connectionString);
        this.conn.Open();

        this.param = new Dictionary<string, object>();
      }
      catch (Exception ex)
      {
        throw new Exception($"「{connectionString}」への接続失敗", ex);
      }
    }

    /// <summary>
    /// SELECTした行の一部だけを取り出すSQLの文字列を返す
    /// </summary>
    /// <param name="limit">取り出す行数</param>
    /// <param name="offset">スキップ行</param>
    /// <returns></returns>
    public string GetLimitSQL(long limit, long offset = 0)
    {
      return $" offset {offset} rows fetch next {limit} rows only ";
    }

    /// <summary>
    /// パラメータを追加
    /// </summary>
    /// <param name="key">キー</param>
    /// <param name="value">値</param>
    public void AddParam(string key, object value)
    {
      this.param.Add(key, value);
    }

    /// <summary>
    /// パラメータをクリア
    /// </summary>
    public void ClearParam()
    {
      this.param.Clear();
    }

    /// <summary>
    /// SQL実行（登録・更新・削除）
    /// </summary>
    /// <param name="sql">SQLステートメント</param>
    /// <returns>処理件数</returns>
    public int ExecuteNonQuery(string sql)
    {
      // SQL未設定
      if (sql == string.Empty)
      {
        return 0;
      }

      // SQL発行
      using (SqlCommand command = conn.CreateCommand())
      {
        command.CommandText = sql;
        command.Connection = this.conn;
        if (isTran)
        {
          command.Transaction = this.tran;
        }

        foreach (var key in this.param.Keys)
        {
          command.Parameters.AddWithValue(key, this.param[key]);
        }

        return command.ExecuteNonQuery();
      }
    }

    /// <summary>
    /// 検索SQL実行
    /// </summary>
    /// <param name="sql">SQLステートメント</param>
    /// <returns>検索結果</returns>
    public DataTable Fill(string sql)
    {
      // SQL未設定
      if (sql == string.Empty)
      {
        return new DataTable();
      }

      // SQL発行
      using (SqlCommand command = conn.CreateCommand())
      {
        command.CommandText = sql;
        command.Connection = this.conn;
        if (isTran)
        {
          command.Transaction = this.tran;
        }

        foreach (var key in this.param.Keys)
        {
          command.Parameters.AddWithValue(key, this.param[key]);
        }

        using (SqlDataReader reader = command.ExecuteReader())
        {
          //スキーマ取得
          var result = this.GetShcema(reader);

          while (reader.Read())
          {
            var addRow = result.NewRow();

            foreach (DataColumn col in result.Columns)
            {
              addRow[col.ColumnName] = reader[col.ColumnName];
            }

            result.Rows.Add(addRow);
          }

          return result;
        }
      }
    }

    /// <summary>
    /// トランザクション設定
    /// </summary>
    public void BeginTransaction()
    {
      if (isTran && isSetSavePoint)
      {
        throw new Exception("トランザクションが設定できませんでした。");
      }

      if (isTran)
      {
        tran.Save(SavePointName);
        isSetSavePoint = true;
      }
      else
      {
        this.tran = this.conn.BeginTransaction();
        this.isTran = true;
      }
    }

    /// <summary>
    /// コミット
    /// </summary>
    public void Commit()
    {
      if (!this.isTran)
      {
        return;
      }

      if (isSetSavePoint)
      {
        isSetSavePoint = false;
      }
      else
      {
        this.tran.Commit();
        this.isTran = false;
      }
    }

    /// <summary>
    /// ロールバック
    /// </summary>
    public void Rollback()
    {
      if (!this.isTran)
      {
        return;
      }

      if (isSetSavePoint)
      {
        tran.Rollback(SavePointName);
        isSetSavePoint = false;
      }
      else
      {
        this.tran.Rollback();
        this.isTran = false;
      }
    }

    /// <summary>
    /// 解放処理
    /// </summary>
    public void Dispose()
    {
      if (isSetSavePoint)
      {
        tran.Rollback(SavePointName);
        isSetSavePoint = false;
      }
      if (this.isTran)
      {
        this.tran.Rollback();
      }
      if (this.tran != null)
      {
        this.tran.Dispose();
      }
      this.conn.Close();
      this.conn.Dispose();
    }

    #endregion

    #region プライベートメソッド

    /// <summary>
    /// コネクション生成
    /// </summary>
    /// <param name="connectionString">接続文字列</param>
    /// <returns>コネクションインスタンス</returns>
    private SqlConnection GetConnection(string connectionString)
    {
      return new SqlConnection(connectionString);
    }

    /// <summary>
    /// スキーマ取得
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    private DataTable GetShcema(SqlDataReader reader)
    {
      var result = new DataTable();

      for (var i = 0; i < reader.FieldCount; i++)
      {
        var dataType = reader.GetFieldType(i);
        result.Columns.Add(new DataColumn(reader.GetName(i), dataType));
      }

      return result;
    }
    #endregion
  }
}
