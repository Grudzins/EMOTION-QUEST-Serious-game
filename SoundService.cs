using System;
using System.IO;
using System.Windows.Media;

namespace EmotionQuest.Services
{
    public class SoundService
    {
        public bool Enabled { get; set; } = true;

        private readonly MediaPlayer _player = new MediaPlayer();
        private readonly Uri _correctUri;
        private readonly Uri _wrongUri;

        public SoundService()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            string correctPath = Path.Combine(baseDir, "correct.wav");
            string wrongPath = Path.Combine(baseDir, "wrong.wav");

            _correctUri = File.Exists(correctPath) ? new Uri(correctPath, UriKind.Absolute) : null;
            _wrongUri = File.Exists(wrongPath) ? new Uri(wrongPath, UriKind.Absolute) : null;

            _player.Volume = 1.0;
        }

        public void PlayCorrect() => Play(_correctUri);
        public void PlayWrong() => Play(_wrongUri);

        private void Play(Uri uri)
        {
            if (!Enabled || uri == null) return;

            try
            {
                _player.Stop();
                _player.Open(uri);
                _player.Position = TimeSpan.Zero;
                _player.Play();
            }
            catch
            {
               
            }
        }
    }
}
