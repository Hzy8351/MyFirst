using UnityEngine.SceneManagement;

namespace Assets
{

    public class BundledScene : Scene
    {
        AssetInfo assetInfo;

        Dependencies dependencies;

        //Bundle bundle;

        protected BundledScene(AssetInfo info, LoadSceneMode model)
        {
            pathOrURL = info.name;
            assetInfo = info;
            LoadMode = model;
        }

        protected override void OnUpdate()
        {
            if (status == LoadableStatus.DependentLoading)
                UpdateDependencies();
            else if (status == LoadableStatus.Loading) 
                UpdateLoading();
        }

        private void UpdateDependencies()
        {
            if (dependencies == null)
            {
                Finish($"[BundleScene] UpdateDependencies: {assetInfo.name} => {assetInfo.bundle} dependencies is nil!");
                return;
            }
            progress = dependencies.progress * 0.5f;
            if (!dependencies.IsDone())
                return;

            var assetBundle = dependencies.GetAssetBundle();
            if (assetBundle == null)
            {
                Finish($"[BundleScene] UpdateDependencies: {assetInfo.name} => {assetInfo.bundle} assetBundle is nil!");
                return;
            }
            // 加载场景
            AsyncLoadScene();
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            dependencies?.Release();
            dependencies = null;
        }

        protected override void OnLoad()
        {
            PrepareToLoad();
            status = LoadableStatus.DependentLoading;
            /// load dep
            dependencies = Dependencies.Load(assetInfo);
        }

        internal static Scene Create(AssetInfo info, bool additive = false)
        {
            var model = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
            return new BundledScene(info, model);
        }
    }
}