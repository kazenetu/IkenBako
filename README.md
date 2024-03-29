# 簡易意見箱
**匿名**で上司などに意見書を送信できる意見箱システムです。  
小規模な組織で使う目的で実装しています。

## 特徴
### 機能
* 投稿者用(匿名投稿者)
  * 意見投稿機能

* 受信者用(意見書受信者:上司など)
  * メッセージ一覧機能

* ログインユーザー用
  * パスワード変更機能

* 管理者用
  * ユーザーメンテナンス機能

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

## Dockerでの実行
dockerがインストール済みであること  
また、下記のファイルを利用する
* docker/dockerfile
* docker/docker-compose.yml

### 手順
1. 本リポジトリをclone
2. ```cd docker```を実行
3. 初回のみビルドを実行  
   ```docker-compose build```
4. コンテナ起動（アプリケーションの実行）  
   ```docker-compose up -d```
5. コンテナ停止・削除（アプリケーションの終了）
   ```docker-compose down```

## Dockerコンテナ上での開発
Dockerコンテナ上で.NET Core開発環境を構築する  
dockerがインストール済みであること  
また、下記のファイルを利用する  
* docker_dev/dotnet_dockerfile  
  .NET Core用
* docker_dev/postgresql_dockerfile  
  PostgreSQL用
* docker_dev/docker-entrypoint-initdb.d/init.sh  
  PostgreSQL初回実行用SQL
* docker_div/docker-compose.yml

### 手順
1. 本リポジトリをclone
2. ```cd docker_div```を実行
3. 初回のみビルドを実行  
   ```docker-compose build```
4. コンテナ起動  
   ```docker-compose up -d```
5. コンテナ停止・削除
   ```docker-compose down```

※Visual Studio Codeの拡張機能「Docker for Visual Studio Code」の利用を推奨する

## 利用可能DB
下記DBの利用が可能。
* SQLite  
  ※サンプルとして「IkenBako/Resource/Test.db」をビルド時にコピーしている
* PostgreSQL
* SQLServer

### DB設定
「IkenBako/appsettings.json」または環境変数に記述する  
* appsettings.json  
  ```json
  "DB": {
    "ConnectionStrings": {
      "sqlite": "Resource/Test.db",
      "postgres": "Server=postgresql_server;Port=5432;User Id=test;Password=test;Database=testDB",
      "sqlserver": "Data Source=.\\SQLEXPRESS;Database=master;Integrated Security=True;"
    },
    "Target": "sqlite"
  }
  ```  
  * DB/ConnectionStrings  
    DBごとの接続文字列(SQLiteはファイルパス)
  * DB/Target  
    利用するDB(sqlite/postgres/sqlserver)  

* 環境変数  
  コロン(:)で区切る
  ```YAML
   "DB:ConnectionStrings:sqlite": "Resource/Test.db"
   "DB:ConnectionStrings:postgres": "Server=postgresql_server;Port=5432;User Id=test;Password=test;Database=testDB"
   "DB:Target": "sqlite"
  ```  

### アプリケーション設定
「IkenBako/appsettings.json」または環境変数に記述する  
* appsettings.json  
  ```json
  "Setting": {
    "AllLogin": "true"
  }
  ```  
  * Setting/AllLogin  
    すべてのユーザーはログイン必須(true/false)  
    省略時：true

* 環境変数  
  コロン(:)で区切る
  ```YAML
   "Setting:AllLogin": "true"
  ```  

### テーブルレイアウト  
現時点のテーブルレイアウトは下記の通り

**m_user**(ユーザーマスタ：ログイン用マスタ)
|カラム名|論理名|型|NOT NULL|備考|
|-------|-------|-----|:----:|-------|
|unique_name|ユニークな略称|varchar(255)|〇|主キー|
|password|暗号化したパスワード|varchar(255)|〇||
|salt|暗号化パラメータ|varchar(255)|〇||
|version|更新バージョン|integer|〇|default 1|

**m_receiver**(受信者マスタ)
|カラム名|論理名|型|NOT NULL|備考|
|-------|-------|-----|:----:|-------|
|unique_name|ユニークな略称|varchar(255)|〇|主キー|
|fullname|氏名|varchar(255)|〇||
|display_list|リスト表示可否|boolean|〇|default true|
|is_admin_role|管理者権限|boolean|〇|default false|
|is_viewlist_role|一覧確認権限|boolean|〇|default true|
|version|更新バージョン|integer|〇|default 1|

**t_message**(メッセージテーブル)
|カラム名|論理名|型|NOT NULL|備考|
|-------|-------|-----|:----:|-------|
|id|連番|SERIAL|〇|主キー|
|send_to|送信対象のユニークな略称|varchar(255)|〇||
|detail|送信メッセージ|varchar(255)|〇||

## 画面イメージ
* ログイン画面(AllLoginがfalseの場合は一般ユーザーはログイン不要)  
  ![](doc/images/login.png)  

* メニュー種類
  * 未ログイン時(AllLoginがfalseの場合の一般ユーザー)  
    ![](doc/images/menu_not_login.png)  
  * ログイン時:一般ユーザー  
    ![](doc/images/menu_user.png)  
  * ログイン時:受信者ユーザー  
    ![](doc/images/menu_receiver.png)  
  * ログイン時:管理者ユーザー  
    ![](doc/images/menu_admin.png)  

* 意見投稿画面  
  ![](doc/images/send_message.png)  

* メッセージ一覧
  * メッセージ受信者対象選択  
    ![](doc/images/message_list1.png)  

  * メッセージ一覧表示  
    ![](doc/images/message_list2.png)  

  * メッセージなし  
    ![](doc/images/message_list3.png)  

* ユーザーメンテナンス  
  ![](doc/images/userMainte.png)  

* パスワード変更  
  ![](doc/images/changePassword.png)  

## 使用パッケージとライセンス通知
本ソフトウェアで使用しているパッケージとライセンス通知は[THIRD-PARTY-NOTICES.txt](THIRD-PARTY-NOTICES.txt)を参照。

## 利用ツール
* [nuget-license](https://github.com/tomchavakis/nuget-license)  
  プロジェクトで使用しているパッケージのライセンス調査・出力ツール