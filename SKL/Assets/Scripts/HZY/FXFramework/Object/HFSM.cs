using System;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    /// <summary>
    /// 状态生命周期
    /// </summary>
    public enum E_StateLife { Enter, Update, Exit }
    /// <summary>
    /// 状态基类
    /// </summary>
    public abstract class State<T> where T : Enum
    {
        private State<T> mFatherState;
        public State<T> SetFather(State<T> father)
        {
            mFatherState = father;
            return this;
        }
        public bool Check(out string nextStateName)
        {
            if (mFatherState != null &&
                mFatherState.Check(out nextStateName)) return true;
            nextStateName = CheckCondition();
            return !string.IsNullOrEmpty(nextStateName);
        }
        // 可以传入对应枚举 调用 Trigger 的对应方法
        public void Trigger(T life)
        {
            // 这里需要先判断父类状态是否存在，如果存在就执行父类状态行为
            mFatherState?.Trigger(life);
            // 执行当前状态 这里代码上一期漏掉了
            Execute(life);
        }
        protected abstract void Execute(T life);
        protected abstract string CheckCondition();
    }
    public interface ICanPlayAnim
    {
        void PlayAnim(int animHash);
    }
    /// <summary>
    /// 动画状态
    /// </summary>
    public abstract class AnimState<T, K> : BaseState<T, K> where T : MonoBehaviour, ICanPlayAnim where K : ScriptableObject
    {
        private int mAnimHash;
        public AnimState<T, K> SetAnim(string animName)
        {
            mAnimHash = Animator.StringToHash(animName);
            return this;
        }
        protected override void Execute(E_StateLife life)
        {
            if (life != E_StateLife.Enter) return;
            Machine.PlayAnim(mAnimHash);
        }
    }
    /// <summary>
    /// 带参数的状态 基类
    /// </summary>
    public abstract class BaseState<T, K> : State<E_StateLife> where T : MonoBehaviour where K : ScriptableObject
    {
        protected T Machine { get; private set; }
        protected K Data { get; private set; }
        /// <summary>
        /// 初始化状态需要的数据
        /// </summary>
        /// <param name="machine">状态机交互提供者 一般为Mono</param>
        /// <param name="data">状态机的状态数据 一般使用SO</param>
        public BaseState<T, K> Init(T machine, K data)
        {
            Machine = machine;
            Data = data;
            // Debug.Log("初始化" + this.GetType().Name + (Data as TestAIData).PursuitCheckOffset);
            return this;
        }
    }
    /// <summary>
    /// 状态构建器
    /// </summary>
    public interface IStateBuilder<T> where T : Enum
    {
        Dictionary<string, State<T>> Create(out string firstStateName);
    }
    /// <summary>
    /// 如果无需定制自己的状态机 直接继承规则状态机即可 
    /// </summary>
    public abstract class RuleFSM<T, K> : HFSM<E_StateLife> where T : ScriptableObject where K : MonoBehaviour
    {
        private T mData;
        protected T Data 
        {
            get
            {
                if (mData == null)
                {
                    mData = Resources.Load<T>(DataResPath);
                }
                return mData;
            }
        }
        public abstract string DataResPath { get; }
        protected override E_StateLife FirstLife => E_StateLife.Enter;
        protected override void SwitchState(State<E_StateLife> nextState)
        {
            mCurState.Trigger(E_StateLife.Exit);
            Life = E_StateLife.Enter;
            mCurState = nextState;
            // Debug.Log(mCurState.GetType().Name);
        }
        protected override void SwitchLifeByRule()
        {
            // 这里为了确保 Enter 方法只执行一次 当执行完 Enter 立刻转换到 Update
            if (Life == E_StateLife.Enter) Life = E_StateLife.Update;
        }
        protected override void Awake()
        {
            base.Awake();
            InitAllState(state => (state as BaseState<K, T>).Init(this as K, Data));
        }
    }
    /// <summary>
    /// 分层有限状态机
    /// </summary>
    /// <typeparam name="T">生命周期</typeparam>
    public abstract class HFSM<T> : MonoBehaviour where T : Enum
    {
        // 状态生命周期 需配合规则使用
        protected T Life;
        // 状态集合
        private Dictionary<string, State<T>> mStates;
        // 保护的当前状态方便子类访问
        protected State<T> mCurState;
        // 构建器 需要适配 泛型
        protected abstract IStateBuilder<T> Builder { get; }
        protected virtual void Awake()
        {
            mStates = Builder.Create(out string defaultStateName);
            mCurState = mStates[defaultStateName];
            Life = FirstLife;
        }
        // 子类重写 首次执行的生命周期
        protected abstract T FirstLife { get; }
        // 对外提供状态切换方法
        protected void SwitchState(string nextStateName)
        {
            if (!mStates.TryGetValue(nextStateName, out var nextState))
                throw new Exception("状态不存在,请检查状态输入是否正确！");
            SwitchState(nextState);
        }
        // 初始化所有状态
        protected void InitAllState(Action<State<T>> callback)
        {
            if (mStates == null || mStates.Count == 0) return;
            foreach (var state in mStates.Values) callback(state);
        }
        // 转换状态
        protected abstract void SwitchState(State<T> nextState);
        // 转换规则
        protected abstract void SwitchLifeByRule();
        // 更新当前状态
        protected virtual void Update()
        {
            if (mCurState == null) return;
            Debug.Log(mCurState.GetType());
            // 执行当前所有状态动作
            mCurState.Trigger(Life);
            // 转换状态生命周期
            SwitchLifeByRule();
            // 对下一个状态进行检测 如果存在可转换的状态 就执行状态转换
            if (!mCurState.Check(out string nextStateName)) return;
            SwitchState(nextStateName);
        }
    }
}