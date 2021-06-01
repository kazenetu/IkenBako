set -e
psql testDB -U test test << EOSQL

DROP TABLE IF EXISTS m_receiver;
DROP TABLE IF EXISTS t_message;

CREATE TABLE m_receiver(
 unique_name varchar(255) NOT NULL,
 fullname varchar(255) NOT NULL,
 password varchar(255) NOT NULL,
 salt varchar(255) NOT NULL,
 primary key(unique_name)
);

CREATE TABLE t_message(
 id SERIAL NOT NULL,
 send_to varchar(255) NOT NULL,
 detail text NOT NULL,
 primary key(id)
);

comment on column m_receiver             is '上司マスタ';
comment on column m_receiver.unique_name is 'ユニークな略称';
comment on column m_receiver.fullname    is '氏名';

comment on column t_message                    is 'メッセージテーブル';
comment on column t_message.id                  is '連番';
comment on column t_message.send_to            is '送信対象の上司名';
comment on column t_message.detail             is '送信メッセージ';

INSERT INTO m_receiver("unique_name", "fullname", "password", "salt") 
VALUES ('aa', 'Aさん', '09Dg4qWzhtrppNMV6eNbRLb0Cdxy5kH47Qi8O9BzRqU=', 'E6b0z+WEzgNd9mZ6CS5/+w==');

INSERT INTO m_receiver("unique_name", "fullname", "password", "salt") 
VALUES ('bb', 'Bさん', 'ti+jOc88+oWfm3aIkg9XGq0No6akBADu6S2fwAImVXQ=', '6toFigaiTXNUVHzBDlDSqw==');

INSERT INTO m_receiver("unique_name", "fullname", "password", "salt") 
VALUES ('cc', 'Cさん', 'q6cQlhHmvFMBcmvnM3ON61a+qLGzCE3dfcFIpCS4nh4=', 'sXoo48uAMbxMJA9HYifoAA==');

EOSQL