using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

namespace UpYun.NETCore.SDK
{
    public static class FileHelper
    {
        /// <summary>
        /// 读文件到byte[]
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static byte[] ReadFile(this string path)
        {
            FileStream pFileStream = null;
            byte[] pReadByte = new byte[0];
            try
            {
                pFileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                BinaryReader r = new BinaryReader(pFileStream);
                r.BaseStream.Seek(0, SeekOrigin.Begin);    //将文件指针设置到文件开
                pReadByte = r.ReadBytes((int)r.BaseStream.Length);
                return pReadByte;
            }
            catch
            {
                return pReadByte;
            }

            finally
            {
                pFileStream?.Close();
            }

        }

        /// <summary>
        /// 写byte[]到文件
        /// </summary>
        /// <param name="pReadByte"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool WriteFile(this byte[] pReadByte, string path)
        {
            FileStream pFileStream = null;
            try
            {
                pFileStream = new FileStream(path, FileMode.OpenOrCreate);
                pFileStream.Write(pReadByte, 0, pReadByte.Length);
            }

            catch
            {
                return false;
            }
            finally
            {
                if (pFileStream != null)
                    pFileStream.Close();
            }
            return true;

        }


        /// <summary>
        /// 计算文件的MD5码
        /// </summary>
        /// <param name="pathName">The pathName<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string MD5File(this string pathName)
        {
            string strResult = "";
            string strHashData = "";

            byte[] arrbytHashValue;
            System.IO.FileStream oFileStream = null;

            using (var md5 = MD5.Create())
            {
                oFileStream = new System.IO.FileStream(pathName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
                arrbytHashValue = md5.ComputeHash(oFileStream);//计算指定Stream 对象的哈希值
                //由以连字符分隔的十六进制对构成的String，其中每一对表示value 中对应的元素；例如“F-2C-4A”
                strHashData = System.BitConverter.ToString(arrbytHashValue);
                //替换-
                strHashData = strHashData.Replace("-", "");
                strResult = strHashData;
                return strResult.ToLower();
            }
        }


        /*
         * ObjectToBytes报错如下，暂时未解决
         * Unhandled Exception: System.Runtime.Serialization.SerializationException: Type 'System.Threading.Tasks.Task`1[[System.Byte[], System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]'
         * in Assembly 'System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e'
         * is not marked as serializable.
         */
        /// <summary> 
        /// 将一个object对象序列化，返回一个byte[]         
        /// </summary> 
        /// <param name="obj">能序列化的对象</param>         
        /// <returns></returns> 
        public static byte[] ObjectToBytes(this object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter(); formatter.Serialize(ms, obj); return ms.GetBuffer();
            }
        }

        /// <summary> 
        /// 将一个序列化后的byte[]数组还原         
        /// </summary>
        /// <param name="Bytes"></param>         
        /// <returns></returns> 
        public static object BytesToObject(this byte[] Bytes)
        {
            using (MemoryStream ms = new MemoryStream(Bytes))
            {
                IFormatter formatter = new BinaryFormatter(); return formatter.Deserialize(ms);
            }
        }


    }
}
