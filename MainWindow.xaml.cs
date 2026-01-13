using System;
using System.Windows;
using EmotionQuest.Logic;
using System.Windows.Threading;
using EmotionQuest.Services;

namespace EmotionQuest
{
    public partial class MainWindow : Window
    {
        private readonly GameController _controller;
        private DateTime _roundStart;

        public MainWindow()
        {
            InitializeComponent();
            _sound = new SoundService();
            _sound.Enabled = true; 
            _feedbackTimer = new DispatcherTimer(DispatcherPriority.Render);
            _feedbackTimer.Interval = TimeSpan.FromMilliseconds(900);
            _feedbackTimer.Tick += FeedbackTimer_Tick;
            _controller = new GameController();
            UpdateUI();
            HookEvents();

        }

        private void HookEvents()
        {
            EmotionJoyButton.Click += (s, e) => HandleEmotionClick("Radość");
            EmotionSadnessButton.Click += (s, e) => HandleEmotionClick("Smutek");
            EmotionAngerButton.Click += (s, e) => HandleEmotionClick("Złość");
            EmotionFearButton.Click += (s, e) => HandleEmotionClick("Strach");
            EmotionCalmButton.Click += (s, e) => HandleEmotionClick("Spokój");
            PlayAgainButton.Click += (s, e) => RestartGame();
            SettingsButton.Click += (s, e) => ToggleSettingsOverlay();
            CloseSettingsButton.Click += (s, e) => HideSettingsOverlay();
            SaveButton.Click += (s, e) => ManualSave();
            RestartButton.Click += (s, e) => RestartGame();
            SoundEnabledCheckBox.Checked += (s, e) => _sound.Enabled = true;
            SoundEnabledCheckBox.Unchecked += (s, e) => _sound.Enabled = false;
        }

        private void HandleEmotionClick(string emotion)
        {
            if (_controller.State.IsFinished)
                return;

            int rtMs = (int)(DateTime.Now - _roundStart).TotalMilliseconds;

            bool correct = _controller.ProcessUserChoice(emotion, rtMs);
            ShowFeedback(correct);
            if (correct) _sound.PlayCorrect();
            else _sound.PlayWrong();

            if (_controller.State.IsFinished)
            {
                ShowEndOverlay();
                return;
            }

            _controller.NextRound();   
            UpdateUI();
        }

        private void UpdateUI()
        {
            StimulusText.Text = _controller.CurrentStimulus.Description;
            ScoreText.Text = _controller.State.Score.ToString();
            LevelText.Text = _controller.State.Difficulty;

            _roundStart = DateTime.Now;
        }
        private void RestartGame()
        {
            
            FeedbackOverlay.Visibility = Visibility.Collapsed;
            SettingsOverlay.Visibility = Visibility.Collapsed;
            EndOverlay.Visibility = Visibility.Collapsed;

            _controller.ResetGame();
            UpdateUI();
        }
        private void ShowEndOverlay()
        {
            double acc = _controller.State.Accuracy * 100.0;

            FinalStatsText.Text =
                "Wynik: " + _controller.State.Score +
                " | Trafność: " + acc.ToString("0") + "%" +
                " | Rund: " + _controller.State.RoundsPlayed + "/" + EmotionQuest.Models.GameState.MaxRounds;

            EndOverlay.Visibility = Visibility.Visible;
        }
        
        private DispatcherTimer _feedbackTimer;
        private void FeedbackTimer_Tick(object sender, EventArgs e)
        {
            _feedbackTimer.Stop();
            FeedbackOverlay.Visibility = Visibility.Collapsed;

            
            Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.Render);
        }
        private void ShowFeedback(bool correct)
        {
            if (correct)
            {
                FeedbackTitle.Text = "✔ Poprawnie!";
                FeedbackDetails.Text = "Dobra interpretacja emocji.";
            }
            else
            {
                FeedbackTitle.Text = "✖ Niepoprawnie";
                FeedbackDetails.Text = "Poprawna emocja: " + _controller.CurrentStimulus.CorrectEmotion;
            }

            FeedbackOverlay.Visibility = Visibility.Visible;

            
            _feedbackTimer.Stop();
            _feedbackTimer.Start();
        }
        private void ToggleSettingsOverlay()
        {
            if (SettingsOverlay.Visibility == Visibility.Visible)
                SettingsOverlay.Visibility = Visibility.Collapsed;
            else
                SettingsOverlay.Visibility = Visibility.Visible;
        }

        private void HideSettingsOverlay()
        {
            SettingsOverlay.Visibility = Visibility.Collapsed;
        }
        private void ManualSave()
        {
            
            _controller.Storage.SaveProgress(_controller.State);

           
            FeedbackTitle.Text = "💾 Zapisano";
            FeedbackDetails.Text = "Postęp zapisany do progress.json";
            FeedbackOverlay.Visibility = Visibility.Visible;

            _feedbackTimer.Stop();
            _feedbackTimer.Start();
        }
        
        private SoundService _sound;
    }
}
