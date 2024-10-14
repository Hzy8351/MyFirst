using UnityEngine;

namespace FXFramework
{
    public interface IGameAudioModel : IModel
    {
        BindableProperty<float> BgmVolume { get; }
        BindableProperty<float> SoundVolume { get; }
    }

    public class GameAudioModel : AbstractModel, IGameAudioModel
    {
        public BindableProperty<float> BgmVolume { get; } = new BindableProperty<float>();
        public BindableProperty<float> SoundVolume { get; } = new BindableProperty<float>();

        protected override void OnInit()
        {
            // 可以在初始化中进行存储读取相关内容
            BgmVolume.Value = PlayerPrefs.GetFloat(nameof(BgmVolume),0.5f);
            SoundVolume.Value = PlayerPrefs.GetFloat(nameof(SoundVolume),1f);

            BgmVolume.Register(value => PlayerPrefs.SetFloat(nameof(BgmVolume), value));
            SoundVolume.Register(value => PlayerPrefs.SetFloat(nameof(SoundVolume), value));
        }
    }
}