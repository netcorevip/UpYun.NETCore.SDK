using System.Collections.Generic;

namespace UpYun.NETCore.SDK
{

    /*
     * https://docs.upyun.com/api/rest_api/#_13
     */


    public class DirFileEntity
    {
        public List<files> files { get; set; }

        /// <summary>
        /// 下一页
        /// 返回下一次分页开始位置。它由一串 Base64 编码的随机数组成，当它是 g2gCZAAEbmV4dGQAA2VvZg 时，表示最后一个分页。
        /// </summary>
        public string iter { get; set; }
    }

    public class files
    {
        //类型可选值：N 表示文件，F 表示目录。
        /// <summary>
        /// 文件/目录类型，folder：表示目录
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 大小
        /// </summary>
        public int length { get; set; }

        /// <summary>
        /// 文件/目录名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public int last_modified { get; set; }
    }
}
