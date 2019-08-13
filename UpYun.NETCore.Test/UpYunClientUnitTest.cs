using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace UpYun.NETCore.SDK.Test
{
    public class UpYunClientUnitTest
    {
        //ע�⣺appsettings.json�ǵñ���utf-8��ʽ�������ȡ����
        static IConfigurationRoot config = new ConfigurationBuilder()
              .AddInMemoryCollection() //�������ļ������ݼ��ص��ڴ���
              .SetBasePath(Directory.GetCurrentDirectory()) //ָ�������ļ����ڵ�Ŀ¼
                                                            // .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) //ָ�����ص������ļ�
            .AddJsonFile("appsettings.dev.json", optional: true, reloadOnChange: true)
              .Build(); //����ɶ���  

        /// <summary>
        /// ��������
        /// </summary>
        private static string bucketName = config["bucketName"];
        /// <summary>
        /// ����Ա�˺�
        /// </summary>
        private static string username = config["username"];
        /// <summary>
        /// ����Ա����
        /// </summary>
        private static string password = config["password"];
        /// <summary>
        /// �ϴ��ļ�����Ŀ¼
        /// </summary>
        private static string uploaddir = config["uploaddir"];
        /// <summary>
        /// �ļ���������
        /// </summary>
        private static string filesecret = config["filesecret"];


        private static readonly string Basedir = GetDataDir(); //AppDomain.CurrentDomain.BaseDirectory.Replace(@"\bin\Debug\netcoreapp2.2\", "");
        private static readonly string Filepatn = Basedir + @"\upload\20180826212900.png";



        // private readonly UpYunClient upyun = new UpYunClient(bucketName, username, password);
        static UpYunConfig fig = new UpYunConfig() { Url = config["url"], IsWebPlossless = true, IsWebPlossy = true, IsHttpResponseMessage = true };
        static UpYunClient upyunfig = new UpYunClient(bucketName, username, password, fig);
        private byte[] filebyte = Filepatn.ReadFile();

        /// <summary>
        /// �Ƿ�ʹ��ǩ����֤��Ĭ��Basic��֤
        /// </summary>
        private void IsAuthType()
        {
            // upyunfig.SetAuthType(true);
        }
        /// <summary>
        /// �������Ƿ�д����־
        /// </summary>
        private void ResLog(Result result, string name)
        {
            // result.Data.LogFileJson(true, name);
            result.LogFileJson(true, name);
        }

        #region MyRegion


        /// <summary>
        /// �ϴ��ļ�
        /// </summary>
        [Fact]    //[Fact(DisplayName = "�ϴ��ļ�")]
        public void WriteFileAsync()
        {
            IsAuthType();
            string uploadpath = uploaddir.ToFileGuidName(Filepatn);
            var res = upyunfig.WriteFileAsync(uploadpath, filebyte, true).Result;
            ResLog(res, "WriteFileAsync");
            Assert.True(res.State);
        }
        /// <summary>
        /// �ϴ��ļ�+��������+�ļ�MD5��֤
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

        WatermarkTextEntity textentity = new WatermarkTextEntity("����ˮӡ����");


        /// <summary>
        /// �ϴ�ͼƬ������ˮӡ
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
        /// �ϴ�ͼƬ��ͼƬˮӡ
        /// </summary>
        [Fact]
        public void WriteFileImageWatermarkAsync()
        {
            //�ϴ�ˮӡͼƬ
            string imagefile = Basedir + @"\upload\WatermarkIMG.png";
            string imagepath = uploaddir.ToFileGuidName(imagefile);
            var resimg = upyunfig.WriteFileAsync(imagepath, imagefile.ReadFile(), true).Result;
            //ע�⣺ͼƬˮӡ��Դ��Ҫ��������Ŀ¼�ļ���
            WatermarkImageEntity imageentity = new WatermarkImageEntity(imagepath);

            //�ϴ��ļ����ͼƬˮӡ
            IsAuthType();
            string uploadpath = uploaddir.ToFileGuidName(Filepatn);
            var res = upyunfig.WriteFileWatermarkAsync(uploadpath, filebyte, imageentity).Result;
            ResLog(res, "WriteFileImageWatermarkAsync");
            Assert.True(res.State);
        }



        #region ��ͼƬ��ˮӡ���ԣ�����403
        [Fact(Skip = "�ļ���ˮӡ")]
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

        [Fact(Skip = "�ļ���ͼƬˮӡ")]
        public void FileWriteFileImageWatermarkAsync()
        {
            //�ϴ�ˮӡͼƬ
            string imagefile = Basedir + @"\upload\WatermarkIMG.png";
            string imagepath = "/test/".ToFileGuidName(imagefile);
            var resimg = upyunfig.WriteFileAsync(imagepath, imagefile.ReadFile(), true).Result;
            //ע�⣺ͼƬˮӡ��Դ��Ҫ��������Ŀ¼�ļ���
            WatermarkImageEntity imageentity = new WatermarkImageEntity(imagepath);

            //�ϴ��ļ����ͼƬˮӡ
            IsAuthType();
            string filepatn = Basedir + @"\upload\GetVisualStudioInstallPath.ps1";
            string uploadpath = "/test/".ToFileGuidName(filepatn);
            var res = upyunfig.WriteFileWatermarkAsync(uploadpath, filepatn.ReadFile(), imageentity).Result;
            ResLog(res, "FileWriteFileImageWatermarkAsync");
            Assert.True(res.State);
        }



        #endregion


        /// <summary>
        /// �ϴ��ļ�+��ˮӡ+��������+�ļ�MD5��֤
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
                /// ����Ŀ¼
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
        /// ����Ŀ¼��ɾ��Ŀ¼
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
        /// ɾ���ļ�
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
        /// ��ȡ�ļ�
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
        /// ��ȡ�ļ���Ϣ
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
        /// ��ȡĿ¼�ļ��б�
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
        ///  ��ȡĿ¼�ļ��б�+��ҳ
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
        /// ��ȡ����ʹ��������λ��Byte
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
        /// ���ļ��ֿ��ϴ�
        /// </summary>
        [Fact]
        public void WriteFileBlockAsync()
        {
            IsAuthType();
            string file = Basedir + @"\upload\Docker-�����ŵ�ʵ��.pdf";
            string uploadpath = uploaddir.ToFileGuidName(file);
            using (FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                var res = upyunfig.WriteFileBlockAsync(uploadpath, fileStream).Result;
                ResLog(res, "WriteFileBlockAsync");
                //���� 204 ״̬�룬��ʾ�ļ�����ͬ���ļ���������� 201 ״̬�룬��ʾ�ļ������ļ���
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
