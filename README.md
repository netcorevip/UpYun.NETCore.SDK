# UpYun.NETCore.SDK
又拍云SDK .NET Core版
NuGet安装指令：   

```C#
Install-Package UpYun.NETCore.SDK
```
NuGet地址：



#### 支持功能

- [x] 签名认证，默认Basic认证

- [x] 文件(图片)上传
- [x] 上传文件+访问密码+文件MD5验证
- [x] 上传图片+文字水印
- [x] 上传图片+图片水印
- [x] 上传文件+加水印+访问密码+文件MD5验证
- [x] 创建目录
- [x] 删除目录
- [x] 删除文件
- [x] 读取文件
- [x] 获取文件信息
- [x] 获取目录文件列表
- [x] 获取目录文件列表+分页
- [x] 获取服务使用量
- [x] 大文件分块上传



示例代码：  

```C#
      class Program
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        private static string bucketName = "bucketName";
        /// <summary>
        /// 操作员账号
        /// </summary>
        private static string username = "username";
        /// <summary>
        /// 操作员密码
        /// </summary>
        private static string password = "password";
        /// <summary>
        /// 上传文件保存目录
        /// </summary>
        private static string uploaddir = "uploaddir";
        /// <summary>
        /// 文件访问密码
        /// </summary>
        private static string filesecret = "filesecret";

        static UpYunConfig fig = new UpYunConfig() { Url = "", IsWebPlossless = true, IsWebPlossy = true, IsHttpResponseMessage = true };
        static UpYunClient upyunfig = new UpYunClient(bucketName, username, password, fig);
        static void Main(string[] args)
        {
            WriteFileAsync();
            Console.WriteLine("执行结束");
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        public static void WriteFileAsync()
        {
            string filepatn = @"C:\upload\20180826212900.png";
            string uploadpath = uploaddir.ToFileGuidName(filepatn);
            var res = upyunfig.WriteFileAsync(uploadpath, filepatn.ReadFile(), true).Result;
            Console.WriteLine(res.Data.ToJson());            
        }
    }
```



上传文件返回参数：

```json
{
    "x-upyun-content-length":[
        "46584"
    ],
    "x-upyun-content-type":[
        "image/png"
    ],
    "x-upyun-file-type":[
        "PNG"
    ],
    "x-upyun-width":[
        "218"
    ],
    "x-upyun-height":[
        "189"
    ],
    "x-upyun-frames":[
        "1"
    ],
    "x-upyun-url":"upyun.xxxxxx.vip/UpYunTest/38281088-2421-4708-87d4-739dc7307859.png",
    "x-upyun-webplossless":"upyun.xxxxx.vip/UpYunTest/38281088-2421-4708-87d4-739dc7307859.png!/format/webp/lossless/true",
    "x-upyun-webplossy":"upyun.xxxxxxx.vip/UpYunTest/38281088-2421-4708-87d4-739dc7307859.png!/format/webp"
}
```



更多用法参考UpYun.NETCore.SDK.Test项目





参考：

又拍云REST API：https://docs.upyun.com/api/rest_api/

杨中科：https://github.com/yangzhongke/UpYun.NETCore