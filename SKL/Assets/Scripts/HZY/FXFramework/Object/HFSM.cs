using System;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    /// <summary>
    /// ״̬��������
    /// </summary>
    public enum E_StateLife { Enter, Update, Exit }
    /// <summary>
    /// ״̬����
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
        // ���Դ����Ӧö�� ���� Trigger �Ķ�Ӧ����
        public void Trigger(T life)
        {
            // ������Ҫ���жϸ���״̬�Ƿ���ڣ�������ھ�ִ�и���״̬��Ϊ
            mFatherState?.Trigger(life);
            // ִ�е�ǰ״̬ ���������һ��©����
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
    /// ����״̬
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
    /// ��������״̬ ����
    /// </summary>
    public abstract class BaseState<T, K> : State<E_StateLife> where T : MonoBehaviour where K : ScriptableObject
    {
        protected T Machine { get; private set; }
        protected K Data { get; private set; }
        /// <summary>
        /// ��ʼ��״̬��Ҫ������
        /// </summary>
        /// <param name="machine">״̬�������ṩ�� һ��ΪMono</param>
        /// <param name="data">״̬����״̬���� һ��ʹ��SO</param>
        public BaseState<T, K> Init(T machine, K data)
        {
            Machine = machine;
            Data = data;
            // Debug.Log("��ʼ��" + this.GetType().Name + (Data as TestAIData).PursuitCheckOffset);
            return this;
        }
    }
    /// <summary>
    /// ״̬������
    /// </summary>
    public interface IStateBuilder<T> where T : Enum
    {
        Dictionary<string, State<T>> Create(out string firstStateName);
    }
    /// <summary>
    /// ������趨���Լ���״̬�� ֱ�Ӽ̳й���״̬������ 
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
            // ����Ϊ��ȷ�� Enter ����ִֻ��һ�� ��ִ���� Enter ����ת���� Update
            if (Life == E_StateLife.Enter) Life = E_StateLife.Update;
        }
        protected override void Awake()
        {
            base.Awake();
            InitAllState(state => (state as BaseState<K, T>).Init(this as K, Data));
        }
    }
    /// <summary>
    /// �ֲ�����״̬��
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    public abstract class HFSM<T> : MonoBehaviour where T : Enum
    {
        // ״̬�������� ����Ϲ���ʹ��
        protected T Life;
        // ״̬����
        private Dictionary<string, State<T>> mStates;
        // �����ĵ�ǰ״̬�����������
        protected State<T> mCurState;
        // ������ ��Ҫ���� ����
        protected abstract IStateBuilder<T> Builder { get; }
        protected virtual void Awake()
        {
            mStates = Builder.Create(out string defaultStateName);
            mCurState = mStates[defaultStateName];
            Life = FirstLife;
        }
        // ������д �״�ִ�е���������
        protected abstract T FirstLife { get; }
        // �����ṩ״̬�л�����
        protected void SwitchState(string nextStateName)
        {
            if (!mStates.TryGetValue(nextStateName, out var nextState))
                throw new Exception("״̬������,����״̬�����Ƿ���ȷ��");
            SwitchState(nextState);
        }
        // ��ʼ������״̬
        protected void InitAllState(Action<State<T>> callback)
        {
            if (mStates == null || mStates.Count == 0) return;
            foreach (var state in mStates.Values) callback(state);
        }
        // ת��״̬
        protected abstract void SwitchState(State<T> nextState);
        // ת������
        protected abstract void SwitchLifeByRule();
        // ���µ�ǰ״̬
        protected virtual void Update()
        {
            if (mCurState == null) return;
            Debug.Log(mCurState.GetType());
            // ִ�е�ǰ����״̬����
            mCurState.Trigger(Life);
            // ת��״̬��������
            SwitchLifeByRule();
            // ����һ��״̬���м�� ������ڿ�ת����״̬ ��ִ��״̬ת��
            if (!mCurState.Check(out string nextStateName)) return;
            SwitchState(nextStateName);
        }
    }
}