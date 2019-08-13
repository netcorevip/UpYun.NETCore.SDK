using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace UpYun.NETCore.SDK.Test
{
    public class UpYunClientUnitTest
    {
        //注意：appsettings.json记得保存utf-8格式，否则读取乱码
        static IConfigurationRoot config = new ConfigurationBuilder()
              .AddInMemoryCollection() //将配置文件的数据加载到内存中
              .SetBasePath(Directory.GetCurrentDirectory()) //指定配置文件所在的目录
                                                            // .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) //指定加载的配置文件
            .AddJsonFile("appsettings.dev.json", optional: true, reloadOnChange: true)
              .Build(); //编译成对象  

        /// <summary>
        /// 服务名称
        /// </summary>
        private static string bucketName = config["bucketName"];
        /// <summary>
        /// 操作员账号
        /// </summary>
        private static string username = config["username"];
        /// <summary>
        /// 操作员密码
        /// </summary>
        private static string password = config["password"];
        /// <summary>
        /// 上传文件保存目录
        /// </summary>
        private static string uploaddir = config["uploaddir"];
        /// <summary>
        /// 文件访问密码
        /// </summary>
        private static string filesecret = config["filesecret"];


        private static readonly string Basedir = GetDataDir(); //AppDomain.CurrentDomain.BaseDirectory.Replace(@"\bin\Debug\netcoreapp2.2\", "");
        private static readonly string Filepatn = Basedir + @"\upload\20180826212900.png";



        // private readonly UpYunClient upyun = new UpYunClient(bucketName, username, password);
        static UpYunConfig fig = new UpYunConfig() { Url = config["url"], IsWebPlossless = true, IsWebPlossy = true, IsHttpResponseMessage = true };
        static UpYunClient upyunfig = new UpYunClient(bucketName, username, password, fig);
        private byte[] filebyte = Filepatn.ReadFile();

        /// <summary>
        /// 是否使用签名认证，默认Basic认证
        /// </summary>
        private void IsAuthType()
        {
            // upyunfig.SetAuthType(true);
        }
        /// <summary>
        /// 请求结果是否写入日志
        /// </summary>
        private void ResLog(Result result, string name)
        {
            // result.Data.LogFileJson(true, name);
            result.LogFileJson(true, name);
        }

        #region MyRegion


        /// <summary>
        /// 上传文件
        /// </summary>
        [Fact]    //[Fact(DisplayName = "上传文件")]
        public void WriteFileAsync()
        {
            IsAuthType();
            string uploadpath = uploaddir.ToFileGuidName(Filepatn);
            var res = upyunfig.WriteFileAsync(uploadpath, filebyte, true).Result;
            ResLog(res, "WriteFileAsync");
            Assert.True(res.State);
        }
        /// <summary>
        /// 上传文件+访问密码+文件MD5验证
        /// </summary>
        [Fact]
        public void WriteFileSecretMD5Async()
        {
            IsAuthType();
            string uploadpath = uploaddir.ToFileGuidName(Filepatn);
            var res = upyunfig.WriteFileAsync(uploadpath, filebyte, true, filesecret, Filepatn.MD5File()).Result;
            ResLog(res, "WriteFileSecretMD5Async");
            Assert.True(res.State);
        }

        WatermarkTextEntity textentity = new WatermarkTextEntity("文字水印内容");


        /// <summary>
        /// 上传图片加文字水印
        /// </summary>
        [Fact]
        public void WriteFileTextWatermarkAsync()
        {
            IsAuthType();
            string uploadpath = uploaddir.ToFileGuidName(Filepatn);
            var res = upyunfig.WriteFileWatermarkAsync(uploadpath, filebyte, textentity).Result;
            ResLog(res, "WriteFileTextWatermarkAsync");
            Assert.True(res.State);
        }
        /// <summary>
        /// 上传图片加图片水印
        /// </summary>
        [Fact]
        public void WriteFileImageWatermarkAsync()
        {
            //上传水印图片
            string imagefile = Basedir + @"\upload\WatermarkIMG.png";
            string imagepath = uploaddir.ToFileGuidName(imagefile);
            var resimg = upyunfig.WriteFileAsync(imagepath, imagefile.ReadFile(), true).Result;
            //注意：图片水印来源需要是又拍云目录文件，
            WatermarkImageEntity imageentity = new WatermarkImageEntity(imagepath);

            //上传文件添加图片水印
            IsAuthType();
            string uploadpath = uploaddir.ToFileGuidName(Filepatn);
            var res = upyunfig.WriteFileWatermarkAsync(uploadpath, filebyte, imageentity).Result;
            ResLog(res, "WriteFileImageWatermarkAsync");
            Assert.True(res.State);
        }



        #region 非图片加水印测试，报错403
        [Fact(Skip = "文件加水印")]
        public void FileWriteFileTextWatermarkAsync()
        {
            //
            IsAuthType();
            //  string filepatn = Basedir + @"\upload\GetVisualStudioInstallPath.ps1";
            string filepatn = Basedir + @"\upload\upload.txt";
            string uploadpath = "/test/".ToFileGuidName(filepatn);
            // var res = upyunfig.WriteFileWatermarkAsync(uploadpath, filepatn.ReadFile(), textentity).Result;
            var res = upyunfig.WriteFileAsync(uploadpath, filepatn.ReadFile()).Result;
            ResLog(res, "FileWriteFileTextWatermarkAsync");
            Assert.True(res.State);
        }

        [Fact(Skip = "文件加图片水印")]
        public void FileWriteFileImageWatermarkAsync()
        {
            //上传水印图片
            string imagefile = Basedir + @"\upload\WatermarkIMG.png";
            string imagepath = "/test/".ToFileGuidName(imagefile);
            var resimg = upyunfig.WriteFileAsync(imagepath, imagefile.ReadFile(), true).Result;
            //注意：图片水印来源需要是又拍云目录文件，
            WatermarkImageEntity imageentity = new WatermarkImageEntity(imagepath);

            //上传文件添加图片水印
            IsAuthType();
            string filepatn = Basedir + @"\upload\GetVisualStudioInstallPath.ps1";
            string uploadpath = "/test/".ToFileGuidName(filepatn);
            var res = upyunfig.WriteFileWatermarkAsync(uploadpath, filepatn.ReadFile(), imageentity).Result;
            ResLog(res, "FileWriteFileImageWatermarkAsync");
            Assert.True(res.State);
        }



        #endregion


        /// <summary>
        /// 上传文件+加水印+访问密码+文件MD5验证
        /// </summary>
        [Fact]
        public void WriteFileSecretMD5WatermarkAsync()
        {
            IsAuthType();
            string uploadpath = uploaddir.ToFileGuidName(Filepatn);
            var res = upyunfig.WriteFileWatermarkAsync(uploadpath, filebyte, textentity, true, filesecret, Filepatn.MD5File()).Result;
            ResLog(res, "WriteFileSecretMD5WatermarkAsync");
            Assert.True(res.State);
        }
        #endregion
        /*  
                /// <summary>
                /// 创建目录
                /// </summary>
                [Fact, Order(1)]
              public void CreateDirAsync()
                {
                    IsAuthType();
                    //  string dir = "/CreateDirTest" + DateTime.Now.Ticks;
                    var res = upyunfig.CreateDirAsync("/CreateDirTest").Result;
                    ResLog(res, "CreateDirAsync");
                    Assert.True(res.State);
                }*/

        /// <summary>
        /// 创建目录，删除目录
        /// </summary>
        [Fact]
        public void CreateAndDeleteDirAsync()
        {
            IsAuthType();

            var resce = upyunfig.CreateDirAsync("/CreateDirTest").Result;
            ResLog(resce, "CreateDirAsync");
            Thread.Sleep(3000);
            var res = upyunfig.DeleteDirAsync("/CreateDirTest").Result;
            ResLog(res, "DeleteDirAsync");
            Assert.True(res.State && resce.State);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        [Fact]
        public void DeleteFileAsync()
        {
            IsAuthType();
            string path = GetPath();
            Thread.Sleep(3000);
            var res = upyunfig.DeleteFileAsync(path).Result;
            ResLog(res, "DeleteFileAsync");
            Assert.True(res.State);
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        [Fact]
        public void ReadFileAsync()
        {
            IsAuthType();
            string path = GetPath();
            Thread.Sleep(3000);
            var res = upyunfig.ReadFileAsync(path).Result;
            ResLog(res, "ReadFileAsync");
            Assert.True(res.State);
        }

        /// <summary>
        /// 获取文件信息
        /// </summary>
        [Fact]
        public void GetFileInfoAsync()
        {
            IsAuthType();
            var res = upyunfig.GetFileInfoAsync(GetPath()).Result;
            ResLog(res, "GetFileInfoAsync");
            Assert.True(res.State);
        }

        /// <summary>
        /// 获取目录文件列表
        /// </summary>
        [Fact]
        public void GetDirFileListAsync()
        {
            IsAuthType();
            var res = upyunfig.GetDirFileListAsync(uploaddir, "", 110).Result;
            var list = res.Data as DirFileEntity;

            if (list.files.Count < 110)
            {
                for (int i = 0; i < 150; i++)
                {
                    GetPath();
                }
                res = upyunfig.GetDirFileListAsync(uploaddir, "", 110).Result;
                list = res.Data as DirFileEntity;
            }
            ResLog(res, "GetDirFileListAsync");
            Assert.True(res.State && list.files.Count == 110);
        }

        /// <summary>
        ///  获取目录文件列表+分页
        /// </summary>
        [Fact]
        public void GetDirFileListAsyncPage()
        {
            IsAuthType();

            var res = upyunfig.GetDirFileListAsync(uploaddir, "", 110).Result;
            var list = res.Data as DirFileEntity;

            while (list.iter != "g2gCZAAEbmV4dGQAA2VvZg")
            {
                res = upyunfig.GetDirFileListAsync(uploaddir, list.iter, 110).Result;
                list = res.Data as DirFileEntity;
            }
            ResLog(res, "GetDirFileListAsyncPage");
            Assert.True(res.State && list.iter == "g2gCZAAEbmV4dGQAA2VvZg");
        }

        /// <summary>
        /// 获取服务使用量，单位：Byte
        /// </summary>
        [Fact]
        // [Theory]
        // [InlineData(1, 2)]
        public void GetFolderUsageAsync()
        {
            IsAuthType();
            var res = upyunfig.GetFolderUsageAsync().Result;
            ResLog(res, "GetFolderUsageAsync");
            Assert.True(res.State);
        }

        /// <summary>
        /// 大文件分块上传
        /// </summary>
        [Fact]
        public void WriteFileBlockAsync()
        {
            IsAuthType();
            string file = Basedir + @"\upload\Docker-从入门到实践.pdf";
            string uploadpath = uploaddir.ToFileGuidName(file);
            using (FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                var res = upyunfig.WriteFileBlockAsync(uploadpath, fileStream).Result;
                ResLog(res, "WriteFileBlockAsync");
                //返回 204 状态码，表示文件覆盖同名文件；如果返回 201 状态码，表示文件是新文件。
                Assert.True(res.Data != null);
            }
        }

        private string GetPath()
        {

            string uploadpath = uploaddir.ToFileGuidName(Filepatn);
            var resup = upyunfig.WriteFileAsync(uploadpath, filebyte, true).Result;
            return uploadpath;
        }

        public static string GetDataDir()
        {
            var parent = Directory.GetParent(Directory.GetCurrentDirectory()).Parent;
            string startDirectory = null;
            if (parent != null)
            {
                var directoryInfo = parent.Parent;
                if (directoryInfo != null)
                {
                    startDirectory = directoryInfo.FullName;
                }
            }
            else
            {
                startDirectory = parent.FullName;
            }
            //   return Path.Combine(startDirectory, "Data\\");
            return startDirectory;
        }


    }
}
