using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public sealed class InstantiateObject : Operation
    {
        private static readonly List<InstantiateObject> AllObjects = new List<InstantiateObject>();
        private Asset asset;
        private string path { get; set; }
        public GameObject result { get; private set; }

        public override void Start()
        {
            base.Start();
            asset = Asset.LoadAsync(path, typeof(GameObject));
            AllObjects.Add(this);
        }

        public static InstantiateObject InstantiateAsync(string assetPath)
        {
            var operation = new InstantiateObject
            {
                path = assetPath
            };
            operation.Start();
            return operation;
        }

        protected override void Update()
        {
            if (status == OperationStatus.Processing)
            {
                if (asset == null)
                {
                    Error("asset == null");
                    return;
                }

                progress = asset.progress;
                if (!asset.IsDone()) 
                    return;

                if (asset.status == LoadableStatus.FailedToLoad)
                {
                    Error(asset.error);
                    return;
                }

                if (asset.asset == null)
                {
                    Error("asset == null");
                    return;
                }

                result = Object.Instantiate(asset.asset as GameObject);
                Finish();
            }
        }


        public void Destroy()
        {
            if (!isDone)
            {
                Error("User Cancelled");
                return;
            }

            if (status == OperationStatus.Success)
                if (result != null)
                {
                    Object.DestroyImmediate(result);
                    result = null;
                }

            if (asset != null)
            {
                if (string.IsNullOrEmpty(asset.error)) asset.Release();

                asset = null;
            }
        }

        public static void UpdateObjects()
        {
            for (var index = 0; index < AllObjects.Count; index++)
            {
                var item = AllObjects[index];
                if (Updater.Busy()) 
                    return;

                if (!item.isDone || item.result != null) 
                    continue;

                AllObjects.RemoveAt(index);
                index--;
                item.Destroy();
            }
        }
    }
}