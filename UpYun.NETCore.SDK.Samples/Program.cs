using System;

namespace UpYun.NETCore.SDK.Samples
{
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
}
