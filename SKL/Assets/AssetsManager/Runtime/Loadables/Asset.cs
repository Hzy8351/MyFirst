using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Object = UnityEngine.Object;

namespace Assets
{
    public class Asset : Loadable, IEnumerator
    {
        /// <summary>
        /// 已加载的资源缓存
        /// </summary>
        protected static readonly Dictionary<string, Asset> Cache = new Dictionary<string, Asset>();

        /// <summary>
        /// 资源卸载 锁 如果锁的数量大于0 那么会暂停资源卸载
        /// </summary>
        static readonly HashSet<string> UnloadLocks = new HashSet<string>();

        /// <summary>
        /// 加载完成回调
        /// </summary>
        public Action<Asset> completed;

        /// <summary>
        /// 加载完成的资源
        /// </summary>
        public Object asset { get; protected set; }

        /// <summary>
        /// 资源类型
        /// </summary>
        protected Type type { get; set; }

        public bool MoveNext() { return !IsDone(); }

        public object Current => null;

        void IEnumerator.Reset() { }

        public Task<Asset> Task
        {
            get
            {
                var tcs = new TaskCompletionSource<Asset>();
                completed += operation => { tcs.SetResult(this); };
                return tcs.Task;
            }
        }

        public T Get<T>() where T : Object
        {
            return asset as T;
        }

        protected void OnLoaded(Object target)
        {
            asset = target;
            Finish(asset == null ? $"[Asset] OnLoaded: asset is null!" : null);
        }

        protected override void OnComplete()
        {
            if (completed == null) 
                return;
            var saved = completed;
            completed -= saved;
            // call on complete
            saved.Invoke(this);
        }

        protected override void OnUnload()
        {
            completed = null;
            /// remove from cache
            string cacheKey = GetCacheKey(pathOrURL, type);
            Cache.Remove(cacheKey);
        }

        /// <summary>
        /// 判断这个资源是否被卸载 
        /// </summary>
        /// <returns></returns>
        public virtual bool IsVaild()
        {
            return status == LoadableStatus.SuccessToLoad;
        }

        public T LoadSubAsset<T>( string path) where T : UnityEngine.Object
        {
            return LoadSubAsset(path, typeof(T)) as T;
        }

        /// <summary>
        /// 加载子Sub Asset
        /// </summary>
        /// <param name="path"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual UnityEngine.Object LoadSubAsset( string path, Type t)
        {
            return null;
        }


        /// <summary>
        /// 异步加载
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="completed"></param>
        /// <returns></returns>
        public static Asset LoadAsync(string path, Type type, Action<Asset> completed = null)
        {
            return LoadInternal(path, type, completed);
        }

        /// <summary>
        /// 同步加载
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Asset Load(string path, Type type)
        {
            var asset = LoadInternal(path, type);
            asset?.LoadImmediate();
            return asset;
        }

        /// <summary>
        /// 模板异步加载
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="completed"></param>
        /// <returns></returns>
        public static Asset LoadAsync<T>(string path, Action<Asset> completed = null, bool v = false)
        {
            return LoadAsync(path, typeof(T), completed);
        }

        /// <summary>
        /// 模板类型加载
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>

        public static Asset Load<T>(string path)
        {
            return Load(path, typeof(T));
        }

        /// <summary>
        /// 内部加载函数
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="immediate"></param>
        /// <param name="completed"></param>
        /// <returns></returns>
        public static Asset LoadInternal(string path, Type type, Action<Asset> completed = null)
        {
            //Versions.GetActualPath(ref path);
            var assetInfo = Manager.GetAsset(path);
            if (assetInfo == null)
            {
                Logger.E("[Asset] LoadInternal: FileNotFoundException {0}", path);
                return null;
            }

            return LoadInternalNoCheck(assetInfo, type, completed);
        }

        /// 获取缓存的key&保证同资源不同类型也能加载成功 如 sprite 用Texture 加载
        public static string GetCacheKey(string path, Type type)
        {
            if (type == null)
                return path;
            return $"{type.Name}@{path}";
        }

        // 如果外部已经校验资源是否存在 这里就不需要在判断一次
        internal static Asset LoadInternalNoCheck(AssetInfo info, Type type, Action<Asset> completed = null)
        {
            string cacheKey = GetCacheKey(info.name, type);
            if (!Cache.TryGetValue(cacheKey, out var item))
            {
                item = Manager.CreateAsset(info, type);
                Cache.Add(cacheKey, item);
            }
            // complete
            if (completed != null)
                item.completed += completed;
            //item.immediate = mustCompleteOnNextFrame;
            item.Load();
            return item;
        }

        /// <summary>
        /// 锁定卸载避免跳转大型加载的过程的时候反复io
        /// </summary>
        /// <param name="key"></param>
        public static void LockUnloading( string key)
        {
            if( !UnloadLocks.Contains(key))
                UnloadLocks.Add(key);
        }

        /// <summary>
        /// 释放Unloading 锁
        /// </summary>
        /// <param name="key"></param>
        public static void UnlockUnloading( string key)
        {
            UnloadLocks.Remove(key);
        }

        public static int UnloadLockCount()
        {
            return UnloadLocks.Count;
        }
    }
}