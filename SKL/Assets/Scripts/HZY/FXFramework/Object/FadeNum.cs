using System;

namespace FXFramework
{
    public enum FadeState
    {
        Close,//�ر�
        FadeIn,
        FadeOut,
    }
    /// <summary>
    /// ���ֽ��䶯��
    /// </summary>
    public class FadeNum
    {
        // ����״̬
        private FadeState mFadeState = FadeState.Close;
        // ����ί�е�ʱ�������ǰ����
        private bool mInit = false;
        // ���������Ҫ��������
        private Action mOnFinish;
        // ��ǰֵ
        private float mCurrentValue;
        // �����С��Χ
        private float mMin = 0, mMax = 1;
        /// <summary>
        /// ���÷�Χ
        /// </summary>
        public void SetMinMax(float min, float max)
        {
            mMin = min;
            mMax = max;
        }
        /// <summary>
        /// �Ƿ�����
        /// </summary>
        public bool IsEnabled => mFadeState != FadeState.Close;
        /// <summary>
        /// ����״̬
        /// </summary>
        public void SetState(FadeState state, Action action = null)
        {
            mOnFinish = action;
            mFadeState = state;
            mInit = false;
        }
        // ��fade���
        private void OnFinish(float value)
        {
            mOnFinish?.Invoke();
            mCurrentValue = value;
            if (!mInit) return;
            mFadeState = FadeState.Close;
        }
        /// <summary>
        /// ��Ҫ��Update�������
        /// </summary>
        public float Update(float step)
        {            
            switch (mFadeState)
            {
                //����ǽ���״̬ 0 - 1
                case FadeState.FadeIn:
                    //ȷ�ϳ�ʼ������
                    if (!mInit)
                    {
                        mCurrentValue = mMin;
                        mInit = true;
                    }
                    if (mCurrentValue < mMax)
                    {
                        mCurrentValue += step;
                    }
                    else OnFinish(mMax);
                    break;
                //����ǽ���״̬ 1 - 0
                case FadeState.FadeOut:
                    if (!mInit)
                    {
                        mCurrentValue = mMax;
                        mInit = true;
                    }
                    if (mCurrentValue > mMin)
                    {
                        mCurrentValue -= step;
                    }
                    else OnFinish(mMin);
                    break;
            }
            return mCurrentValue;
        }
    }
}