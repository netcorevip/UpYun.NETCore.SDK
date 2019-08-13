
namespace UpYun.NETCore.SDK
{
    public class EnumHelper
    {
        /* 
        又拍云文档中心
        https://docs.upyun.com/cloud/image/#_14
        */

        /*
            northwest     |     north      |     northeast
                          |                |
                          |                |
            --------------+----------------+--------------
                          |                |
            west          |     center     |          east
                          |                |
            --------------+----------------+--------------
                          |                |
                          |                |
            southwest     |     south      |     southeast
        */
        /// <summary>
        /// 位置
        /// </summary>
        public enum Align
        {
            /// <summary>
            /// 左上
            /// </summary>
            northwest,
            /// <summary>
            /// 左中
            /// </summary>
            west,
            /// <summary>
            /// 左下
            /// </summary>
            southwest,
            /// <summary>
            /// 上中
            /// </summary>
            north,
            /// <summary>
            /// 中间
            /// </summary>
            center,
            /// <summary>
            /// 下中
            /// </summary>
            south,
            /// <summary>
            /// 右上
            /// </summary>
            northeast,
            /// <summary>
            /// 右中
            /// </summary>
            east,
            /// <summary>
            /// 右下
            /// </summary>
            southeast,

        }



        /// <summary>
        /// 字体
        /// </summary>
        public enum WatermarkFont
        {

            /// <summary>
            /// 宋体  中文字体    
            /// </summary>
            simsun,
            /// <summary>
            /// 黑体  中文字体    
            /// </summary>
            simhei,
            /// <summary>
            /// 楷体  中文字体    
            /// </summary>
            simkai,
            /// <summary>
            /// 隶书  中文字体    
            /// </summary>
            simli,
            /// <summary>
            /// 幼圆  中文字体    
            /// </summary>
            simyou,
            /// <summary>
            /// 仿宋  中文字体    
            /// </summary>
            simfang,
            /// <summary>
            /// 简体中文    中文字体    
            /// </summary>
            sc,
            /// <summary>
            /// 繁体中文    中文字体    
            /// </summary>
            tc,
            /// <summary>
            /// Arial   英文字体    
            /// </summary>
            arial,
            /// <summary>
            /// Georgia 英文字体    
            /// </summary>
            georgia,
            /// <summary>
            /// Helvetica   英文字体    
            /// </summary>
            helvetica,
            /// <summary>
            /// Times-New-Roman 英文字体    
            /// </summary>
            roman,


        }







    }
}
