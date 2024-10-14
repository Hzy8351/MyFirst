using System;

namespace FXFramework
{
    // 让公共mono脚本继承单例基类 将泛型指定为当前脚本 
    public class PublicMono : MonoSingle<PublicMono>
    {
        public event Action OnUpdate;
        public event Action OnLateUpdate;
        public event Action OnFixedUpdate;

        private void Update() => OnUpdate?.Invoke();
        private void LateUpdate() => OnLateUpdate?.Invoke();
        private void FixedUpdate() => OnFixedUpdate?.Invoke();
    }
}