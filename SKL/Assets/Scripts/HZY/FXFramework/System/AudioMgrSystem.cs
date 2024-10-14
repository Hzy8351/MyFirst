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
    /// ʵ����Ƶ�� �� ��Ч�������
    /// </summary>
    public class AudioMgrSystem : AbstractSystem, IAudioMgrSystem
    {
        // �������ֲ������
        private AudioSource mBGM;
        // ��ʱ��� �������� 
        private AudioSource tempSource;
        // �������乤�� 0 - 1
        private FadeNum mFade;
        // ��Դ ����ϵͳ
        private ResPool<AudioClip> mClipPool;
        // AudioSource �����
        private ComponentPool<AudioSource> mSourcePool;
        // ��Ϸ��Ƶ����
        private IGameAudioModel mAudioModel;
        // ��ʼ��ϵͳ
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
        /// ������Ч
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
        /// ��ȡ��Ч
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
        /// ����ֹͣѭ����Ч
        /// </summary>
        void IAudioMgrSystem.RecoverySound(AudioSource source) => mSourcePool.Push(source, source.Stop);

        /// <summary>
        /// ֹͣBgm
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
        /// ����BGM
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
                //�����bgm���ڲ��� ���Ȱ����������� 1 - 0 �ٲ��ŵ�ǰ���� 0 - 1 ����ֱ�Ӳ���
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
        // ���������������ı�ʱ
        private void OnBgmVolumeChanged(float v)
        {
            if (mBGM == null) return;
            mFade.SetMinMax(0, v);
            mBGM.volume = v;
        }
        // ��������
        private void UpdateVolume()
        {
            if (!mFade.IsEnabled) return;
            mBGM.volume = mFade.Update(Time.deltaTime);
        }
        // ��ʼ�����
        public void InitSource()
        {
            mSourcePool.AutoPush(source => !source.isPlaying);
            mSourcePool.Get(out tempSource);
            tempSource.volume = mAudioModel.SoundVolume.Value;
        }
    }
}