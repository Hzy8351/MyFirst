using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Assets.Editor
{
    public class BuildPlayer : EditorWindow
    {
        [MenuItem("资源管理/打包", false, 101)]
        public static void ShowExample()
        {
            GetWindow<BuildPlayer>("构建设置", new Type[] { typeof(BuildPlayer), typeof(AssetCollectRuleEditor) });
            //wnd.Close();
        }

        /// <summary>
        ///  
        /// </summary>
        BuildSettings settings;

        ListView _settingListView;

        VisualElement _rightContainer;

        Toggle _settingActive;

        PopupField<Assets.AssetsModel> _curPlayModel;
        /// <summary>
        /// 名字
        /// </summary>
        TextField _settingName;

        TextField _settingBuildPath;

        TextField _settingVersion;

        PopupField<SystemLanguage> _settingLanguage;

        VisualElement _buildCatlogs;

        TextField _settingUrl;

        TextField _settingChannel;

        TextField _settingStartScene;
        
        Toggle _settingConsole;

        Toggle _settingOffline;

        Toggle _settingLocalLanguage;

        VisualElement _resourceSetting;

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/AssetsManager/Editor/BuildPlayer.uxml");
            if (visualTree == null)
                return;
            visualTree.CloneTree(root);

            settings = BuildSettings.GetInstance();
            try
            {
                var export = root.Q<Button>("ExportButton");
                if (export != null) export.clicked += OnExportClicked;

                var import = root.Q<Button>("ImportButton");
                if (import != null) import.clicked += OnImportClicked;

                // bind data
                _settingListView = root.Q<ListView>("SettingListView");
                _settingListView.fixedItemHeight = 40;//itemHeight = 40;
                _settingListView.makeItem = OnMakSettingListItem;
                _settingListView.bindItem = OnBindSettingListItem;
                _settingListView.onSelectionChange += OnSettingListSelectionChanged;
                _settingListView.itemsSource = settings.Settings;
                _settingListView.reorderable = true;
                // 分组添加删除按钮
                {
                    var groupAddContainer = root.Q("SettingListOpr");
                    var addBtn = groupAddContainer.Q<Button>("AddBtn");
                    addBtn.clicked += OnSettingListAddClicked;
                    var removeBtn = groupAddContainer.Q<Button>("RemoveBtn");
                    removeBtn.clicked += OnSettingListRemoveClicked;
                }

                /// right container
                _rightContainer = root.Q<VisualElement>("RightContainer");
                // group active
                _settingActive = root.Q<Toggle>("GroupActive");
                _settingActive.RegisterValueChangedCallback(eve =>
                {
                    var curSetting = _settingListView.selectedItem as BuildSetting;
                    if (curSetting != null)
                        OnSettingActiveChanged(curSetting, eve.newValue);
                });
                /// find sub element
                _settingName = root.Q<TextField>("BuildName");
                _settingName.RegisterValueChangedCallback(eve =>
                {
                    var setting = _settingListView.selectedItem as BuildSetting;
                    if (setting != null)
                    {
                        setting.Name = eve.newValue;
                        _settingListView.Rebuild();
                    }
                });
                /// find sub element
                _settingBuildPath = root.Q<TextField>("BuildPath");
                _settingBuildPath.RegisterValueChangedCallback(eve =>
                {
                    var setting = _settingListView.selectedItem as BuildSetting;
                    if (setting != null)
                    {
                        setting.BuildPath = eve.newValue;
                        _settingListView.Rebuild();
                    }
                });
                var refreshbuilds = root.Q<Button>("RefreshBuilds");
                if (refreshbuilds != null)
                {
                    refreshbuilds.clicked += () =>
                    {
                        var setting = _settingListView.selectedItem as BuildSetting;
                        if (setting != null)
                        {
                            BuildSettings.ResetBuilds(setting);
                            RefreshBuildList(setting);
                        }
                    };
                }
                /// 版本
                _settingVersion = root.Q<TextField>("Version");
                _settingVersion.RegisterValueChangedCallback(eve =>
                {
                    var setting = _settingListView.selectedItem as BuildSetting;
                    if (setting != null)
                    {
                        setting.Version = eve.newValue;
                        _settingListView.Rebuild();
                    }
                });
                var language = root.Q<VisualElement>("Set.Language");
                language.Clear();
                _settingLanguage = new PopupField<SystemLanguage>("语言设置", BuildSettings.EnableLanguages, BuildSettings.EnableLanguages[0]);
                _settingLanguage.RegisterValueChangedCallback(eve =>
                {
                    var setting = _settingListView.selectedItem as BuildSetting;
                    if (setting != null)
                    {
                        setting.Language = eve.newValue;
                    }
                });
                language.Add(_settingLanguage);
                /// remote url
                _settingUrl = root.Q<TextField>("Set.RemoteURL");
                _settingUrl.RegisterValueChangedCallback(eve =>
                {
                    var setting = _settingListView.selectedItem as BuildSetting;
                    if (setting != null)
                    {
                        setting.remoteUri = eve.newValue;
                    }
                });
                /// StartScene
                _settingStartScene = root.Q<TextField>("Set.StartScene");
                _settingStartScene.RegisterValueChangedCallback(eve =>
                {
                    var setting = _settingListView.selectedItem as BuildSetting;
                    if (setting != null)
                    {
                        setting.startScene = eve.newValue;
                    }
                });
                /// Channel
                _settingChannel = root.Q<TextField>("Set.Channel");
                _settingChannel.RegisterValueChangedCallback(eve =>
                {
                    var setting = _settingListView.selectedItem as BuildSetting;
                    if (setting != null)
                    {
                        setting.channel = eve.newValue;
                    }
                });
                /// Console
                _settingConsole = root.Q<Toggle>("Set.Console");
                _settingConsole.RegisterValueChangedCallback(eve =>
                {
                    var setting = _settingListView.selectedItem as BuildSetting;
                    if (setting != null)
                    {
                        setting.enableDebugLogger = eve.newValue;
                    }
                });
                /// Console
                _settingOffline = root.Q<Toggle>("Set.Offline");
                _settingOffline.RegisterValueChangedCallback(eve =>
                {
                    var setting = _settingListView.selectedItem as BuildSetting;
                    if (setting != null)
                    {
                        setting.offlineMode = eve.newValue;
                    }
                });
                /// Console
                //_settingLocalLanguage = root.Q<Toggle>("Set.LocalLanguage");
                //_settingLocalLanguage.RegisterValueChangedCallback(eve =>
                //{
                //    var setting = _settingListView.selectedItem as BuildSetting;
                //    if (setting != null)
                //    {
                //        setting.useLocalLanguage = eve.newValue;
                //    }
                //});

                _buildCatlogs = root.Q<VisualElement>("CatlogChoose");
                _resourceSetting = root.Q<VisualElement>("LanguageContainer");

                var modelParent = root.Q<VisualElement>("PlayModel");
                if( modelParent != null)
                {
                    modelParent.style.flexDirection = FlexDirection.Column;
                    var lab = new Label("运行模式");
                    lab.style.fontSize = 20;
                    lab.style.left = 5;
                    lab.style.unityTextAlign = TextAnchor.MiddleLeft;
                    modelParent.Add(lab);
                    _curPlayModel = new PopupField<Assets.AssetsModel>(BuildSettings.PlayModelsList, Assets.AssetsModel.Editor);
                    _curPlayModel.style.top = 10;
                    _curPlayModel.value = settings.PlayModel;
                    _curPlayModel.RegisterValueChangedCallback((v) => settings.PlayModel = v.newValue);
                    modelParent.Add(_curPlayModel);
                }
                BindButton(_rightContainer, "ExportExcels", OnExportExcelsClicked);
                BindButton(_rightContainer, "ExportProto", OnExportProtoClicked);
                BindButton(_rightContainer, "BuildBundle", OnBuildBundleClicked);
                BindButton(_rightContainer, "CopyToStreaming", OnCopyStreamingClicked);
                BindButton(_rightContainer, "PublishBuild", OnPublishClicked);
                BindButton(_rightContainer, "BuildPlayer", OnBuildPlayerClicked);
                BindButton(_rightContainer, "ActiveSetting", OnActiveSettingClicked);
                BindButton(_rightContainer, "ApplySetting", OnApplySettingClicked);
                /// 设置当前选中
                var index = settings.GetActiveIndex();
                if (index > 0) _settingListView.SetSelectionWithoutNotify(new int[1] { index });
                RefreshView();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        void OnExportClicked()
        {
            // 采用特殊后缀 避免导入或导出的时候覆盖错文件
            var path = EditorUtility.SaveFilePanel("提示", Path.GetFullPath("./"), "BuildPlayerSettings", "setting");
            if (!string.IsNullOrEmpty(path))
            {
                var content = JsonUtility.ToJson(settings, true);
                File.WriteAllText(path, content);
            }
        }

        void OnImportClicked()
        {
            // 采用特殊后缀 避免导入或导出的时候覆盖错文件
            var path = EditorUtility.OpenFilePanel("提示", "选中导入文件", "setting");
            if (!string.IsNullOrEmpty(path))
            {
                var content = File.ReadAllText(path);
                JsonUtility.FromJsonOverwrite(content, settings);
                // 刷新界面
                _settingListView.Rebuild();
                RefreshView();
            }
        }

        VisualElement OnMakSettingListItem()
        {
            VisualElement element = new VisualElement();
            element.style.flexDirection = FlexDirection.Column;
            var name = new Label();
            name.name = "Name";
            name.style.unityTextAlign = TextAnchor.MiddleLeft;
            name.style.flexGrow = 1f;
            name.style.height = 30f;
            element.style.height = UnityEngine.Random.Range(1, 20) + 30;
            element.Add(name);
            return element;
        }

        void OnBindSettingListItem( VisualElement element, int index)
        {
            var item = settings.Settings[index];
            // Group Name
            string groupName = string.IsNullOrEmpty(item.Name) ? "null" : $"{item.Name} ({item.Version})";
            var name = element.Q<Label>("Name");
            if (name != null)
            {
                name.text = groupName;
                name.style.color = item.Active ? Color.green : Color.white;
            }
        }

        void OnSettingListSelectionChanged( IEnumerable<object> values)
        {
            RefreshView();
        }

        void OnSettingListAddClicked()
        {
            settings.Settings.Add(new BuildSetting());
            _settingListView.Rebuild();
        }

        void OnSettingListRemoveClicked()
        {
            var setting = _settingListView.selectedItem as BuildSetting;
            var index = settings.Settings.IndexOf(setting);
            if (index >= 0 && EditorUtility.DisplayDialog("错误", $"是否要删除{setting.Name}\n!", "确定"))
            {
                settings.Settings.RemoveAt(index);
                _settingListView.Rebuild();
            }
        }

        void OnSettingActiveChanged(BuildSetting setting, bool bActive)
        {

        }

        void BindButton( VisualElement element, string child, System.Action callback)
        {
            var child_e = element.Q<Button>(child);
            if(child_e != null)
            {
                child_e.clicked -= callback;
                child_e.clicked += callback;
            }
        }

        void RefreshView()
        {
            var curitem = _settingListView.selectedItem as BuildSetting;
            _rightContainer.visible = curitem != null;
            if (curitem == null)
                return;

            _settingName.SetValueWithoutNotify( curitem.Name);
            _settingBuildPath.SetValueWithoutNotify(curitem.BuildPath);
            _settingVersion.SetValueWithoutNotify( curitem.Version);
            _settingLanguage.SetValueWithoutNotify(curitem.Language); ;
            /// 刷新语言设置
            RefreshLanguageResItems( curitem);

            _settingUrl.SetValueWithoutNotify( curitem.remoteUri);
            _settingChannel.SetValueWithoutNotify( curitem.channel);
            _settingStartScene.SetValueWithoutNotify( curitem.startScene);
            _settingConsole.SetValueWithoutNotify( curitem.enableDebugLogger);
            _settingOffline.SetValueWithoutNotify( curitem.offlineMode);
            //_settingLocalLanguage.SetValueWithoutNotify( curitem.useLocalLanguage);
            /// 刷新构建列表
            RefreshBuildList(curitem);
        }

        void RefreshLanguageResItems( BuildSetting setting)
        {
            //_resourceSetting.Clear();
            //setting.CheckLanguageSettings();
            //foreach( var item in setting.ResourceSetting)
            //{
            //    MakeLanguageResItem(item);
            //}
        }

        //void MakeLanguageResItem(LocalizationItem item)
        //{
        //    VisualElement element = new VisualElement();
        //    element.style.minWidth = 150;
        //    element.style.backgroundColor = new Color( 67.0f/255, 67.0f / 255, 67.0f / 255, 1);
        //    _resourceSetting.Add(element);
        //    {
        //        var p = new VisualElement();
        //        p.style.flexDirection = FlexDirection.Row;

        //        var lab = new Label("语言");
        //        lab.style.width = 50;
        //        lab.style.unityTextAlign = TextAnchor.MiddleLeft;
        //        p.Add(lab);
        //        var lan = new PopupField<SystemLanguage>(new List<SystemLanguage>() { item.language }, item.language);
        //        lab.style.flexGrow = 1;
        //        p.Add(lan);
        //        element.Add(p);
        //    }
        //    {
        //        var p = new VisualElement();
        //        p.style.flexDirection = FlexDirection.Row;

        //        var lab = new Label("Res");
        //        lab.style.width = 50;
        //        lab.style.unityTextAlign = TextAnchor.MiddleLeft;
        //        p.Add(lab);
        //        var res = new TextField();
        //        res.value = item.ResourceDir;
        //        res.style.flexGrow = 1;
        //        res.RegisterValueChangedCallback((v) => item.ResourceDir = v.newValue);
        //        p.Add(res);
        //        element.Add(p);
        //    }
        //    var space = new VisualElement();
        //    space.style.width = 10;
        //    _resourceSetting.Add(space);
        //}

        void RefreshBuildList( BuildSetting setting)
        {
            _buildCatlogs.Clear();
            var catlogs = BuildSettings.GetBuilds( setting);
            foreach( var catlog in catlogs)
            {
                MakeBuildItem(setting, catlog);
            }
        }

        /// <summary>
        /// 创建build item
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="catlog"></param>
        void MakeBuildItem(BuildSetting setting, Catlog catlog)
        {
            VisualElement element = new VisualElement();
            element.style.flexDirection = FlexDirection.Row;
            _buildCatlogs.Add(element);

            var tog = new Toggle();
            tog.style.flexDirection = FlexDirection.Row;
            tog.value = setting.CurBuildData == catlog.buildDate;
            tog.RegisterValueChangedCallback((v) =>
            {
                if( v.newValue)
                {
                    setting.CurBuildData = catlog.buildDate;
                    RefreshBuildList(setting);
                }
            });
            element.Add(tog);

            var datetime = DateTimeOffset.FromUnixTimeSeconds(catlog.buildDate).ToLocalTime();
            var lab = new Label();
            lab.style.color = tog.value ? Color.green : Color.white;
            lab.style.unityTextAlign = TextAnchor.MiddleLeft;
            lab.style.left = 10;
            lab.text = $"版本:{catlog.version} <{catlog.buildDate}> 时间:{datetime.ToString("yyyy'-'MM'-'dd'  'HH':'mm':'ssK")} bundle:{ catlog.bundles?.Length} assets:{catlog.assets?.Length}    ";
            tog.Add(lab);

            var delete = new Button();
            delete.text = "删除";
            delete.clicked += () =>
            {
                if (EditorUtility.DisplayDialog("warring", $"是否要删除版本\n{lab.text}", "确定"))
                {
                    DeleteVersion(setting, catlog.buildDate);
                    BuildSettings.ResetBuilds(setting);
                    RefreshBuildList(setting);
                }
            };
            element.Add(delete);

            var copy = new Button();
            copy.text = "复制 BuildDate";
            copy.clicked += () =>
            {
                GUIUtility.systemCopyBuffer = catlog.buildDate.ToString();
            };
            element.Add(copy);
        }

        private void OnDestroy()
        {
            BuildSettings.SaveAsset( settings);
        }

        void OnExportExcelsClicked()
        {

        }

        void OnExportProtoClicked()
        {

        }

        static string GetAssetBundleName( AssetBundleManifest manifest, RuleResults.BundleInfo info )
        {
            var bundles = manifest.GetAllAssetBundles();
            foreach( var bundle in bundles)
            {
                var sp = bundle.LastIndexOf("_");
                var name = bundle.Substring(0, sp);
                if (name == info.Name)
                    return bundle;
            }
            return null;
        }

        /// <summary>
        /// 根据manifest 创建catlog
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        static Catlog MakeCatlog(BuildSetting setting, RuleResults result, AssetBundleManifest manifest)
        {
            var catlog = new Catlog();
            var bundles = new List<BundleInfo>(result.Bundles.Count);
            var save_path = setting.GetPrePublish(true);
            foreach (var item in result.Bundles)
            {
                item.Value.BundleID = bundles.Count;
                var bundleName = GetAssetBundleName( manifest, item.Value);
                var bundle = new BundleInfo();
                bundle.name = item.Value.Name;
                if (string.IsNullOrEmpty(bundleName))
                    Debug.Log(item.Value.Name);
                bundle.hashName = $"{manifest.GetAssetBundleHash(bundleName)}{BuildSetting.Extension}";
                bundle.bundleID = item.Value.BundleID;
                /// copy file
                var srcfile = Path.Combine(setting.GetBuildPath(), $"{bundle.name}_{bundle.hashName}");
                byte[] bytes = File.ReadAllBytes(srcfile);
#if ENCRYPE_RES
                ENRES.ConvertData(bytes);
#endif
                var dst_path = Path.Combine(save_path, bundle.hashName);
                File.WriteAllBytes(Path.Combine(save_path, bundle.hashName), bytes);
                bundle.size = bytes.Length;
                bundle.md5 = Utility.ComputeHash(dst_path);
                bundles.Add(bundle);
            }
            //bundles.Sort( (a,b)=>string.Compare(a.name, b.name));
            //for( int i=0; i < bundles.Count; i++) { bundles[i].bundleID = i; }
            foreach( var bundle in bundles)
            {
                bundle.deps = Array.ConvertAll(manifest.GetAllDependencies($"{bundle.name}_{bundle.hashName}"), (input)=> {
                    var tmp = input.Substring( 0, input.LastIndexOf('_'));
                    return result.Bundles[tmp].BundleID;
                });
            }

            catlog.bundles = bundles.ToArray();
            var assets = new List<AssetInfo>(result.Asset2Bundle.Count);
            var depList = new List<int>(10);
            foreach (var item in result.Assets)
            {
                //if (item.Value.AssetPath == "Assets/BaseResources/MapBase/selectedRed.prefab")
                //    Debug.Log("***");
                if (!item.Value.Loadable())
                    continue;
                var asset = new AssetInfo() { name = item.Key };
                asset.bundle = result.GetBundle( item.Key);
                // 目前因为依赖处理bug 问题暂时还原ab引用ab
                //if (result.Assets.TryGetValue(item.Key, out var tmp))
                //{
                //    depList.Clear();
                //    if (tmp.SubAssets != null && tmp.SubAssets.Length > 0)
                //    {
                //        foreach (var sub in tmp.SubAssets)
                //        {
                //            var bundleID = result.GetBundle( sub);
                //            if (depList.Contains(bundleID))
                //                continue;
                //            depList.Add(bundleID);
                //        }
                //        //asset.deps = depList.ToArray();
                //    }
                //}
                assets.Add(asset);
            }
            catlog.assets = assets.ToArray();
            return catlog;
        }

        static bool CompareDeps(int[] deps1, int[] deps2)
        {
            if (deps1 == null && deps2 == null)
                return true;
            if (deps1 == null && deps2 == null)
                return false;
            if (deps1 != null && deps2 == null)
                return false;
            if (deps1.Length != deps2.Length)
                return false;
            foreach( var item in deps1)
            {
                if (Array.IndexOf(deps2, item) < 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 比较catlog 是否一致
        /// </summary>
        /// <param name="catlog1"></param>
        /// <param name="catlog2"></param>
        /// <returns></returns>
        static bool CompareCatlog(Catlog catlog1, Catlog catlog2)
        {
            /// 对比平台
            if (catlog1.version != catlog2.version || catlog1.platform != catlog2.platform)
                return false;
            /// 对比bundles 数量 和资源数量
            if (catlog1.bundles.Length != catlog2.bundles.Length || catlog1.assets.Length != catlog2.assets.Length)
                return false;
            /// 对比bundle 信息
            foreach (var bundle1 in catlog1.bundles)
            {
                var bundle2 = Array.Find(catlog2.bundles, (a) => a.name == bundle1.name);
                if (bundle2 == null || bundle1.hashName != bundle2.hashName)
                    return false;
                if (!CompareDeps(bundle1.deps, bundle2.deps))
                    return false;
            }
            /// 防止 assset 信息不一致 & 这个和资源是否可加载有关
            Dictionary<string, AssetInfo> assets = new Dictionary<string, AssetInfo>();
            foreach (var item in catlog2.assets)
            {
                if (!assets.ContainsKey(item.name))
                    assets.Add(item.name, item);
            }
            foreach (var item in catlog1.assets)
            {
                if (!assets.ContainsKey(item.name))
                    return false;
            }
            return true;
        }

        public static Catlog BuildBundles(BuildSetting curSetting)
        {
            if (curSetting == null)
            {
                EditorUtility.DisplayDialog("打包错误", "请选择设置", "确定");
                return null;
            }

            var result = AssetGroups.CollectAssets();
            var build_bundles = result.GetBundleBuild(BuildSetting.Extension);
            var bundleOptions = BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.AppendHashToAssetBundleName;
            /// 重新构建避免缓存造成的打包未更新
            var build_path = curSetting.GetBuildPath(true);
            //foreach (var item in build_bundles)
            //{
            //    foreach (var p in item.assetNames)
            //    {
            //        Debug.Log($"{item.assetBundleName} => {p}");
            //    }
            //}
            //return null;

            //if (Directory.Exists(build_path))
            //    Directory.Delete(build_path, true);
            //Directory.CreateDirectory(build_path);
            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(build_path, build_bundles, bundleOptions, EditorUserBuildSettings.activeBuildTarget);
            if (manifest == null)
            {
                EditorUtility.DisplayDialog("打包错误", "打AssetBundle错误, manifest nil!", "确定");
                return null;
            }
            /// 生成catlog
            var catlog = MakeCatlog(curSetting, result, manifest);
            catlog.version = curSetting.Version;
            catlog.buildDate = DateTimeOffset.Now.ToUnixTimeSeconds();
            catlog.platform = BuildSettings.GetPlatformName();
            /// 对比是否有一样的catlog 只需要对比catlog 的bundle 数量和hash 是否一直
            var catlogs = BuildSettings.GetBuilds(curSetting);
            if (catlogs.Count > 0 && CompareCatlog(catlog, catlogs[0]))
            {
                Debug.Log("[BuildPlayer] 与上次构建的目录一致，无需更新!");
                return catlogs[0];
            }
            /// 保存为压缩格式
            var path = Path.Combine(curSetting.GetPrePublish(), Catlog.GetCatlogFile(catlog.buildDate));
            File.WriteAllBytes(path, catlog.ToJson(true));
            /// 写入可读取格式
            File.WriteAllText(path + ".json", JsonUtility.ToJson(catlog, true));
            /// 将bundles 包含的资源输出到文件中 以便于debug 查看bundle中文件变动情况
            var debugs = DebugBundles.Convert(build_bundles);
            File.WriteAllText(path + ".bundles", JsonUtility.ToJson(debugs, true));
            return catlog;
        }

        /// <summary>
        /// 开始构建AssetBundle
        /// </summary>
        void OnBuildBundleClicked()
        {
            var curSetting = _settingListView.selectedItem as BuildSetting;
            /// 打包配置
            var catlog = BuildBundles( curSetting);
            /// 刷新UI
            BuildSettings.ResetBuilds(curSetting);
            RefreshBuildList(curSetting);
        }

        /// <summary>
        /// 将选定版本拷贝到StreamingAssets目录
        /// </summary>
        void OnCopyStreamingClicked()
        {
            var curitem = _settingListView.selectedItem as BuildSetting;
            if (curitem == null)
                return;
            CopyBundleTo(curitem, Application.streamingAssetsPath, true);
        }

        /// <summary>
        /// 发布指定版本到
        /// </summary>
        void OnPublishClicked()
        {
            var curitem = _settingListView.selectedItem as BuildSetting;
            if (curitem == null)
                return;
            CopyBundleTo(curitem, curitem.GetPublish(), true);
        }

        void OnActiveSettingClicked()
        {
            var curitem = _settingListView.selectedItem as BuildSetting;
            if (curitem == null)
                return;
            settings.SetActive(curitem);
            _settingListView.Rebuild();
        }

        void OnApplySettingClicked()
        {
            var curitem = _settingListView.selectedItem as BuildSetting;
            if (curitem == null)
                return;
            ApplyGameSetting(curitem);
        }


        /// <summary>
        /// 直接构建包
        /// </summary>
        void OnBuildPlayerClicked()
        {
            /// 先激活该流程
            var curitem = _settingListView.selectedItem as BuildSetting;
            if (curitem == null)
                return;
            /// 查找build 是否存在
            settings.SetActive(curitem);
            _settingListView.Rebuild();
        }

        /// <summary>
        /// 删除指定版本
        /// </summary>
        /// <param name="buildDate"></param>
        static public void DeleteVersion( BuildSetting setting, long buildDate)
        {
            if (Application.isPlaying && EditorUtility.DisplayDialog("错误", "请退出运行模式！", "确定"))
                return;

            if (setting == null && EditorUtility.DisplayDialog("错误", "请激活打包的配置！", "确定"))
                return;
            // 确认打包平台文件夹
            var prebuild_path = setting.GetPrePublish();
            /// setting
            var catlogs = BuildSettings.GetBuilds(setting);
            HashSet<BundleInfo> bundles = new HashSet<BundleInfo>();
            var selver = catlogs.Find(a => a.buildDate == buildDate);
            if (selver != null)
            {
                foreach (var bundle in selver.bundles)
                {
                    if (!bundles.Contains(bundle))
                    {
                        bundles.Add(bundle);
                    }
                }
                foreach (var ver in catlogs)
                {
                    if (ver == selver)
                        continue;
                    foreach (var bundle in ver.bundles)
                    {
                        bundles.RemoveWhere( (a)=> a.hashName == bundle.hashName);
                    }
                }

                var build_path = setting.GetBuildPath();
                // 删除对应的版本以及catlog
                if (EditorUtility.DisplayDialog("删除确认", $"是否要删除当前版本：{selver.version} 日期：{selver.buildDate} 共:{bundles.Count}个", "确定", "取消"))
                {
                    foreach (var item in bundles)
                    {
                        try
                        {
                            File.Delete(Path.Combine(prebuild_path, item.hashName));
                            // 删除对应的manifest
                            var source_file = Path.Combine(build_path, $"{item.name}_{item.hashName}");
                            if (File.Exists(source_file)) File.Delete(source_file);
                            source_file = Path.Combine(build_path, $"{item.name}{BuildSetting.Extension}.manifest");
                            if (File.Exists(source_file)) File.Delete(source_file);
                        }
                        catch
                        {
                            Debug.Log($"删除文件失败:{Path.Combine(prebuild_path, item.hashName)}");
                        }
                    }
                    Debug.Log($"删除Bundle:{bundles.Count} 个");
                    // 删除catlog 文件
                    var catlog = Catlog.GetCatlogFile(buildDate);
                    try
                    {
                        var file = Path.Combine(prebuild_path, catlog);
                        File.Delete(file);
                        // 删除Json catlog
                        var jsonfile = file + ".json";
                        if (File.Exists(jsonfile)) File.Delete(jsonfile);
                        var bundlefile = file + ".bundles";
                        if (File.Exists(bundlefile)) File.Delete(bundlefile);
                    }
                    catch { Debug.LogError($"删除文件失败:{catlog}"); }
                }
            }
        }

        /// <summary>
        /// 加载指定catlog
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        static public Catlog LoadFromFile(string file)
        {
            if (File.Exists(file))
                return Catlog.LoadCatlogFromBytes(File.ReadAllBytes(file));
            return null;
        }

        /// <summary>
        /// 更新当前buildsetting 的配置
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="catlog"></param>
        static void UpdateAssetsConfig(BuildSetting setting, Catlog catlog)
        {
            // 拿到GameSetting & 设置Building
            var path = $"Assets/Resources/{nameof(AssetsSetting)}.asset";
            AssetsSetting config = File.Exists(path) ? AssetDatabase.LoadAssetAtPath<AssetsSetting>(path) : null;
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<AssetsSetting>();
                AssetDatabase.CreateAsset(config, path);
            }
            config.config = new AssetsSetting.Config()
            {
                Version = setting.Version,
                LoadScene = setting.startScene,
                BuildDate = setting.CurBuildData,
                Offline = setting.offlineMode
            };
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssetIfDirty(config);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 复制当前选定的AssetBundle 到指定目录
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="savePath"></param>
        /// <param name="forBuild"></param>
        static public void CopyBundleTo(BuildSetting setting, string savePath, bool forBuild)
        {
            if (setting == null && EditorUtility.DisplayDialog("错误", "请激活打包的配置！", "确定"))
                throw new Exception("请激活打包的配置！");
            // 确认打包平台文件夹
            var fromDirectory = setting.GetPrePublish();
            // 获取版本文件 如果Version 不存在 那么使用默认版本
            var catlogFile = Path.Combine(fromDirectory, Catlog.GetCatlogFile(setting.CurBuildData));
            var catlog = LoadFromFile(catlogFile);
            if (catlog == null && EditorUtility.DisplayDialog("错误", $"当前 build:{setting.CurBuildData} 版本不存在！", "确定"))
                throw new Exception($"当前 build:{setting.CurBuildData} 版本不存在！");
            // update config
            if (forBuild)
                UpdateAssetsConfig(setting, catlog);
            /// set property
            CopyCatlog(catlog, fromDirectory, savePath, forBuild);
            if (forBuild)
            {
                PlayerSettings.bundleVersion = setting.Version;
                Debug.Log($"设置Player版本为:{setting.Version}");
            }
        }

        static public void CopyCatlog(Catlog catlog, string src, string dst, bool forBuild)
        {
            /// set property
            Dictionary<string, BundleInfo> copyBundles = new Dictionary<string, BundleInfo>();
            foreach (var item in catlog.bundles)
            {
                copyBundles.Add(item.hashName, item);
            }
            // 清空文件夹
            if (forBuild && Directory.Exists(dst))
                Directory.Delete(dst, true);
            // create directory
            Directory.CreateDirectory(dst);
            // 拷贝文件
            foreach (var bundle in copyBundles)
            {
                File.Copy(Path.Combine(src, bundle.Value.hashName), Path.Combine(dst, bundle.Value.hashName), true);
            }
            Debug.Log("复制bundle 完成");
            var catlogFile = Path.Combine(src, Catlog.GetCatlogFile(catlog.buildDate));
            File.Copy(catlogFile, Path.Combine(dst, Catlog.GetCatlogFile(catlog.buildDate)), true);
            Debug.Log($"复制 catlog 文件 完成:{catlogFile}");
        }


        /// <summary>
        /// 将当前设置的资源拷贝到 StreamingAssets
        /// </summary>
        static public void CopyBundlesToBuild()
        {
            //var setting = BuildSettings.GetInstance().GetActiveSetting();
            //CopyBundleTo(setting, Application.streamingAssetsPath, true);
            //// apply game setting
            //ApplyGameSetting(setting);
        }

        /// <summary>
        /// 应用当前设置到游戏启动设置里面
        /// </summary>
        /// <param name="setting"></param>
        static public void ApplyGameSetting(BuildSetting setting)
        {
            var gamesetting = Assets.Setting.Instance();
            if (gamesetting == null)
            {
                gamesetting = CreateInstance<Assets.Setting>();
                AssetDatabase.CreateAsset(gamesetting, $"Assets/Resources/{nameof(Setting)}.asset");
            }
            gamesetting.Channel = setting.channel;
            gamesetting.RemoteURL = setting.remoteUri;
            gamesetting.EnableDebugLogger = setting.enableDebugLogger;
            gamesetting.StartScene = setting.startScene;
            UnityEditor.EditorUtility.SetDirty(gamesetting);
            UnityEditor.AssetDatabase.SaveAssets();
        }
    }
}