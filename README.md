# 日志采集器 Demo

## 功能简介
1. 自动采集Log4Net日志
2. 提取整理关键资料(联合MySQL数据库)
3. 生成Excel报表

## 相关技术
1. DotNET Core 2.0
2. EF Core(for MySQL)
3. NPOI(for DotNet Core)

## 技术点
1. .net core文件读写在Mac等平台下无法读取Windows共享目录下的文件
2. .net core配置文件使用方式
 
+ .net core项目配置文件使用json格式。生成方式一定要设置为生成到输出目录
+ 配置文件读取借助以下两个Nuget包
    - [Microsoft.Extensions.Configuration](https://www.Nuget.org/packages/Microsoft.Extensions.Configuration/)
    - [Microsoft.Extensions.Configuration.Json](https://www.Nuget.org/packages/Microsoft.Extensions.Configuration.Json/)
+ 示例语法如下

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

3. .net core中文编码
+ .net core默认仅支持8种编码，使用不支持的编码会直接会抛异常
+ 使用GB2312等编码方式需要[System.Text.Encoding.CodePages](https://www.Nuget.org/packages/System.Text.Encoding.CodePages/) Nuget包
+ 在使用中文编码前注册第三方编码

``` csharp
//在使用编码前注册，建议在startup中全局注册
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
```
4. .net core 2.0引用Nuget包会在csproj文件中生成配置，没有package.json文件
5. Nuget包使用
+ Visual Studio for Mac中安装“Nuget Package Management Extensions”扩展之后可以像Windows中一样通过Nuget管理器界面安装Nuget包
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
