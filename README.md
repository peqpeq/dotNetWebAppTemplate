# 

How to create postgresql database with ubuntu
1) Login into postgre user
`sudo -i -u postgres`
2) Create database
`createdb <database name>`
3) Login into PSQL cli 
`psql`
4) Create role
` CREATE USER <user_name> WITH PASSWORD <user_password>;`
5) Grant privileges
`GRANT ALL PRIVILEGES ON DATABASE <database name> TO <user_name>;`

