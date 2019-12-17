# 簡易意見箱
**匿名**で上司などに意見書を送信できる意見箱システムです。  
小規模な組織で使う目的で実装しています。

## 特徴
### 機能
* 投稿者用(匿名投稿者)
  * 意見メッセージ投稿機能

* 受信者用(意見書受信者:上司など)
  * 意見メッセージ一覧機能

### 注意事項
* テキストファイルで永続化(DB未使用)
* 意見メッセージ一覧用のログイン機能なし

## 開発環境
* Visual Studio 2019 Community
* .NET Core 3.1.100

## 実行環境
* .NET Core 3.1.100

## 実行方法
* Visual Studio(2019以上)を利用する場合  
   IkenBako.slnを開いて実行

* dotnetコマンドを利用する場合  
   ```sh
   #IkenBako/IkenBako.csprojを実行(ctrl+cで終了)
   dotnet run --project ./IkenBako/IkenBako.csproj

   # 例：ブラウザで http://localhost:5000 にアクセス
   #
   #出力例：
   # info: Microsoft.Hosting.Lifetime[0]
   #       Now listening on: http://localhost:5000
   #
   # ・・・(以下略)・・・

  ```

## 設定フォルダ・出力フォルダ
**簡易版のため、すべてテキストファイルで永続化**  
※DBに格納する場合は下記を実行する。  
  　・Infrastructures内にDB利用Repositoryクラスを追加  
  　・Startup#ConfigureServicesメソッドでDI設定変更  
  　・接続情報の設定と取得処理を追加

### 設定フォルダ  
   **Receivers/** *ID(物理ファイル名)*.txt  
   ※内容は**選択表示用名**  
     
   例：aa.txt  
   ```
   Aさん
   ```

### 出力フォルダ  
   **SendResult** */送信者ID/* *送信日時*.txt  
   ※内容は**意見メッセージJSON**(プロパティ名と値のペアリスト)  
     
   例：20191116131403.txt
   ```json
   [{"Key":"SendTo","Value":"aa"},{"Key":"Detail","Value":"11"}]
   ```

## 画面イメージ
* 意見投稿画面  
  ![](doc/images/send_message.png)  

* 意見一覧：メッセージ受信者対象選択  
   ![](doc/images/message_list1.png)  

* 意見一覧：メッセージ一覧  
  ![](doc/images/message_list2.png)  

* 意見一覧：メッセージなし  
  ![](doc/images/message_list3.png)  
