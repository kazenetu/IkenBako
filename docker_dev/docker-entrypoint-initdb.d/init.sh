set -e
psql testDB -U test test << EOSQL

DROP TABLE IF EXISTS m_receiver;
DROP TABLE IF EXISTS t_message;

CREATE TABLE m_receiver(
 unique_name varchar(255) NOT NULL,
 fullname varchar(255) NOT NULL,
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

INSERT INTO m_receiver("unique_name", "fullname") VALUES ('aa', 'Aさん');
INSERT INTO m_receiver("unique_name", "fullname") VALUES ('bb', 'Bさん');
INSERT INTO m_receiver("unique_name", "fullname") VALUES ('cc', 'Cさん');


EOSQL