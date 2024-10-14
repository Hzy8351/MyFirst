using System;
using System.Collections.Generic;
using System.IO;

namespace Assets
{
    public class Reference
    {
        public int count { get; private set; }
        //public bool unused
        //{
        //    get { return count <= 0; }
        //}
        public void Retain()
        {
            count++;
        }
        public void Release()
        {
            count--;
        }

    }

    public enum LoadableStatus
    {
        Wait,
        Loading,
        DependentLoading,
        SuccessToLoad,
        FailedToLoad,
        Unloaded
    }

    /// <summary>
    /// loadable
    /// </summary>
    public class Loadable
    {
        protected static readonly List<Loadable> Loading = new List<Loadable>();
        protected static readonly List<Loadable> Unused = new List<Loadable>();


        protected readonly Reference _reference = new Reference();
        public LoadableStatus status { get; protected set; } = LoadableStatus.Wait;
        public string pathOrURL { get; protected set; }
        public string error { get; internal set; }
        public float progress { get; protected set; }

        public bool IsDone()
        {
            return status == LoadableStatus.SuccessToLoad || status == LoadableStatus.Unloaded ||
                              status == LoadableStatus.FailedToLoad;
        }

        public bool IsUnused()
        {
            return _reference.count <= 0;
        }

        protected void Finish(string errorCode = null)
        {
            error = errorCode;
            status = string.IsNullOrEmpty(errorCode) ? LoadableStatus.SuccessToLoad : LoadableStatus.FailedToLoad;
            progress = 1;
        }

        public static void UpdateAll()
        {
            for (var index = 0; index < Loading.Count; index++)
            {
                var item = Loading[index];
                if (Updater.Busy()) 
                    return;

                item.Update();
                if (!item.IsDone()) 
                    continue;

                Loading.RemoveAt(index);
                index--;
                item.Complete();
            }

            if (Scene.IsProgressing() || Asset.UnloadLockCount() > 0) 
                return;

            for (int index = 0, max = Unused.Count; index < max; index++)
            {
                var item = Unused[index];
                if (Updater.Busy())
                    break;

                if (!item.IsDone()) 
                    continue;

                Unused.RemoveAt(index);
                index--;
                max--;
                if (!item.IsUnused()) 
                    continue;

                item.Unload();
            }
        }

        private void Update()
        {
            OnUpdate();
        }

        private void Complete()
        {
            if (status == LoadableStatus.FailedToLoad)
            {
                Logger.E("[Loadable] Unable to load {0} {1} with error: {2}", GetType().Name, pathOrURL, error);
                Release();
            }

            OnComplete();
        }

        protected virtual void OnUpdate() { }

        protected virtual void OnLoad() {  }

        protected virtual void OnUnload() { }

        protected virtual void OnComplete() { }

        public virtual void LoadImmediate()
        {
            throw new InvalidOperationException();
        }

        protected void Load()
        {
            _reference.Retain();
            /// 加入加载队列中
            Loading.Add(this);
            if (status != LoadableStatus.Wait) 
                return;
            /// call onload
            Logger.I("[Loadable] Load: {0} {1}.", GetType().Name, pathOrURL);
            status = LoadableStatus.Loading;
            progress = 0;
            OnLoad();
        }

        private void Unload()
        {
            if (status == LoadableStatus.Unloaded) 
                return;
            // log unused
            Logger.I("[Loadable] Unload: {0} {1}.", GetType().Name, Path.GetFileName(pathOrURL));
            OnUnload();
            status = LoadableStatus.Unloaded;
        }

        public void Release()
        {
            if (_reference.count <= 0)
                Logger.W("[Loadable] Release: {0} {1}.", GetType().Name, Path.GetFileName(pathOrURL));

            _reference.Release();
            if (!IsUnused())
                return;

            Unused.Add(this);
        }
    }
}