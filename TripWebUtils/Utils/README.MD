.net 6.0 版本

# 需要安装的包：
1. StackExchange.Redis 2.6.96
2. protobuf-net 3.2.0
3. Newtonsoft.Json 12.0.1
4. Microsoft.Extensions.Configuration 6.0
5. Microsoft.Extensions.Configuration.Json 6.0


# 如何使用

所有的Redis 操作，都使用CacheManager类

1. 连接字符串改成从配置文件中读取
    ```c#
   static CacheManager()
   {
        // 需要安装的包：
        // Microsoft.Extensions.Configuration 6.0
        // Microsoft.Extensions.Configuration.Json 6.0
        ConfigurationBuilder configuration = new ConfigurationBuilder(); //读取配置文件
        var config = configuration.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(file =>
        {
            file.Path = "/appsettings.json";
            file.Optional = false;
            file.ReloadOnChange = true;
    	 }).Build();
    	 RedisClient.StartConnect(config["Redis"]);
    }