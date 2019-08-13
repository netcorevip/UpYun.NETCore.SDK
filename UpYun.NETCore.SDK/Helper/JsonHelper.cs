using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UpYun.NETCore.SDK
{
    public static class JsonHelper
    {
        /*JSON扩展类——JsonHelper - VAllen - 博客园
        https://www.cnblogs.com/VAllen/p/JsonHelper.html*/

        private static readonly JsonSerializerSettings JsonSettings;

        private const string EmptyJson = "[]";

        static JsonHelper()
        {
            var datetimeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };

            JsonSettings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            JsonSettings.Converters.Add(datetimeConverter);
        }

        /// <summary>
        /// 应用Formatting.None和指定的JsonSerializerSettings设置,序列化对象到JSON格式的字符串
        /// </summary>
        /// <param name="obj">任意一个对象</param>
        /// <param name="jsonSettings">在一个 Newtonsoft.Json.JsonSerializer 对象上指定设置，如果为null，则使用默认设置</param>
        /// <returns>标准的JSON格式的字符串</returns>
        public static string ToJson(this object obj, JsonSerializerSettings jsonSettings)
        {
            return ToJson(obj, Formatting.None, jsonSettings);
        }

        /// <summary>
        /// 应用指定的Formatting枚举值None和指定的JsonSerializerSettings设置,序列化对象到JSON格式的字符串
        /// </summary>
        /// <param name="obj">任意一个对象</param>
        /// <param name="format">指定 Newtonsoft.Json.JsonTextWriter 的格式设置选项</param>
        /// <param name="jsonSettings">在一个 Newtonsoft.Json.JsonSerializer 对象上指定设置，如果为null，则使用默认设置</param>
        /// <returns>标准的JSON格式的字符串</returns>
        public static string ToJson(this object obj, Formatting format, JsonSerializerSettings jsonSettings)
        {
            try
            {
                return obj == null ? EmptyJson : JsonConvert.SerializeObject(obj, format, jsonSettings ?? JsonSettings);
            }
            catch (Exception)
            {
                //TODO LOG
                return EmptyJson;
            }
        }

        /// <summary>
        /// 应用Formatting.None和指定的JsonSerializerSettings设置,反序列化JSON数据为dynamic对象
        /// <para>如果发生JsonSerializationException异常，再以集合的方式重试一次，取出集合的第一个dynamic对象。</para>
        /// <para>转换失败，或发生其它异常，则返回dynamic对象的默认值</para>
        /// </summary>
        /// <param name="json">需要反序列化的JSON字符串</param>
        /// <param name="jsonSettings">在一个 Newtonsoft.Json.JsonSerializer 对象上指定设置，如果为null，则使用默认设置</param>
        /// <returns>dynamic对象</returns>
        public static dynamic FromJson(this string json, JsonSerializerSettings jsonSettings)
        {
            return FromJson<dynamic>(json, Formatting.None, jsonSettings);
        }

        /// <summary>
        /// 应用指定的Formatting枚举值None和指定的JsonSerializerSettings设置,反序列化JSON数据为dynamic对象
        /// <para>如果发生JsonSerializationException异常，再以集合的方式重试一次，取出集合的第一个dynamic对象。</para>
        /// <para>转换失败，或发生其它异常，则返回dynamic对象的默认值</para>
        /// </summary>
        /// <param name="json">需要反序列化的JSON字符串</param>
        /// <param name="format">指定 Newtonsoft.Json.JsonTextWriter 的格式设置选项</param>
        /// <param name="jsonSettings">在一个 Newtonsoft.Json.JsonSerializer 对象上指定设置，如果为null，则使用默认设置</param>
        /// <returns>dynamic对象</returns>
        public static dynamic FromJson(this string json, Formatting format, JsonSerializerSettings jsonSettings)
        {
            return FromJson<dynamic>(json, format, jsonSettings);
        }

        /// <summary>
        /// 应用Formatting.None和指定的JsonSerializerSettings设置,反序列化JSON数据到指定的.NET类型对象
        /// <para>如果发生JsonSerializationException异常，再以集合的方式重试一次，取出集合的第一个T对象。</para>
        /// <para>转换失败，或发生其它异常，则返回T对象的默认值</para>
        /// </summary>
        /// <param name="json">需要反序列化的JSON字符串</param>
        /// <param name="jsonSettings">在一个 Newtonsoft.Json.JsonSerializer 对象上指定设置，如果为null，则使用默认设置</param>
        /// <typeparam name="T">反序列化对象的类型</typeparam>
        /// <returns></returns>
        public static T FromJson<T>(this string json, JsonSerializerSettings jsonSettings) where T : class, new()
        {
            return FromJson<T>(json, Formatting.None, jsonSettings);
        }

        /// <summary>
        /// 应用指定的Formatting枚举值None和指定的JsonSerializerSettings设置,反序列化JSON数据到指定的.NET类型对象
        /// <para>如果发生JsonSerializationException异常，再以集合的方式重试一次，取出集合的第一个T对象。</para>
        /// <para>转换失败，或发生其它异常，则返回T对象的默认值</para>
        /// </summary>
        /// <param name="json">需要反序列化的JSON字符串</param>
        /// <param name="format">指定 Newtonsoft.Json.JsonTextWriter 的格式设置选项</param>
        /// <param name="jsonSettings">在一个 Newtonsoft.Json.JsonSerializer 对象上指定设置，如果为null，则使用默认设置</param>
        /// <typeparam name="T">反序列化对象的类型</typeparam>
        /// <returns></returns>
        public static T FromJson<T>(string json, Formatting format, JsonSerializerSettings jsonSettings) where T : class, new()
        {
            T result;

            if (jsonSettings == null)
            {
                jsonSettings = JsonSettings;
            }

            try
            {
                result = string.IsNullOrWhiteSpace(json) ? default(T) : JsonConvert.DeserializeObject<T>(json, jsonSettings);
            }
            catch (JsonSerializationException) //在发生该异常后，再以集合的方式重试一次.
            {
                //LOG
                try
                {
                    var array = JsonConvert.DeserializeObject<IEnumerable<T>>(json, jsonSettings);
                    result = array.FirstOrDefault();
                }
                catch (Exception)
                {
                    //LOG
                    result = default(T);
                }
            }
            catch (Exception)
            {
                //LOG
                result = default(T);
            }
            return result;
        }

        /// <summary>
        /// 反序列化JSON数据为dynamic对象
        /// <para>如果发生JsonSerializationException异常，再以集合的方式重试一次，取出集合的第一个dynamic对象。</para>
        /// <para>转换失败，或发生其它异常，则返回dynamic对象的默认值</para>
        /// </summary>
        /// <param name="json">需要反序列化的JSON字符串</param>
        /// <returns>dynamic对象</returns>
        public static dynamic FromJson(this string json)
        {
            return FromJson<dynamic>(json, Formatting.None, JsonSettings);
        }

        /// <summary>
        /// 反序列化JSON数据到指定的.NET类型对象
        /// <para>如果发生JsonSerializationException异常，再以集合的方式重试一次，取出集合的第一个T对象。</para>
        /// <para>转换失败，或发生其它异常，则返回T对象的默认值</para>
        /// </summary>
        /// <param name="json">需要反序列化的JSON字符串</param>
        /// <typeparam name="T">反序列化对象的类型</typeparam>
        /// <returns></returns>
        public static T FromJson<T>(this string json) where T : class, new()
        {
            return FromJson<T>(json, Formatting.None, JsonSettings);
        }

        /// <summary>
        /// 应用默认的Formatting枚举值None和默认的JsonSerializerSettings设置,序列化对象到JSON格式的字符串
        /// </summary>
        /// <param name="obj">任意一个对象</param>
        /// <returns>标准的JSON格式的字符串</returns>
        public static string ToJson(this object obj)
        {
            //   var jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            return ToJson(obj, Formatting.None, JsonSettings);
        }

        public static string ToJson<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, bool isFilterNull = true)
        {
            return DelegateToJson(source, enumerable => enumerable.Where(predicate), isFilterNull);
        }

        public static string ToJson<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate, bool isFilterNull = true)
        {
            return DelegateToJson(source, enumerable => enumerable.Where(predicate), isFilterNull);
        }

        public static string ToJson<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector, bool isFilterNull = true)
        {
            return DelegateToJson(source, enumerable => enumerable.Where(t => t != null).Select(selector), isFilterNull);
        }

        public static string ToJson<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, TResult> selector, bool isFilterNull = true)
        {
            return DelegateToJson(source, enumerable => enumerable.Where(t => t != null).Select(selector), isFilterNull);
        }

        public static string ToJson<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, Func<TSource, TResult> selector, bool isFilterNull = true)
        {
            return DelegateToJson(source, enumerable => enumerable.Where(predicate).Select(selector), isFilterNull);
        }

        public static string ToJson<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, Func<TSource, int, TResult> selector, bool isFilterNull = true)
        {
            return DelegateToJson(source, enumerable => enumerable.Where(predicate).Select(selector), isFilterNull);
        }

        public static string ToJson<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate, Func<TSource, TResult> selector, bool isFilterNull = true)
        {
            return DelegateToJson(source, enumerable => enumerable.Where(predicate).Select(selector), isFilterNull);
        }

        public static string ToJson<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate, Func<TSource, int, TResult> selector, bool isFilterNull = true)
        {
            return DelegateToJson(source, enumerable => enumerable.Where(predicate).Select(selector), isFilterNull);
        }

        /// <summary>
        /// 委托处理需要序列化为JSON格式的对象，返回标准的JSON格式的字符串。
        /// 默认过滤null对象，如果需要在上层调用时，自己进行条件过滤null对象，
        /// 则设置isFilterNull为false，不建议isFilterNull设置为false。
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source">需要转换为JSON格式字符串的对象</param>
        /// <param name="func">集合/数组条件筛选方法委托，返回筛选后的集合/数组</param>
        /// <param name="isFilterNull">是否过滤IEnumerable<TSource> source中的null对象，默认为true</param>
        /// <returns>标准的JSON格式的字符串</returns>
        private static string DelegateToJson<TSource, TResult>(IEnumerable<TSource> source, Func<TSource[], IEnumerable<TResult>> func, bool isFilterNull = true)
        {
            return DelegateToJson(source, enumerable => func(enumerable).ToJson(), isFilterNull);
        }

        /// <summary>
        /// 委托处理需要序列化为JSON格式的对象，返回标准的JSON格式的字符串。
        /// 默认过滤null对象，如果需要在上层调用时，自己进行条件过滤null对象，
        /// 则设置isFilterNull为false，不建议isFilterNull设置为false。
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">需要转换为JSON格式字符串的对象</param>
        /// <param name="func">JSON处理方法委托，返回JSON格式的字符串</param>
        /// <param name="isFilterNull">是否过滤IEnumerable<TSource> source中的null对象，默认为true</param>
        /// <returns>标准的JSON格式的字符串</returns>
        private static string DelegateToJson<TSource>(IEnumerable<TSource> source, Func<TSource[], string> func, bool isFilterNull = true)
        {
            if (source == null)
            {
                return EmptyJson;
            }

            TSource[] enumerable;
            if (isFilterNull)
            {
                //过滤null
                enumerable = source.Where(t => t != null).ToArray();
            }
            else
            {
                //不过滤null，但上层需要注意内里面有null对象时，可能会导致Where或Select引发异常。
                enumerable = source as TSource[] ?? source.ToArray();
            }

            return enumerable.Any() ? func(enumerable) : EmptyJson;
        }
    }
}