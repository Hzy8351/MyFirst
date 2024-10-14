using System;

namespace FXFramework
{
    public enum FadeState
    {
        Close,//关闭
        FadeIn,
        FadeOut,
    }
    /// <summary>
    /// 数字渐变动画
    /// </summary>
    public class FadeNum
    {
        // 淡入状态
        private FadeState mFadeState = FadeState.Close;
        // 调用委托的时候避免提前结束
        private bool mInit = false;
        // 淡入结束后要做的事情
        private Action mOnFinish;
        // 当前值
        private float mCurrentValue;
        // 最大最小范围
        private float mMin = 0, mMax = 1;
        /// <summary>
        /// 设置范围
        /// </summary>
        public void SetMinMax(float min, float max)
        {
            mMin = min;
            mMax = max;
        }
        /// <summary>
        /// 是否启动
        /// </summary>
        public bool IsEnabled => mFadeState != FadeState.Close;
        /// <summary>
        /// 设置状态
        /// </summary>
        public void SetState(FadeState state, Action action = null)
        {
            mOnFinish = action;
            mFadeState = state;
            mInit = false;
        }
        // 当fade完成
        private void OnFinish(float value)
        {
            mOnFinish?.Invoke();
            mCurrentValue = value;
            if (!mInit) return;
            mFadeState = FadeState.Close;
        }
        /// <summary>
        /// 需要再Update持续检测
        /// </summary>
        public float Update(float step)
        {            
            switch (mFadeState)
            {
                //如果是渐入状态 0 - 1
                case FadeState.FadeIn:
                    //确认初始化参数
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
                //如果是渐出状态 1 - 0
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