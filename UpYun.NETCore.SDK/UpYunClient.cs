using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UpYun.NETCore.SDK
{
    public class UpYunClient
    {
        #region 内部属性

        private HttpClient httpClient;

        /// <summary>
        /// Defines the bucketname
        /// </summary>
        private string bucketname;

        /// <summary>
        /// Defines the username
        /// </summary>
        private string username;

        /// <summary>
        /// Defines the password
        /// </summary>
        private string password;

        private UpYunConfig upyunconfig;

        /// <summary>
        /// 默认：false
        /// </summary>
        private bool upAuth = false;

        /// <summary>
        /// 智能选路（推荐）：v0.api.upyun.com
        /// 电信线路：v1.api.upyun.com
        /// 联通（网通）线路：v2.api.upyun.com
        /// 移动（铁通）线路：v3.api.upyun.com
        /// </summary>
        private string api_domain = "v0.api.upyun.com";

        /// <summary>
        /// Defines the DL
        /// </summary>
        private string DL = "/";

        /// <summary>
        /// 用于分隔图片 URL 和处理信息，有 3 种可选，分别是：!（感叹号/默认值）、-（中划线）和 _（下划线），可登录又拍云控制台，在 「服务」 > 「功能配置」 > 「云处理」 中设置。
        /// </summary>
        private string separator_mark = "!";

        /// <summary>
        /// 用于分隔图片URL和处理信息，有 3 种可选，分别是：!（感叹号/默认值）、-（中划线）和 _（下划线）
        /// </summary>
        /// <param name="str"></param>
        public void SetSeparatorMark(string str)
        {
            this.separator_mark = str;
        }

        /// <summary>
        /// The version
        /// </summary>
        /// <returns>The <see cref="string"/></returns>
        public string version()
        {
            return "1.0.1";
        }

        /// <summary>
        /// 切换 API 接口的域名
        /// 默认 v0.api.upyun.com 自动识别, v1.api.upyun.com 电信, v2.api.upyun.com 联通, v3.api.upyun.com 移动
        /// </summary>
        /// <param name="domain">The domain<see cref="string"/></param>
        public void SetApiDomain(string domain)
        {
            this.api_domain = domain;
            httpClient.BaseAddress = new Uri("http://" + api_domain);
        }

        /// <summary>
        /// 是否启用 又拍云签名认证
        /// 默认 false 不启用(直接使用basic auth)，true 启用又拍云签名认证
        /// </summary>
        /// <param name="upAuth">The upAuth<see cref="bool"/></param>
        public void SetAuthType(bool upAuth)
        {
            this.upAuth = upAuth;
        }

        #endregion 内部属性

        public UpYunClient(string bucketname, string username, string password)
        {
            this.bucketname = bucketname;
            this.username = username;
            this.password = password;
            this.upyunconfig = new UpYunConfig();
            GetServiceHttpClient();
        }

        public UpYunClient(string bucketname, string username, string password, UpYunConfig upyunconfig)
        {
            this.bucketname = bucketname;
            this.username = username;
            this.password = password;
            this.upyunconfig = upyunconfig;
            GetServiceHttpClient();
        }

        private void GetServiceHttpClient()
        {
            var serviceProvider = new ServiceCollection().AddHttpClient().BuildServiceProvider();
            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
            httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri("http://" + api_domain);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="path">又拍云存储路径</param>   //又拍云存储文件路径（包含文件名）例如：上传到根目录：/aaaa.ipg，上传到子目录：/blog/bbb.png
        /// <param name="data">文件byte数据</param>
        /// <param name="automkdir">是否自动创建目录</param>
        /// <returns></returns>
        public async Task<Result> WriteFileAsync(string path, byte[] data, bool automkdir = true)
        {
            return await WriteFileAsync(path, data, automkdir, null, null);
        }

        /// <summary>
        /// 上传文件,支持访问密码，文件md5验证
        /// </summary>
        /// <param name="path">又拍云存储路径</param>
        /// <param name="data">文件byte数据</param>
        /// <param name="automkdir">是否自动创建目录</param>
        /// <param name="filesecret">文件访问密码</param>
        /// <param name="contentmd5">文件MD5</param>
        /// <returns></returns>
        public async Task<Result> WriteFileAsync(string path, byte[] data, bool automkdir, string filesecret = null, string contentmd5 = null)
        {
            Dictionary<string, object> headers = new Dictionary<string, object>();
            //             /jia-blog-image/UploadDev/3bf4f947-071d-4d0e-a17f-666903e89303.png
            string filepath = DL + this.bucketname + path;
            using (ByteArrayContent byteContent = new ByteArrayContent(data))
            {
                /*
                POST请求也可以上传文件
                 ByteArrayContentCommon("POST", byteContent, data, filepath, headers, automkdir, contentmd5, filesecret);
                                var resp = await httpClient.PostAsync(filepath, byteContent);*/
                ByteArrayContentCommon("PUT", byteContent, data, filepath, headers, automkdir, contentmd5, filesecret);
                var resp = await httpClient.PutAsync(filepath, byteContent);
                return Result(UploadResult(resp.Headers, ResRequestUrl(path, filesecret)), resp);
            }
        }

        /// <summary>
        /// 上传图片加水印，注意：文字,水印图片大小、长度必须小于图片大小，否则失败
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="data">文件byte数据</param>
        /// <param name="entity">水印类型实体</param>
        /// <param name="automkdir">是否自动创建目录</param>
        /// <returns></returns>
        public async Task<Result> WriteFileWatermarkAsync<T>(string path, byte[] data, T entity, bool automkdir = true)
        {
            return await WriteFileWatermarkAsync(path, data, entity, automkdir, null, null);
        }

        /// <summary>
        /// 上传图片加水印，注意：文字,水印图片大小、长度必须小于图片大小，否则失败
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="data">文件byte数据</param>
        /// <param name="entity">水印类型实体</param>
        /// <param name="automkdir">是否自动创建目录</param>
        /// <param name="filesecret">文件访问密码</param>
        /// <param name="contentmd5">文件MD5</param>
        /// <returns></returns>
        public async Task<Result> WriteFileWatermarkAsync<T>(string path, byte[] data, T entity, bool automkdir, string filesecret = null, string contentmd5 = null)
        {
            Dictionary<string, object> headers = new Dictionary<string, object>();

            //x-gmkerl-thumb:/watermark/text/d3d3Lm5ldGNvcmUudmlw/size/15/font/simsun/color/FF0000/border/FFFFFFFF/align/southeast/margin/5x5/opacity/100/animate/False
            headers.Add("x-gmkerl-thumb", entity.ToWatermarkUrl());
            string filepath = DL + this.bucketname + path;
            using (ByteArrayContent byteContent = new ByteArrayContent(data))
            {
                /*ByteArrayContentCommon("POST", byteContent, data, filepath, headers, automkdir, contentmd5, filesecret);
                var resp = await httpClient.PostAsync(filepath, byteContent);*/
                ByteArrayContentCommon("PUT", byteContent, data, filepath, headers, automkdir, contentmd5, filesecret);
                var resp = await httpClient.PutAsync(filepath, byteContent);
                return Result(UploadResult(resp.Headers, ResRequestUrl(path, filesecret)), resp);
            }
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="path">The path<see cref="string"/></param>
        public async Task<Result> CreateDirAsync(string path)
        {
            //  Dictionary<string, object> headers = new Dictionary<string, object>();
            string filepath = DL + this.bucketname + path;
            using (ByteArrayContent byteContent = new ByteArrayContent(new byte[] { }))
            {
                byteContent.Headers.Add("folder", "true");
                //var aa = HttpMethod.Post;
                ByteArrayContentCommon("POST", byteContent, filepath);
                var resp = await httpClient.PostAsync(filepath, byteContent);
                return Result("", resp);
            }
        }

        /// <summary>
        ///  删除目录,只允许删除空的目录，非空目录需要先删除里面的文件，否则删除请求会被拒绝。
        /// </summary>
        /// <param name="path">The path<see cref="string"/></param>
        public async Task<Result> DeleteDirAsync(string path)
        {
            string filepath = DL + this.bucketname + path;
            using (ByteArrayContent byteContent = new ByteArrayContent(new byte[] { }))
            {
                ByteArrayContentCommon("DELETE", byteContent, filepath);
                var resp = await httpClient.DeleteAsync(filepath);
                return Result("", resp);
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path">The path<see cref="string"/></param>
        /// <param name="async">true 表示进行异步删除，不设置表示同步删除（默认）</param>
        public async Task<Result> DeleteFileAsync(string path, bool async = false)
        {
            string filepath = DL + this.bucketname + path;
            using (ByteArrayContent byteContent = new ByteArrayContent(new byte[] { }))
            {
                if (async)
                {
                    byteContent.Headers.Add("x-upyun-async", "true");
                }
                ByteArrayContentCommon("DELETE", byteContent, filepath);
                var resp = await httpClient.DeleteAsync(filepath);
                return Result("", resp);
            }
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="path">The path<see cref="string"/></param>
        public async Task<Result> ReadFileAsync(string path)
        {
            string filepath = DL + this.bucketname + path;
            using (ByteArrayContent byteContent = new ByteArrayContent(new byte[] { }))
            {
                ByteArrayContentCommon("GET", byteContent, filepath);
                var resp = await httpClient.GetAsync(filepath);
                return Result(resp.Content.ReadAsByteArrayAsync().Result, resp);
            }
        }

        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<Result> GetFileInfoAsync(string path)
        {
            string filepath = DL + this.bucketname + path;
            using (ByteArrayContent byteContent = new ByteArrayContent(new byte[] { }))
            {
                ByteArrayContentCommon("HEAD", byteContent, filepath);
                var resp = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, filepath));

                var res = new Dictionary<string, object>();
                foreach (var header in resp.Headers)
                {
                    if (header.Key.Length > 7 && header.Key.Substring(0, 7) == "x-upyun")
                    {
                        res.Add(header.Key, header.Value);
                    }
                }
                foreach (var header in resp.Content.Headers)
                {
                    if (header.Key.Length > 7 && header.Key.Substring(0, 7).ToLower() == "content")
                    {
                        res.Add(header.Key, header.Value);
                    }
                }
                return Result(res, resp);
            }
        }

        /// <summary>
        /// 获取目录文件列表
        /// </summary>
        /// <param name="path"></param>
        /// <param name="iter">分页开始位置，通过x-upyun-list-iter 响应头返回，所以第一次请求不需要填写，当它是 g2gCZAAEbmV4dGQAA2VvZg 时，表示最后一个分页</param>
        /// <param name="limit">获取的文件数量，默认 100，最大 10000</param>
        /// <param name="order">asc 或 desc，按文件名升序或降序排列。默认 asc</param>
        /// <returns></returns>
        public async Task<Result> GetDirFileListAsync(string path, string iter = "", int limit = 100, string order = "asc")
        {
            httpClient.DefaultRequestHeaders.Clear();
            string filepath = DL + this.bucketname + path;
            using (ByteArrayContent byteContent = new ByteArrayContent(new byte[] { }))
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (iter.IsNotNull())
                {
                    httpClient.DefaultRequestHeaders.Add("x-list-iter", iter);
                }
                httpClient.DefaultRequestHeaders.Add("x-list-limit", limit.ToString());
                httpClient.DefaultRequestHeaders.Add("x-list-order", order);
                ByteArrayContentCommon("GET", byteContent, filepath);
                var resp = await httpClient.GetAsync(filepath);
                return Result(resp.Content.ReadAsStringAsync().Result.FromJson<DirFileEntity>(), resp);
            }
        }

        /// <summary>
        /// 获取服务使用量，单位：Byte
        /// </summary>
        /// <returns></returns>
        public async Task<Result> GetFolderUsageAsync()
        {
            string filepath = DL + bucketname + DL + "?usage";
            using (ByteArrayContent byteContent = new ByteArrayContent(new byte[] { }))
            {
                ByteArrayContentCommon("GET", byteContent, filepath);
                var resp = await httpClient.GetAsync(filepath);
                string count = await resp.Content.ReadAsStringAsync();
                //var resp = await httpClient.GetStringAsync(filepath);
                return Result(count, resp);
            }
        }

        #region 文件分块上传内部方法

        /// <summary>
        /// 初始化断点续传
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filebytelength"></param>
        /// <returns></returns>
        public async Task<Result> WriteFileInitiate(string path, string filebytelength)
        {
            string filepath = DL + this.bucketname + path;
            using (ByteArrayContent byteContent = new ByteArrayContent(new byte[] { }))
            {
                // httpClient.DefaultRequestHeaders.Add("x-upyun-multi-stage", "initiate");
                byteContent.Headers.Add("x-upyun-multi-stage", "initiate");
                byteContent.Headers.Add("x-upyun-multi-length", filebytelength);
                byteContent.Headers.Add("x-upyun-multi-type", path.GetMimeMapping());
                ByteArrayContentCommon("PUT", byteContent, filepath);
                var resp = await httpClient.PutAsync(filepath, byteContent);
                return Result(UploadResult(resp.Headers), resp, true);
            }
        }

        /// <summary>
        /// 续传数据,分块大小固定为 1M。最后一个分块除外。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <param name="uuid"></param>
        /// <param name="partid"></param>
        /// <returns></returns>
        public async Task<Result> WriteFileUpload(string path, byte[] data, string uuid, string partid)
        {
            string filepath = DL + this.bucketname + path;
            using (ByteArrayContent byteContent = new ByteArrayContent(data))
            {
                byteContent.Headers.Add("x-upyun-multi-stage", "upload");
                byteContent.Headers.Add("x-upyun-multi-uuid", uuid);
                byteContent.Headers.Add("x-upyun-part-id", partid);
                ByteArrayContentCommon("PUT", byteContent, filepath);
                var resp = await httpClient.PutAsync(filepath, byteContent);
                return Result(UploadResult(resp.Headers), resp, true);
            }
        }

        /// <summary>
        /// 结束断点续传
        /// </summary>
        /// <param name="path"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public async Task<Result> WriteFileComplete(string path, string uuid)
        {
            string filepath = DL + this.bucketname + path;
            using (ByteArrayContent byteContent = new ByteArrayContent(new byte[] { }))
            {
                byteContent.Headers.Add("x-upyun-multi-stage", "complete");
                byteContent.Headers.Add("x-upyun-multi-uuid", uuid);
                ByteArrayContentCommon("PUT", byteContent, filepath);
                var resp = await httpClient.PutAsync(filepath, byteContent);
                return Result(UploadResult(resp.Headers), resp, true);
            }
        }

        #endregion 文件分块上传内部方法

        /// <summary>
        /// 文件分块上传
        /// </summary>
        /// <param name="path">上传路径</param>
        /// <param name="fileStream">文件流</param>
        /// <returns></returns>
        public async Task<Result> WriteFileBlockAsync(string path, FileStream fileStream)
        {
            int blockLen = 1048576;   //分块大小固定为1M(否则报错：403)。最后一个分块除外
            long fileLength = fileStream.Length;
            List<long> pkgList = new List<long>();
            for (long iIdx = 0; iIdx < fileLength / Convert.ToInt64(blockLen); iIdx++)
            {
                pkgList.Add(Convert.ToInt64(blockLen));   //计算每块大小
            }
            long s = fileLength % blockLen;
            if (s != 0)
            {
                pkgList.Add(s);         //最后一个块
            }
            //  var getInit = WriteFileInitiate(path, fileStream.Length.ToString()).Result.Data as Dictionary<string, object>;
            var getreResult = await WriteFileInitiate(path, fileStream.Length.ToString());
            var getInit = getreResult.Data as Dictionary<string, object>;
            if (getInit == null) throw new NullReferenceException("未获取到又拍云返回值");
            if (getInit["x-upyun-multi-uuid"] == null) throw new NullReferenceException("未获取到x-upyun-multi-uuid");
            if (getInit["x-upyun-next-part-id"] == null) throw new NullReferenceException("未获取到x-upyun-next-part-id");

            string uuid = (getInit["x-upyun-multi-uuid"] as string[])?[0];
            string partid = (getInit["x-upyun-next-part-id"] as string[])?[0];

            for (long iPkgIdx = 0; iPkgIdx < pkgList.Count; iPkgIdx++)
            {
                long bufferSize = pkgList[(int)iPkgIdx];  //1024*1024
                byte[] buffer = new byte[bufferSize];
                int bytesRead = fileStream.Read(buffer, 0, (int)bufferSize);   //获取一个块上传
                /*var upres = WriteFileUpload(path, buffer, uuid, partid).Result.Data as Dictionary<string, object>;
                partid = (upres["x-upyun-next-part-id"] as string[])[0];*/
                var upres2 = await WriteFileUpload(path, buffer, uuid, partid);
                var res2 = upres2.Data as Dictionary<string, object>;
                partid = (res2["x-upyun-next-part-id"] as string[])[0];
            }
            var res = await WriteFileComplete(path, uuid);
            return res;
        }

        #region 内部方法

        private void ByteArrayContentCommon(string method, ByteArrayContent byteContent, byte[] data, string url, Dictionary<string, object> headers, bool automkdir, string contentmd5 = null, string filesecret = null)
        {
            if (automkdir)
            {
                byteContent.Headers.Add("mkdir", "true");
            }
            if (data != null)
            {
                if (!string.IsNullOrWhiteSpace(contentmd5))
                {
                    byteContent.Headers.Add("Content-MD5", contentmd5.Trim());
                }
                if (!string.IsNullOrWhiteSpace(filesecret))
                {
                    byteContent.Headers.Add("Content-Secret", filesecret.Trim());
                }
            }
            if (this.upAuth)
            {
                UpyunAuth(byteContent, method, url);
            }
            else
            {
                var value = Convert.ToBase64String(new ASCIIEncoding().GetBytes(this.username + ":" + this.password));
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", value);
            }
            foreach (var kv in headers)
            {
                byteContent.Headers.Add(kv.Key, kv.Value.ToString());
            }
        }

        private void ByteArrayContentCommon(string method, ByteArrayContent byteContent, string url)
        {
            if (this.upAuth)
            {
                UpyunAuth(byteContent, method, url);
            }
            else
            {
                var value = Convert.ToBase64String(new ASCIIEncoding().GetBytes(this.username + ":" + this.password));
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", value);
            }
        }

        /// <summary>
        /// 签名认证
        /// </summary>
        /// <param name="requestContent"></param>
        /// <param name="method"></param>
        /// <param name="uri"></param>
        private void UpyunAuth(ByteArrayContent requestContent, string method, string uri)
        {
            DateTime dt = DateTime.UtcNow;
            string date = dt.ToString("ddd, dd MMM yyyy HH':'mm':'ss 'GMT'", new CultureInfo("en-US"));
            httpClient.DefaultRequestHeaders.Date = dt;
            string body = requestContent.ReadAsStringAsync().Result;
            string auth;
            if (!string.IsNullOrEmpty(body))
                auth = md5(method + '&' + uri + '&' + date + '&' + requestContent.ReadAsByteArrayAsync().Result.Length + '&' + md5(this.password));
            else
                auth = md5(method + '&' + uri + '&' + date + '&' + 0 + '&' + md5(this.password));

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("UPYUN", this.username + ':' + auth);
        }

        /// <summary>
        /// The md5
        /// </summary>
        /// <param name="str">The str<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        private string md5(string str)
        {
            using (MD5 m = MD5.Create())
            {
                byte[] s = m.ComputeHash(Encoding.UTF8.GetBytes(str));
                string resule = BitConverter.ToString(s);
                resule = resule.Replace("-", "");
                return resule.ToLower();
            }
        }

        private Dictionary<string, object> UploadResult(HttpResponseHeaders headers, Dictionary<string, object> resurl = null)
        {
            var res = new Dictionary<string, object>();
            foreach (var header in headers)
            {
                if (header.Key.Length > 7 && header.Key.Substring(0, 7) == "x-upyun")
                {
                    res.Add(header.Key, header.Value);
                }
            }
            if (resurl == null) return res;
            foreach (var item in resurl)
            {
                res.Add(item.Key, item.Value);
            }
            return res;
        }

        private Dictionary<string, object> ResRequestUrl(string path, string filesecret = null)
        {
            if (string.IsNullOrWhiteSpace(upyunconfig.Url)) return null;
            var res = new Dictionary<string, object>();
            string fileurl = upyunconfig.Url + path;
            string requesturl = "";
            if (string.IsNullOrWhiteSpace(filesecret))
            {
                requesturl = fileurl + separator_mark;
                res.Add("x-upyun-url", fileurl);
            }
            else
            {
                string secreturl = fileurl + separator_mark + filesecret;
                requesturl = secreturl;
                res.Add("x-upyun-url", secreturl);
            }
            if (upyunconfig.IsWebPlossless)
            {
                res.Add("x-upyun-webplossless", requesturl + CommonBase.WebPlossless);
            }
            if (upyunconfig.IsWebPlossy)
            {
                res.Add("x-upyun-webplossy", requesturl + CommonBase.WebPlossy);
            }
            return res;
        }



        private Result Result(object data, HttpResponseMessage responsemessage)
        {
            if (upyunconfig.IsHttpResponseMessage)
            {
                return new Result
                {
                    State = (responsemessage.StatusCode == System.Net.HttpStatusCode.OK),
                    Data = data,
                    ResponseMessage = responsemessage
                };
            }
            return new Result
            {
                State = (responsemessage.StatusCode == System.Net.HttpStatusCode.OK),
                Data = data,
            };
        }

        private Result Result(byte[] data, HttpResponseMessage responsemessage)
        {
            if (upyunconfig.IsHttpResponseMessage)
            {
                return new Result
                {
                    State = (responsemessage.StatusCode == System.Net.HttpStatusCode.OK),
                    Bytes = data,
                    ResponseMessage = responsemessage
                };
            }
            return new Result
            {
                State = (responsemessage.StatusCode == System.Net.HttpStatusCode.OK),
                Bytes = data,
            };
        }

        private Result Result<T>(T data, HttpResponseMessage responsemessage, bool statuscodemultiple = false)
        {

            bool state = responsemessage.StatusCode == System.Net.HttpStatusCode.OK;
            if (statuscodemultiple)
            {
                state = responsemessage.StatusCode == System.Net.HttpStatusCode.OK ||
                            responsemessage.StatusCode == System.Net.HttpStatusCode.NoContent ||
                            responsemessage.StatusCode == System.Net.HttpStatusCode.Created;
            }
            if (upyunconfig.IsHttpResponseMessage)
            {
                return new Result
                {
                    State = state,
                    Data = data,
                    ResponseMessage = responsemessage
                };
            }
            return new Result
            {
                State = state,
                Data = data,
            };
        }





        #endregion 内部方法
    }
}