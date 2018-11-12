# 日志采集器分析

## 功能简介
1. 自动采集Log4Net日志
2. 提取整理关键资料(联合MySQL数据库)
3. 生成Excel报表

## 相关技术
1. DotNET Core 2.0
2. EF Core(for MySQL)
3. NPOI(for DotNet Core)

## 技术点
1. .net core文件读写在Mac等平台下无法读取Windows共享目录下的文件
2. .net core配置文件使用方式
 
+ .net core项目配置文件使用json格式。生成方式一定要设置为生成到输出目录
+ 配置文件读取借助以下两个Nuget包
    - [Microsoft.Extensions.Configuration](https://www.Nuget.org/packages/Microsoft.Extensions.Configuration/)
    - [Microsoft.Extensions.Configuration.Json](https://www.Nuget.org/packages/Microsoft.Extensions.Configuration.Json/)
+ 示例语法如下

    **appsettings.json**
    ``` json
    {
        "ConnectionStrings": {
            "TestConnection": "server=192.168.0.202;port=3307;uid=sevenstar;pwd=ss@2016#07;DataBase=sevenstar_hf_director2;charset=utf8;max pool size=5000;sslmode=none;"
        }
    }
    ```

    **C#**
    ``` csharp
    var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    var configuration = builder.Build();
    string connectionString = configuration.GetConnectionString("TestConnection");
    ```

3. .net core中文编码
+ .net core默认仅支持8种编码，使用不支持的编码会直接会抛异常
+ 使用GB2312等编码方式需要[System.Text.Encoding.CodePages](https://www.Nuget.org/packages/System.Text.Encoding.CodePages/) Nuget包
+ 在使用中文编码前注册第三方编码

``` csharp
//在使用编码前注册，建议在startup中全局注册
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
```
4. .net core 2.0引用Nuget包会在csproj文件中生成配置，没有package.json文件
5. Nuget包使用
+ Visual Studio for Mac中安装“Nuget Package Management Extensions”扩展之后可以像Windows中一样通过Nuget管理器界面安装Nuget包
+ Visual Studio for Mac中安装“PowerShell"扩展后可以像Windows中一样使用包管理控制台通过命令 `PM> Install-Package xxx`安装Nuget包
+ 直接过个dotnet命令安装Nuget包。终端中转到项目目录下执行 `> dotnet add package xxx`安装Nuget包
+ 直接在csproj文件添加Nuget包引用，然后还原Nuget包（编译项目也会自动还愿Nuget包），引用语法如下：

**csproj文件**
``` xml
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <PackageReference Include="DotNetCore.NPOI" Version="1.0.0" />
  </ItemGroup>
</Project>
```
> 相关Nuget包
>> 1. [DotNetCore.NPOI](https://www.nuget.org/packages/DotNetCore.NPOI/)
>> 2. [MySql.Data.EntityFrameworkCore](https://www.nuget.org/packages/MySql.Data.EntityFrameworkCore/8.0.8-dmr)
