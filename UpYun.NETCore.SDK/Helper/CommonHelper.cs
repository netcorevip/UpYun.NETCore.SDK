using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace UpYun.NETCore.SDK
{
    public static class CommonHelper
    {

        /// <summary>
        /// 水印内容编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToWatermarkBase64(this string str)
        {
            string res = str.EncodeBase64().Replace("/", "|");
            return res;
        }

        /// <summary>
        /// 返回水印参数Url
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string ToWatermarkUrl<T>(this T entity)
        {
            Type type = entity.GetType();
            PropertyInfo[] propertyInfos = type.GetProperties();
            StringBuilder stringBuilder = new StringBuilder("/watermark");
            foreach (PropertyInfo item in propertyInfos)
            {
                switch (item.Name)
                {
                    case "text":
                    case "url":
                        if (!item.GetValue(entity, null).ToString().IsBase64())
                        {
                            throw new ArgumentException("水印内容不是有效的Base64字符串");
                            //return "";
                        }
                        break;
                }
                if (item.GetValue(entity, null) != null)
                {
                    stringBuilder.AppendFormat("/{0}/{1}", item.Name, item.GetValue(entity, null).ToString());
                }
            }
            return stringBuilder.ToString();
        }


        public static string ToFileGuidName(this string uploaddir, string filepatn)
        {
            string extension = Path.GetExtension(filepatn);//扩展名 “.aspx”
            string filepath = uploaddir + Guid.NewGuid().ToString() + extension;
            return filepath;
        }


        public static bool IsNull(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static bool IsNotNull(this string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }



    }
}
