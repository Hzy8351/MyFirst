using UnityEngine;

namespace FXFramework
{
    public interface IAudioMgrSystem : ISystem
    {
        void PlayBgm(string name);
        void StopBgm(bool isPause);
        void PlaySound(string name);
        AudioSource GetSound(string name);
        void RecoverySound(AudioSource source);
        void Clear();
    }
    /// <summary>
    /// 实现音频池 和 音效组件管理
    /// </summary>
    public class AudioMgrSystem : AbstractSystem, IAudioMgrSystem
    {
        // 背景音乐播放组件
        private AudioSource mBGM;
        // 临时组价 方便引用 
        private AudioSource tempSource;
        // 音量渐变工具 0 - 1
        private FadeNum mFade;
        // 资源 加载系统
        private ResPool<AudioClip> mClipPool;
        // AudioSource 组件池
        private ComponentPool<AudioSource> mSourcePool;
        // 游戏音频数据
        private IGameAudioModel mAudioModel;
        // 初始化系统
        protected override void OnInit()
        {
            mClipPool = new ResPool<AudioClip>();
            mSourcePool = new ComponentPool<AudioSource>("GameSound");

            mAudioModel = this.GetModel<IGameAudioModel>();

            mFade = new FadeNum();
            mFade.SetMinMax(0, mAudioModel.BgmVolume.Value);

            mAudioModel.BgmVolume.Register(OnBgmVolumeChanged);
            mAudioModel.SoundVolume.Register(v =>
            mSourcePool.SetAllEnabledComponent(source => source.volume = v));

            PublicMono.Instance.OnUpdate += UpdateVolume;
        }
        /// <summary>
        /// 播放音效
        /// </summary>
        void IAudioMgrSystem.PlaySound(string name)
        {
            InitSource();
            mClipPool.Get("Audio/Sound/" + name, clip =>
            {
                tempSource.clip = clip;
                tempSource.loop = false;
                tempSource.Play();
            });
        }
        /// <summary>
        /// 获取音效
        /// </summary>
        AudioSource IAudioMgrSystem.GetSound(string name)
        {
            InitSource();
            mClipPool.Get("Audio/Sound/" + name, clip =>
            {
                tempSource.clip = clip;
                tempSource.loop = true;
            });
            return tempSource;
        }
        /// <summary>
        /// 用于停止循环音效
        /// </summary>
        void IAudioMgrSystem.RecoverySound(AudioSource source) => mSourcePool.Push(source, source.Stop);

        /// <summary>
        /// 停止Bgm
        /// </summary>
        void IAudioMgrSystem.StopBgm(bool isPause)
        {
            if (mBGM == null || !mBGM.isPlaying) return;

            mFade.SetState(FadeState.FadeOut, () =>
            {
                if (isPause) mBGM.Pause();
                else mBGM.Stop();
            });
        }
        void IAudioMgrSystem.Clear()
        {
            mClipPool.Clear();
        }
        /// <summary>
        /// 播放BGM
        /// </summary>
        void IAudioMgrSystem.PlayBgm(string name)
        {
            if (mBGM == null)
            {
                var o = new GameObject("GameBGM");
                GameObject.DontDestroyOnLoad(o);
                mBGM = o.AddComponent<AudioSource>();
                mBGM.loop = true;
                mBGM.volume = 0;
            }
            mClipPool.Get("Audio/Bgm/" + name, clip =>
            {
                //如果有bgm正在播放 就先把音量降下来 1 - 0 再播放当前音乐 0 - 1 否则直接播放
                if (!mBGM.isPlaying) PlayBgm(clip);
                else mFade.SetState(FadeState.FadeOut, () => PlayBgm(clip));
            });
        }
        private void PlayBgm(AudioClip clip)
        {
            mBGM.clip = clip;
            mFade.SetState(FadeState.FadeIn);
            mBGM.Play();
        }
        // 当背景音乐音量改变时
        private void OnBgmVolumeChanged(float v)
        {
            if (mBGM == null) return;
            mFade.SetMinMax(0, v);
            mBGM.volume = v;
        }
        // 更新音量
        private void UpdateVolume()
        {
            if (!mFade.IsEnabled) return;
            mBGM.volume = mFade.Update(Time.deltaTime);
        }
        // 初始化组件
        public void InitSource()
        {
            mSourcePool.AutoPush(source => !source.isPlaying);
            mSourcePool.Get(out tempSource);
            tempSource.volume = mAudioModel.SoundVolume.Value;
        }
    }
}