using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


namespace Assets.Editor
{
    public class AssetCollectRuleEditor : EditorWindow
    {
        /// <summary>
        /// asset groups
        /// </summary>
        AssetGroups assetGroup;

        /// <summary>
        /// 分组
        /// </summary>
        ListView _groupListView;

        /// <summary>
        /// 右侧容器
        /// </summary>
        VisualElement _rightContainer;

        /// <summary>
        /// 当前组名
        /// </summary>
        Toggle _groupActive;

        /// <summary>
        /// 当前组名
        /// </summary>
        TextField _groupName;

        /// <summary>
        /// 当前组名
        /// </summary>
        TextField _groupDesc;

        /// <summary>
        /// 分组
        /// </summary>
        ListView _curRuleListView;


        [MenuItem("资源管理/资源收集", false, 101)]
        public static void ShowExample()
        {
            AssetCollectRuleEditor wnd = GetWindow<AssetCollectRuleEditor>("资源收集", new Type[] { typeof(BuildPlayer), typeof(AssetCollectRuleEditor) });
            wnd.minSize = new Vector2(800, 600);
            //wnd.Close();
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/AssetsManager/Editor/AssetCollectRuleEditor.uxml");
            if (visualTree == null)
                return;
            visualTree.CloneTree(root);
            /// create element
            try
            {
                assetGroup = AssetGroups.GetInstance();

                var test = root.Q<Button>("TestButton");
                if (test != null) test.clicked += OnTestBtnClicked;

                var export = root.Q<Button>("ExportButton");
                if (export != null) export.clicked += OnExportClicked;

                var import = root.Q<Button>("ImportButton");
                if (import != null) import.clicked += OnImportClicked;

                // bind data
                _groupListView = root.Q<ListView>("GroupListView");
                _groupListView.fixedItemHeight = 40;//itemHeight = 40;
                _groupListView.makeItem = OnMakeGroupListItem;
                _groupListView.bindItem = OnBindGroupListItem;
                _groupListView.onSelectionChange += OnGroupListSelectionChanged;
                _groupListView.itemsSource = assetGroup.groups;
                _groupListView.reorderable = true;
                // 分组添加删除按钮
                {
                    var groupAddContainer = root.Q("GroupAddContainer");
                    var addBtn = groupAddContainer.Q<Button>("AddBtn");
                    addBtn.clicked += OnGroupListAddClicked;
                    var removeBtn = groupAddContainer.Q<Button>("RemoveBtn");
                    removeBtn.clicked += OnGroupListRemoveClicked;
                }

                /// right container
                _rightContainer = root.Q<VisualElement>("RightContainer");
                // group active
                _groupActive = root.Q<Toggle>("GroupActive");
                _groupActive.RegisterValueChangedCallback(eve =>
                {
                    var selectGroup = _groupListView.selectedItem as AssetGroup;
                    if (selectGroup != null)
                    {
                        selectGroup.Active = eve.newValue;
                        _groupListView.Rebuild();
                    }
                });
                /// find sub element
                _groupName = root.Q<TextField>("GroupName");
                _groupName.RegisterValueChangedCallback(eve =>
                {
                    var selectGroup = _groupListView.selectedItem as AssetGroup;
                    if (selectGroup != null)
                    {
                        selectGroup.GroupName = eve.newValue;
                        _groupListView.Rebuild();
                    }
                });
                _groupDesc = root.Q<TextField>("GroupDesc");
                _groupDesc.RegisterValueChangedCallback(eve =>
                {
                    var selectGroup = _groupListView.selectedItem as AssetGroup;
                    if (selectGroup != null)
                    {
                        selectGroup.GroupDescription = eve.newValue;
                        _groupListView.Rebuild();
                    }
                });

                // right grouplist
                {
                    var addBtn = _rightContainer.Q<Button>("AddBtn");
                    addBtn.clicked += OnRuleListAddClicked;
                    var removeBtn = _rightContainer.Q<Button>("RemoveBtn");
                    removeBtn.clicked += OnRuleListRemoveClicked;
                }
                // right grouplist
                _curRuleListView = _rightContainer.Q<ListView>("RuleListView");
                _curRuleListView.fixedItemHeight = 85;//itemHeight = 85;
                _curRuleListView.reorderable = true;
                _curRuleListView.makeItem = OnMakeRuleItem;
                _curRuleListView.bindItem = OnRuleBindRuleItem;
                /// 设置当前选中
                RefreshView();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        void RefreshView()
        {
            _groupListView.Rebuild();
            if (assetGroup.groups.Count > 0)
                _groupListView.SetSelectionWithoutNotify(new int[1] { 0 });
            SelectGroupListItem();
        }

        VisualElement OnMakeGroupListItem()
        {
            VisualElement element = new VisualElement();
            element.style.flexDirection = FlexDirection.Row;
            var name = new Label();
            name.name = "Name";
            name.style.unityTextAlign = TextAnchor.MiddleLeft;
            name.style.flexGrow = 1f;
            name.style.height = 30f;
            element.style.height = UnityEngine.Random.Range(1, 20) + 30;
            element.Add(name);
            // active
            var toggle = new Toggle();
            toggle.name = "Active";
            toggle.RegisterValueChangedCallback((t) => {
                var group = assetGroup.groups[element.tabIndex];
                if (group != null)
                {
                    group.Active = t.newValue;
                    _groupListView.Rebuild();
                }
            });
            element.Add(toggle);
            return element;
        }

        void OnBindGroupListItem(VisualElement element, int index)
        {
            var item = assetGroup.groups[index];
            element.tabIndex = index;
            // Group Name
            string groupName = string.IsNullOrEmpty(item.GroupName) ? "null" : $"{item.GroupName} : {item.GroupDescription}";
            var name = element.Q<Label>("Name");
            if (name != null)
            {
                name.text = groupName;
                name.style.color = item.Active ? Color.green : Color.white;
            }

            var active = element.Q<Toggle>("Active");
            if( active != null)
            {
                active.SetValueWithoutNotify(item.Active);
            }
        }

        void OnGroupListSelectionChanged(IEnumerable<object> objs)
        {
            SelectGroupListItem();
        }

        void OnGroupListAddClicked()
        {
            assetGroup.groups.Add(new AssetGroup());
            _groupListView.Rebuild();
        }

        void OnGroupListRemoveClicked()
        {
            var group = _groupListView.selectedItem as AssetGroup;
            var index = assetGroup.groups.IndexOf(group);
            if (index >= 0 && EditorUtility.DisplayDialog("错误", $"是否要删除{group.GroupName}\n删除后分组内的规则也将移除!", "确定"))
            {
                assetGroup.groups.RemoveAt(index);
                _groupListView.Rebuild();
            }
        }

        void SelectGroupListItem()
        {
            var group = _groupListView.selectedItem as AssetGroup;
            _rightContainer.visible = group != null;
            if (group == null)
                return;
            // 设置item name
            _groupActive.SetValueWithoutNotify(group.Active);
            _groupName.SetValueWithoutNotify(group.GroupName);
            _groupDesc.SetValueWithoutNotify(group.GroupDescription);
            /// clear other 
            UpdateRuleListView(group);
        }

        void OnRuleListAddClicked()
        {
            var group = _groupListView.selectedItem as AssetGroup;
            if (group == null)
                return;
            group.Rules.Add(new CollectRule());
            _curRuleListView.Rebuild();
        }

        void OnRuleListRemoveClicked()
        {
            var group = _groupListView.selectedItem as AssetGroup;
            if (group == null)
                return;
            var rule = _curRuleListView.selectedItem as CollectRule;
            var index = group.Rules.IndexOf(rule);
            if (index >= 0 && EditorUtility.DisplayDialog("错误", $"是否要删除{rule.name}", "确定"))
            {
                group.Rules.RemoveAt(index);
                _curRuleListView.Rebuild();
            }
        }

        void UpdateRuleListView(AssetGroup group)
        {
            _curRuleListView.Clear();
            _curRuleListView.itemsSource = group.Rules;
            //_curRuleListView.reorderable = true;
            _curRuleListView.Rebuild();
        }

        VisualElement OnMakeRuleItem()
        {
            VisualElement element = new VisualElement();
            element.style.flexDirection = FlexDirection.Column;
            {
                VisualElement space = new VisualElement();
                space.style.height = 10;
                element.Add(space);
            }
            // add name element
            VisualElement _top1 = new VisualElement();
            _top1.style.flexDirection = FlexDirection.Row;
            element.Add(_top1);
            {
                //var name = new Label("规则名");
                //name.style.minWidth = 80;
                //_top1.Add( name);
                // name field
                TextField value = new TextField("规则名");
                value.name = "RuleName";
                value.style.flexGrow = 1;
                _top1.Add(value);
                value.RegisterValueChangedCallback((t) => OnRuleNameChanged(element.tabIndex, t.newValue));

                ObjectField target = new ObjectField();
                target.name = "Target";
                target.style.flexGrow = 0;
                target.style.width = 250;
                target.objectType = typeof(UnityEngine.Object);
                target.allowSceneObjects = false;
                _top1.Add(target);
                target.RegisterValueChangedCallback((t) => OnRuleObjectChanged(element.tabIndex, t.newValue));
            }
            // target element
            VisualElement middle = new VisualElement();
            middle.style.flexDirection = FlexDirection.Row;
            element.Add(middle);
            {
                //var name = new Label("基础设置");
                //name.style.borderLeftWidth = 5;
                //name.style.unityTextAlign = TextAnchor.MiddleLeft;
                //name.style.width = 150;
                //middle.Add(name);
                var popfield = new PopupField<CollectRule.RuleBundleType>("打包方式", CollectRule.RuleBundlePopList, CollectRule.RuleBundleType.Together);
                popfield.name = "BundleField";
                middle.Add(popfield);
                popfield.RegisterValueChangedCallback((t) => OnRuleBundleTypeChanged(element.tabIndex, t.newValue));
                {
                    VisualElement space = new VisualElement();
                    space.style.width = 30;
                    middle.Add(space);
                }
                /// popup
                var depField = new PopupField<CollectRule.DependenciesType>("依赖处理", CollectRule.DepPopList, CollectRule.DependenciesType.Together);
                depField.name = "DepField";
                middle.Add(depField);
                depField.RegisterValueChangedCallback((t) => OnRuleDepChanged(element.tabIndex, t.newValue));
                /// search options
                {
                    VisualElement space = new VisualElement();
                    space.style.width = 30;
                    middle.Add(space);
                    //"搜索规则"
                    var label = new Label("是否可加载");
                    label.style.unityTextAlign = TextAnchor.MiddleLeft;
                    middle.Add(label);
                    /// load able
                    var loadable = new Toggle();
                    loadable.name = "Loadable";
                    middle.Add(loadable);
                    loadable.RegisterValueChangedCallback((t) => OnRuleLoadableChanged(element.tabIndex, t.newValue));
                }

                var move = new Button();
                move.text = "<<移动分组>>";
                move.style.minWidth = 150;
                move.clicked += () => OnRuleMoveClicked(element.tabIndex);
                middle.Add(move);
            }

            VisualElement options = new VisualElement();
            options.style.flexDirection = FlexDirection.Row;
            element.Add(options);

            var name = new Label("Filter设置");
            name.style.borderLeftWidth = 3;
            name.style.unityTextAlign = TextAnchor.MiddleLeft;
            name.style.width = 150;
            options.Add(name);

            var empty = new VisualElement();
            empty.style.flexDirection = FlexDirection.Row;
            empty.style.paddingTop = 5;
            empty.name = "SearchOptions";
            options.Add(empty);

            return element;
        }

        CollectRule GetCurrentRule(int index)
        {
            var group = _groupListView.selectedItem as AssetGroup;
            if (group == null)
                return null;
            if (index < 0 || index >= group.Rules.Count)
                return null;
            return group.Rules[index];
        }

        void OnRuleBindRuleItem(VisualElement element, int index)
        {
            element.tabIndex = index;
            // creat rule
            var rule = GetCurrentRule(index);
            if (rule == null)
                return;
            var name = element.Q<TextField>("RuleName");
            if (name != null) name.value = rule.name;
            var target = element.Q<ObjectField>("Target");
            if (target != null) target.value = rule.Target;
            var bundle = element.Q<PopupField<CollectRule.RuleBundleType>>("BundleType");
            if (bundle != null) bundle.value = rule.BundleType;
            // make item
            var search = element.Q<VisualElement>("SearchOptions");
            search.Clear(); int i = 0;
            foreach (var item in rule.Filters)
            {
                var tmp = i++;
                if( tmp >= 1)
                {
                    var btnRemove = new Button(() => OnRemoveSearchFilter(rule, tmp));
                    btnRemove.text = $"del{tmp}";
                    search.Add(btnRemove);
                }
                // popup
                var pop = new PopupField<CollectRule.FilterType>(CollectRule.FilterPopUpList, item);
                pop.RegisterValueChangedCallback((nv) => rule.Filters[tmp] = nv.newValue);
                search.Add(pop);
                {
                    var space = new VisualElement();
                    space.style.width = 10;
                    search.Add(space);
                }
            }
            // add btn
            var btnAdd = new Button(() => OnAddFilterItemClicked(rule));
            btnAdd.text = "[+]";
            search.Add(btnAdd);
            //var search = element.Q<PopupField<CollectRule.FilterType>>("SearchField");
            //if (search != null) search.value = rule.Filter;
            var dep = element.Q<PopupField<CollectRule.DependenciesType>>("DepField");
            if (dep != null) dep.value = rule.DepType;
            var loadable = element.Q<Toggle>("Loadable");
            if (loadable != null) loadable.value = rule.EnableLoad;
        }

        void OnRuleNameChanged(int index, string value)
        {
            var rule = GetCurrentRule(index);
            if (rule != null)
            {
                rule.name = value;
            }
        }

        void OnRuleObjectChanged(int index, UnityEngine.Object value)
        {
            var rule = GetCurrentRule(index);
            if (rule != null)
            {
                rule.SetTarget(value);
            }
        }
        void OnRuleBundleTypeChanged(int index, CollectRule.RuleBundleType value)
        {
            var rule = GetCurrentRule(index);
            if (rule != null)
            {
                rule.BundleType = value;
            }
        }

        //void OnRuleSearchTypeChanged( int index, CollectRule.FilterType value)
        //{
        //    var rule = GetCurrentRule(index);
        //    if (rule != null)
        //    {
        //        //rule.Filter = value;
        //    }
        //}

        void OnRuleDepChanged(int index, CollectRule.DependenciesType value)
        {
            var rule = GetCurrentRule(index);
            if (rule != null)
            {
                rule.DepType = value;
            }
        }

        void OnRuleLoadableChanged(int index, bool value)
        {
            var rule = GetCurrentRule(index);
            if (rule != null)
            {
                rule.EnableLoad = value;
            }
        }

        void OnRuleMoveClicked(int index)
        {
            var group = _groupListView.selectedItem as AssetGroup;
            if (group == null)
                return;
            if (index < 0 || index >= group.Rules.Count)
                return;
            var rule = group.Rules[index];
            MoveGroupWindow.ShowMove(assetGroup, group, rule);
            /// 刷新
            SelectGroupListItem();
        }

        void OnAddFilterItemClicked(CollectRule rule)
        {
            rule.Filters.Add(CollectRule.FilterType.ALL);
            _curRuleListView.Rebuild();
        }

        void OnRemoveSearchFilter(CollectRule rule, int index)
        {
            if (rule.Filters.Count - 1 <= 0)
            {
                EditorUtility.DisplayDialog("错误", $"每个规则最少有个过滤选项", "确定");
                return;
            }
            if(EditorUtility.DisplayDialog("错误", $"是否要删除{rule.name} 的第 {index} Filter!", "确定"))
            {
                rule.Filters.RemoveAt(index);
                _curRuleListView.Rebuild();
            }
        }

        private void OnDestroy()
        {
            AssetGroups.SaveAsset( assetGroup);
        }

        void OnTestBtnClicked()
        {
            var path = EditorUtility.SaveFilePanel("提示", "选择保存文件", "AssetGroups", "txt");
            if( !string.IsNullOrEmpty(path))
            {
                var result = AssetGroups.CollectAssets( true);
                Catlog catlog = new Catlog();
                var bundles = new List<BundleInfo>(result.Bundles.Count);
                foreach (var item in result.Bundles)
                {
                    item.Value.BundleID = bundles.Count;
                    bundles.Add(new BundleInfo()
                    {
                        hashName = "00000000.bundle",
                        name = item.Value.Name,
                        bundleID = item.Value.BundleID,
                    });
                }
                catlog.bundles = bundles.ToArray();
                var assets = new List<AssetInfo>(result.Asset2Bundle.Count);
                var depList = new List<int>(10);
                foreach (var item in result.Assets)
                {
                    var asset = new AssetInfo() { name = item.Key };
                    asset.bundle = result.GetBundle( item.Key);
                    if (result.Assets.TryGetValue(item.Key, out var tmp))
                    {
                        depList.Clear();
                        //if (tmp.SubAssets != null && tmp.SubAssets.Length > 0)
                        //{
                        //    foreach (var sub in tmp.SubAssets)
                        //    {
                        //        var bundleID = result.GetBundle( sub);
                        //        if (depList.Contains(bundleID))
                        //            continue;
                        //        depList.Add(bundleID);
                        //    }
                        //    asset.deps = depList.ToArray();
                        //}
                    }
                    assets.Add(asset);
                }
                catlog.assets = assets.ToArray();
                var output = JsonUtility.ToJson(catlog, true);
                File.WriteAllText(path, output);
            }
        }

        void OnExportClicked()
        {
            // 采用特殊后缀 避免导入或导出的时候覆盖错文件
            var path = EditorUtility.SaveFilePanel("提示", Path.GetFullPath("./"), "AssetGroups", "groups");
            if( !string.IsNullOrEmpty( path))
            {
                var content = JsonUtility.ToJson(assetGroup, true);
                File.WriteAllText(path, content);
            }
        }

        void OnImportClicked()
        {
            // 采用特殊后缀 避免导入或导出的时候覆盖错文件
            var path = EditorUtility.OpenFilePanel("提示", "选中导入文件", "groups");
            if (!string.IsNullOrEmpty(path))
            {
                var content = File.ReadAllText(path);
                JsonUtility.FromJsonOverwrite(content, assetGroup);
                assetGroup.OnImport();
                // 刷新界面
                RefreshView();
            }
        }
    }

    class MoveGroupWindow : EditorWindow
    {
        VisualElement nameParent;

        VisualElement popupParent;

        /// <summary>
        /// 提示将规则移动到某一个分组
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="curGroup"></param>
        /// <param name="rule"></param>
        public static void ShowMove( AssetGroups groups, AssetGroup curGroup, CollectRule rule)
        {
            if( groups.groups.Count <= 1)
            {
                EditorUtility.DisplayDialog("错误", $"当前只有一个分组", "确定");
                return;
            }

            var window = GetWindow<MoveGroupWindow>("移动规则");
            window.minSize = new Vector2(300, 300);
            window.maxSize = new Vector2(300, 300);
            window.InitValues(groups, curGroup, rule);
            window.ShowModal();
        }

        private void CreateGUI()
        {
            var element = new VisualElement();
            element.style.flexDirection = FlexDirection.Column;
            {
                var space = new VisualElement();
                space.style.height = 50; 
                element.Add(space);
            }
            this.rootVisualElement.Add(element);
            {
                nameParent = new VisualElement();
                nameParent.style.flexDirection = FlexDirection.Row;
                element.Add(nameParent);

                var label = new Label("原分组:");
                label.style.minWidth = 150;
                label.style.unityTextAlign = TextAnchor.MiddleLeft;
                nameParent.Add(label);
            }
            {
                var space = new VisualElement();
                space.style.height = 10;
                popupParent = new VisualElement();
                nameParent.style.flexDirection = FlexDirection.Row;
                element.Add(popupParent);
            }
            {
                var space = new VisualElement();
                space.style.height = 100;
                element.Add(space);
                var confirm = new Button(OnConfirmClicked);
                confirm.text = "确定";
                element.Add(confirm);
            }
        }

        Label GetOldName()
        {
            Label oldName = nameParent.Q<Label>("OldName");
            if (oldName != null)
                return oldName;
            oldName = new Label();
            nameParent.Add(oldName);
            oldName.style.unityTextAlign = TextAnchor.MiddleLeft;
            return oldName;
        }

        PopupField<string> GetPopup( List<string> values)
        {
            PopupField<string> ret = popupParent.Q<PopupField<string>>("PopUp");
            if ( ret != null)
                popupParent.Remove(ret);
            ret = new PopupField<string>("选择分组", values, values[0]);
            ret.name = "PopUp";
            popupParent.Add(ret);
            return ret;
        }

        List<AssetGroup> CurGroups = new List<AssetGroup>();
        AssetGroup OldGroup = null;
        CollectRule OprRule = null;
        void InitValues(AssetGroups groups, AssetGroup curGroup, CollectRule rule)
        {
            if (nameParent == null || popupParent == null)
                return;
            /// init values
            OprRule = rule;
            OldGroup = curGroup;
            GetOldName().text = curGroup.GroupName;
            /// groups
            CurGroups.Clear();
            var _list = new List<string>(groups.groups.Count - 1);
            foreach( var item in groups.groups)
            { 
                if (item == curGroup)
                    continue;
                CurGroups.Add(item);
                string tmp = string.IsNullOrEmpty(item.GroupName) ? "null" : item.GroupName;
                var name = $"{tmp}:{item.GetHashCode()}";
                _list.Add(name);
            }
            var group = GetPopup(_list);
            
        }

        public void OnConfirmClicked()
        {
            PopupField<string> selection = popupParent.Q<PopupField<string>>("PopUp");
            if( selection.index >= 0 && selection.index <= CurGroups.Count)
            {
                var newgroup = CurGroups[selection.index];
                if( newgroup != null && OldGroup != null && OprRule != null)
                {
                    OldGroup.Rules.Remove(OprRule);
                    newgroup.Rules.Add(OprRule);
                    // close self
                    Close();
                    return;
                }
                else
                {
                    EditorUtility.DisplayDialog("错误", $"选择分组错误\n{OprRule}\n{newgroup}\n{OldGroup}", "确定");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("错误", "选择分组错误", "确定");
            }
        }


        class TestSortC
        {
            public string name;
            public int number;
            public TestSortC( string n, int i)
            {
                name = n; number = i;
            }
        }

        //[MenuItem("资源管理/测试", false, 101)]
        static void TestSort()
        {
            List<TestSortC> list = new List<TestSortC>();
            list.Add(new TestSortC("test/abc", 1));
            for(int i=0; i < 10; i++)
            {
                list.Add(new TestSortC("test", 10-i));
            }
            list.Add(new TestSortC("tes", 1));
            for (int i = 0; i < 10; i++)
            {
                list.Add(new TestSortC("test", 20 - i));
            }
            list.Sort((a, b) =>
            {
                if (a.name == b.name)
                    return b.number - a.number;
                return string.Compare(b.name, a.name);
            });
            var builder = new System.Text.StringBuilder();
            foreach (var item in list)
                builder.AppendLine($"{item.name}:{item.number}");
            Debug.Log(builder);
        }
    }
}