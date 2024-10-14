using System;

namespace FXFramework
{
    // �ù���mono�ű��̳е������� ������ָ��Ϊ��ǰ�ű� 
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