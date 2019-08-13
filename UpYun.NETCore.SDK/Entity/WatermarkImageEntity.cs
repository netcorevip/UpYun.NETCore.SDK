
namespace UpYun.NETCore.SDK
{
    /// <summary>
    /// 图片水印参数实体
    /// </summary>
    public class WatermarkImageEntity
    {
        //https://docs.upyun.com/cloud/image/#_14

        public WatermarkImageEntity()
        {

        }

        public WatermarkImageEntity(string watermarkurl)
        {
            url = watermarkurl.ToWatermarkBase64();
        }

        /// <summary>
        /// 编码字符串，如 L3BhdGgvdG8vd2F0ZXJtYXJrLnBuZw==	水印图片的 URI，示例为 /path/to/watermark.png 的 Base64 编码字符串。特别地，水印图片必须和待处理图片在同一服务名下
        /// 图片水印的 url 需要 base64 编码，并把编码后的字符串中的 /（斜杠）替换成 |（竖线）。
        /// </summary>
        public string url { get; set; }

        /// <summary>
        ///  /align/<align>	位置，如 north	水印图片放置方位，默认 northwest，详见方位说明
        /// https://help.upyun.com/knowledge-base/image/#align_gravity
        /// </summary>
        public EnumHelper.Align align { get; set; } = EnumHelper.Align.southeast;

        /// <summary>
        /// /margin/<x>x<y> 横偏移x纵偏移，如 15×10	水印图片横纵相对偏移，默认 20x20
        /// </summary>
        public string margin { get; set; } = "5x5";

        /// <summary>
        /// /opacity/<opacity>	透明度，如 90	水印图片透明度，默认 100，取值范围 [0-100]，值越大越不透明，0 完全透明，100 完全不透明
        /// </summary>
        public int opacity { get; set; } = 100;

        /// <summary>
        /// /percent/<percent>	百分比值，如 50	水印图片自适应原图短边的比例，取值范围 [0-100]，默认 0，0 表示不设置该参数
        /// </summary>
        public int percent { get; set; } = 0;

        /// <summary>
        ///  /animate/<boolean>	true	水印图片是否重复铺满原图，默认 false
        /// </summary>
        public bool repeat { get; set; } = false;

        /// <summary>
        ///  /animate/<boolean>	true	允许对动态图片加水印，默认 false
        /// </summary>
        public bool animate { get; set; } = false;



    }
}
