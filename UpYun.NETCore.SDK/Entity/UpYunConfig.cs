namespace UpYun.NETCore.SDK
{
    public class UpYunConfig
    {


        /// <summary>
        /// 返回有损WebP访问连接
        /// </summary>
        public bool IsWebPlossy { get; set; } = false;

        /// <summary>
        /// 返回无损WebP访问连接
        /// </summary>
        public bool IsWebPlossless { get; set; } = false;

        /// <summary>
        /// 配置访问域名
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 是否返回HttpResponseMessage
        /// </summary>
        public bool IsHttpResponseMessage { get; set; } = false;



    }
}
