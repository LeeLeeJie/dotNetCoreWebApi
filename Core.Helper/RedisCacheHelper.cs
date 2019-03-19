using Core.Model.ConfigModel;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Core.Helper
{
   public  class RedisCacheHelper
    {
        protected IDistributedCache cache;

        /// <summary>
        /// 通过IDistributedCache来构造RedisCache缓存操作类
        /// </summary>
        /// <param name="cache">IDistributedCache对象</param>
        public RedisCacheHelper(IDistributedCache cache)
        {
            this.cache = cache;
        }

        /// <summary>
        /// 添加或更改Redis的键值，并设置缓存的过期策略
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="distributedCacheEntryOptions">设置Redis缓存的过期策略，可以用其设置缓存的绝对过期时间（AbsoluteExpiration或AbsoluteExpirationRelativeToNow），也可以设置缓存的滑动过期时间（SlidingExpiration）</param>
        public void Set(string key, object value, DistributedCacheEntryOptions distributedCacheEntryOptions)
        {
            //通过Json.NET序列化缓存对象为Json字符串
            //调用JsonConvert.SerializeObject方法时，设置ReferenceLoopHandling属性为ReferenceLoopHandling.Ignore，来避免Json.NET序列化对象时，因为对象的循环引用而抛出异常
            //设置TypeNameHandling属性为TypeNameHandling.All，这样Json.NET序列化对象后的Json字符串中，会包含序列化的类型，这样可以保证Json.NET在反序列化对象时，去读取Json字符串中的序列化类型，从而得到和序列化时相同的对象类型
            var stringObject = JsonConvert.SerializeObject(value, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.All
            });

            var bytesObject = Encoding.UTF8.GetBytes(stringObject);//将Json字符串通过UTF-8编码，序列化为字节数组

            cache.Set(key, bytesObject, distributedCacheEntryOptions);//将字节数组存入Redis
            Refresh(key);//刷新Redis
        }

        /// <summary>
        /// 查询键值是否在Redis中存在
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>true：存在，false：不存在</returns>
        public bool Exist(string key)
        {
            var bytesObject = cache.Get(key);//从Redis中获取键值key的字节数组，如果没获取到，那么会返回null

            if (bytesObject == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 从Redis中获取键值
        /// </summary>
        /// <typeparam name="T">缓存的类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="isExisted">是否获取到键值，true：获取到了，false：键值不存在</param>
        /// <returns>缓存的对象</returns>
        public T Get<T>(string key, out bool isExisted)
        {
            var bytesObject = cache.Get(key);//从Redis中获取键值key的字节数组，如果没获取到，那么会返回null

            if (bytesObject == null)
            {
                isExisted = false;
                return default(T);
            }

            var stringObject = Encoding.UTF8.GetString(bytesObject);//通过UTF-8编码，将字节数组反序列化为Json字符串

            isExisted = true;

            //通过Json.NET反序列化Json字符串为对象
            //调用JsonConvert.DeserializeObject方法时，也设置TypeNameHandling属性为TypeNameHandling.All，这样可以保证Json.NET在反序列化对象时，去读取Json字符串中的序列化类型，从而得到和序列化时相同的对象类型
            return JsonConvert.DeserializeObject<T>(stringObject, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            });
        }

        /// <summary>
        /// 从Redis中获取键值
        /// </summary>
        /// <typeparam name="T">缓存的类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="isExisted">是否获取到键值，true：获取到了，false：键值不存在</param>
        /// <returns>缓存的对象</returns>
        public string Get(string key)
        {
            string stringObject = string.Empty;
            var bytesObject = cache.Get(key);//从Redis中获取键值key的字节数组，如果没获取到，那么会返回null

            if (bytesObject != null)
            {
                stringObject= Encoding.UTF8.GetString(bytesObject);//通过UTF-8编码，将字节数组反序列化为Json字符串
            }
            return stringObject;



        }

        /// <summary>
        /// 从Redis中删除键值，如果键值在Redis中不存在，该方法不会报错，只是什么都不会发生
        /// </summary>
        /// <param name="key">缓存键</param>
        public void Remove(string key)
        {
            cache.Remove(key);//如果键值在Redis中不存在，IDistributedCache.Remove方法不会报错，但是如果传入的参数key为null，则会抛出异常
        }

        /// <summary>
        /// 从Redis中刷新键值
        /// </summary>
        /// <param name="key">缓存键</param>
        public void Refresh(string key)
        {
            cache.Refresh(key);
        }
    }
}
