

namespace UpYun.NETCore.SDK
{
    /// <summary>
    /// 文字水印实体
    /// </summary>
    public class WatermarkTextEntity
    {
        //  public string watermark { get; set; }
        /*
         * https://docs.upyun.com/cloud/image/#_14
         */
        public WatermarkTextEntity()
        {

        }
        /// <summary>
        /// 文字水印
        /// </summary>
        /// <param name="watermarktext"></param>
        public WatermarkTextEntity(string watermarktext)
        {
            text = watermarktext.ToWatermarkBase64();
        }

        /// <summary>
        /// /watermark/text/<text>	文字内容，如 5L2g5aW977yB 文字内容，示例中为 你好！的 Base64 编码字符串。
        /// 文字水印的 text 需要 base64 编码，并把编码后的字符串中的 /（斜杠）替换成 |（竖线）。
        /// 文字水印的中含中文内容（text）时，字体（font）请使用中文字体，否则会乱码。
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// /size/<size>	大小，如 16	文字大小，单位 px，默认 32
        /// </summary>
        public int size { get; set; } = 15;

        /// <summary>
        /// /font/<font>	字体，如 simsun（宋体）	文字字体，默认 simsun。字体使用时，需要用参数名。参数名见字体列表
        /// </summary>
        public EnumHelper.WatermarkFont font { get; set; } = EnumHelper.WatermarkFont.simsun;

        /// <summary>
        /// /color/<color>	RRGGBB，如 FF0000（红色）	字体颜色，默认 000000（黑色）
        /// </summary>
        public string color { get; set; } = "FF0000";
        /// <summary>
        /// /border/<border>	RRGGBBAA，如 FF000000（不透明红色）	文字描边，默认 FFFFFFFF（透明白色），详见 border 说明
        /// </summary>
        public string border { get; set; } = "FFFFFFFF";

        /// <summary>
        ///  /align/<align>	位置，如 north  文字放置方位，默认 northwest，详见方位说明
        /// https://help.upyun.com/knowledge-base/image/#align_gravity
        /// </summary>
        public EnumHelper.Align align { get; set; } = EnumHelper.Align.southeast;

        /// <summary>
        /// /margin/<x>x<y> 横偏移x纵偏移，如 15×10	文字横纵相对偏移，默认 20x20
        /// </summary>
        public string margin { get; set; } = "5x5";

        /// <summary>
        /// /opacity/<opacity>	透明度，如 90	文字透明度，默认 100，取值范围[0 - 100]，值越大越不透明，0 完全透明，100 完全不透明
        /// </summary>
        public int opacity { get; set; } = 100;

        /// <summary>
        ///  /animate/<boolean>	true	允许对动态图片加水印，默认 false
        /// </summary>
        public bool animate { get; set; } = false;



      















    }
}
