using System;

namespace EmotionQuest.Models
{
    public class GameState
    {
        public int Score { get; private set; }
        public int RoundsPlayed { get; private set; }
        public int CorrectAnswers { get; private set; }
        public string Difficulty { get; private set; } = "Łatwy";

        public double Accuracy => RoundsPlayed == 0
            ? 0
            : (double)CorrectAnswers / RoundsPlayed;

        public void Update(bool correct)
        {
            RoundsPlayed++;

            if (correct)
            {
                CorrectAnswers++;
                Score += 5;
            }
            else
            {
                Score -= 2;
            }
        }

        public void SetDifficulty(string difficulty)
        {
            Difficulty = difficulty;
        }
        
        public const int MaxRounds = 10;

        public bool IsFinished
        {
            get { return RoundsPlayed >= MaxRounds; }
        }

        public void Reset()
        {
            Score = 0;
            RoundsPlayed = 0;
            CorrectAnswers = 0;
            Difficulty = "Łatwy";
        }
    }
}
