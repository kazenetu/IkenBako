set -e
psql -d testDB -U test << EOSQL

DROP TABLE IF EXISTS m_user;
DROP TABLE IF EXISTS m_receiver;
DROP TABLE IF EXISTS t_message;

-- ユーザーマスタ
CREATE TABLE m_user(
 unique_name varchar(255) NOT NULL,
 password varchar(255) NOT NULL,
 salt varchar(255) NOT NULL,
 primary key(unique_name)
);

-- 上司マスタ
CREATE TABLE m_receiver(
 unique_name varchar(255) NOT NULL,
 fullname varchar(255) NOT NULL,
 display_list boolean default true,
 is_admin_role boolean default false,
 primary key(unique_name)
);

-- メッセージテーブル
CREATE TABLE t_message(
 id SERIAL NOT NULL,
 send_to varchar(255) NOT NULL,
 detail text NOT NULL,
 primary key(id)
);

comment on table m_user              is 'ユーザーマスタ';
comment on column m_user.unique_name is 'ユニークな略称';
comment on column m_user.password    is '暗号化したパスワード';
comment on column m_user.salt        is '暗号化パラメータ';

comment on table m_receiver                is '上司マスタ';
comment on column m_receiver.unique_name   is 'ユニークな略称';
comment on column m_receiver.fullname      is '氏名';
comment on column m_receiver.display_list  is 'リスト表示可否';
comment on column m_receiver.is_admin_role is '管理者権限';

comment on table t_message                     is 'メッセージテーブル';
comment on column t_message.id                 is '連番';
comment on column t_message.send_to            is '送信対象の上司略称';
comment on column t_message.detail             is '送信メッセージ';

-- ユーザーマスタ登録
INSERT INTO m_user("unique_name", "password", "salt") 
VALUES ('aa', '09Dg4qWzhtrppNMV6eNbRLb0Cdxy5kH47Qi8O9BzRqU=', 'E6b0z+WEzgNd9mZ6CS5/+w==');

INSERT INTO m_user("unique_name", "password", "salt") 
VALUES ('bb', 'ti+jOc88+oWfm3aIkg9XGq0No6akBADu6S2fwAImVXQ=', '6toFigaiTXNUVHzBDlDSqw==');

INSERT INTO m_user("unique_name", "password", "salt") 
VALUES ('cc', 'q6cQlhHmvFMBcmvnM3ON61a+qLGzCE3dfcFIpCS4nh4=', 'sXoo48uAMbxMJA9HYifoAA==');

INSERT INTO m_user("unique_name", "password", "salt") 
VALUES ('guest', 'XN+YezObIVLTzVT3jBNtrQSfjtaHXL5zWqW/ci50u6g=', '7Hi5ZraAGng7gQRD6r/bqw==');

INSERT INTO m_user("unique_name", "password", "salt") 
VALUES ('admin', 're/aFXSjfuKtSeBkCoxAImeQ2Mb6fbunb30EmXpODLc=', ' R2ujDXTjQvUbD4BQRvtbdQ==');

-- 上司マスタ登録
INSERT INTO m_receiver("unique_name", "fullname") 
VALUES ('aa', 'Aさん');

INSERT INTO m_receiver("unique_name", "fullname") 
VALUES ('bb', 'Bさん');

INSERT INTO m_receiver("unique_name", "fullname") 
VALUES ('cc', 'Cさん');

INSERT INTO m_receiver("unique_name", "fullname", "display_list", "is_admin_role") 
VALUES ('admin', '管理者', false, true);

EOSQL