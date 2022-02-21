# 命令行参数说明  
##  /local:<zh-CN | en-US>   命令行工具的语言环境(默认zh-CN)  
##  <mssql | npgsql | sqlite3 | mysql<:engine>   其中 MySQL 数据库的存储引擎参数使用冒号指定，默认（即不指定时）为 InnoDB  
##        示例指定数据库为 MySQL 并使用 MyISAM 存储引擎： dotnet tool WDA-CF mysql:MyISAM  