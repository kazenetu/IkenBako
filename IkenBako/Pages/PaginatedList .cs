using System;
using System.Collections.Generic;
using System.Linq;

namespace IkenBako.Pages
{
  /// <summary>
  /// ページング機能付きList
  /// </summary>
  /// <typeparam name="T">リスト要素</typeparam>
  public class PaginatedList<T> : List<T>
  {
    /// <summary>
    /// 現在のページインデックス
    /// </summary>
    public int PageIndex { get; private set; }

    /// <summary>
    /// 総ページ数
    /// </summary>
    public int TotalPages { get; private set; }

    /// <summary>
    /// 前ページに遷移できるか
    /// </summary>
    public bool HasPreviousPage => PageIndex > 1;

    /// <summary>
    /// 次ページに遷移できるか
    /// </summary>
    public bool HasNextPage => PageIndex < TotalPages;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="items">対象リスト</param>
    /// <param name="count">要素数</param>
    /// <param name="pageIndex">ページインデックス</param>
    /// <param name="pageSize">1ページあたりの表示数</param>
    public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
    {
      PageIndex = pageIndex;
      TotalPages = (int)Math.Ceiling(count / (double)pageSize);

      this.AddRange(items);
    }

    /// <summary>
    /// ページング機能付きListの生成
    /// </summary>
    /// <param name="source">対象リスト</param>
    /// <param name="pageIndex">ページインデックス</param>
    /// <param name="pageSize">1ページあたりの表示数</param>
    /// <returns>ページング機能付きListインスタンス</returns>
    public static PaginatedList<T> Create(
            IQueryable<T> source, int pageIndex, int pageSize)
    {
      var count = source.Count();
      var totalPages = (int)Math.Ceiling(count / (double)pageSize);

      // 指定されたページインデックスが不正の場合はページインデックスを範囲内に変更する
      if (1 > pageIndex)
      {
        pageIndex = 1;
      }
      if (totalPages < pageIndex)
      {
        pageIndex = totalPages;
      }

      var items = source.Skip(
          (pageIndex - 1) * pageSize)
          .Take(pageSize).ToList();
      return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }
  }
}