using EmotionQuest.Models;

namespace EmotionQuest.Logic
{
    public class Mechanics
    {
        public bool Evaluate(Stimulus stimulus, string userChoice)
        {
            return string.Equals(
                stimulus.CorrectEmotion,
                userChoice,
                System.StringComparison.OrdinalIgnoreCase);
        }

        
        public void AdjustDifficulty(GameState state)
        {
            if (state.RoundsPlayed < 5)
                return; 

            if (state.Accuracy >= 0.8)
                state.SetDifficulty("Średni");
            else if (state.Accuracy <= 0.5)
                state.SetDifficulty("Łatwy");
            else
                state.SetDifficulty("Średni");
        }
    }
}
