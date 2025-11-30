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

        // Proste DDA – wystarczy na Etap 3
        public void AdjustDifficulty(GameState state)
        {
            if (state.RoundsPlayed < 5)
                return; // za mało danych

            if (state.Accuracy >= 0.8)
                state.SetDifficulty("Średni");
            else if (state.Accuracy <= 0.5)
                state.SetDifficulty("Łatwy");
            else
                state.SetDifficulty("Średni");
        }
    }
}//Here will be the code
