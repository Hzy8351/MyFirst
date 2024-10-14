using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class MessageBox : MonoBehaviour
    {
        public static float Alpha = 0.1f;
        public static float AnimeTime = 0.1f;

        [SerializeField]
        CanvasGroup canvasGroup = null;

        [SerializeField]
        Button btn_cancel;

        [SerializeField]
        Button btn_confirm;

        [SerializeField]
        protected Text txt_content;
        public Text Content { get => txt_content; }


        [SerializeField]
        protected Text txt_title;
        public Text Title { get => txt_title; }

        public System.Action<bool> onClicked;

        RectTransform _rectTransform;
        RectTransform rectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = canvasGroup?.GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        /// 设置关闭按钮回调索引
        public int CloseLikeSelection = -1;

        private void Awake()
        {
            if (canvasGroup == null)
                canvasGroup = this.GetComponentInChildren<CanvasGroup>();
            // callback
            btn_cancel?.onClick.AddListener(() => this.DismissWith(false));
            btn_confirm?.onClick.AddListener(() => this.DismissWith(true));
        }

        private void Start()
        {
            //canvasGroup?.DOAlpha(1, AnimeTime);
        }

        private void DismissWith(bool value)
        {
            onClicked?.Invoke(value);
            onClicked = null;
            /// dismiss animation
            Destroy(gameObject);
        }

        public void SetContent(string title, string content, string confirm, string cancel, System.Action<bool> result)
        {
            if (Title != null)
                Title.text = string.IsNullOrEmpty(title) ? "" : title;

            if (Content != null)
                Content.text = string.IsNullOrEmpty(content) ? "" : content;

            onClicked = result;

            // 如果没有cancel 那么只显示一个
            if (string.IsNullOrEmpty(cancel))
            {
                btn_cancel?.gameObject.SetActive(false);
                btn_confirm?.gameObject.SetActive(true);
                // set content
                SetBtnTxt(btn_confirm, confirm);
            }
            else
            {
                btn_cancel?.gameObject.SetActive(true);
                btn_confirm?.gameObject.SetActive(true);
                // set content
                SetBtnTxt(btn_cancel, cancel);
                // set content
                SetBtnTxt(btn_confirm, confirm);
            }
        }

        private void SetBtnTxt(Button button, string title)
        {
            if (button)
                button.transform.SetAsLastSibling();

            var txt = button?.GetComponentInChildren<Text>();
            if (txt)
                txt.text = string.IsNullOrEmpty(title) ? "null" : title;
        }


        static private GameObject default_prefab;

        static GameObject getDefaultPrefab()
        {
            if (default_prefab == null)
                default_prefab = Resources.Load<GameObject>("Messagebox/MessageBox");
            return default_prefab;
        }

        public static MessageBox Show(string title, string content, string confirm, System.Action<bool> clicked = null)
        {
            return ShowWith(title, content, confirm, null, clicked);
        }

        public static MessageBox ShowWith(string title, string content, string confirm, string cancel, System.Action<bool> clicked = null)
        {
            GameObject prefab = getDefaultPrefab();
            //Canvas parent = ObjectEx.GetRootCanvas();
            if (prefab != null && Assets.ApplicationEx.isQuiting == false)
            {
                var obj = GameObject.Instantiate(prefab);
                var box = obj.GetComponent<MessageBox>();
                box.SetContent(title, content, confirm, cancel, clicked);
                return box;
            }
            return null;
        }

        static public void Dismiss()
        {
            var childs = GameObject.FindObjectsOfType<MessageBox>();
            foreach (var messagebox in childs)
                Destroy(messagebox.gameObject);
        }
    }
}