using System;
using System.Collections.Generic;
using EmotionQuest.Models;
using EmotionQuest.Services;

namespace EmotionQuest.Logic
{
    public class GameController
    {
        private readonly List<Stimulus> _stimuli;
        private readonly Random _rnd = new Random();

        public GameState State { get; }
        public Mechanics Mechanics { get; }
        public StorageService Storage { get; }

        public Stimulus CurrentStimulus { get; private set; }

        public GameController()
        {
            State = new GameState();
            Mechanics = new Mechanics();
            Storage = new StorageService();

            _stimuli = LoadDemoStimuli();
            NextRound();
        }

        private List<Stimulus> LoadDemoStimuli()
        {
            // Na Etap 3 wystarczy hard-code. Później możesz wczytać z stimuli.json.
            return new List<Stimulus>
            {
                new Stimulus
                {
                    Id = "1",
                    Description = "Kolega niespodziewanie kupił Ci kawę dokładnie taką, jak lubisz.",
                    CorrectEmotion = "Radość"
                },
                new Stimulus
                {
                    Id = "2",
                    Description = "Szef neutralnym tonem mówi: 'Porozmawiajmy jutro rano o projekcie'.",
                    CorrectEmotion = "Spokój"
                },
                new Stimulus
                {
                    Id = "3",
                    Description = "Twój ważny plik zniknął z dysku tuż przed deadlinem.",
                    CorrectEmotion = "Strach"
                }
            };
        }

        public void NextRound()
        {
            if (_stimuli.Count == 0) return;

            CurrentStimulus = _stimuli[_rnd.Next(_stimuli.Count)];
        }

        // !!! Ta metoda jest na Slajdzie 6 (fragment kodu)
        public bool ProcessUserChoice(string emotion, int reactionTimeMs)
        {
            // 1. Ocena odpowiedzi
            bool correct = Mechanics.Evaluate(CurrentStimulus, emotion);

            // 2. Aktualizacja stanu gry
            State.Update(correct);

            // 3. Zapis postępu (JSON)
            Storage.SaveProgress(State);

            // 4. Zapis metryk rundy (CSV)
            Storage.AppendMetrics(CurrentStimulus, emotion, correct, reactionTimeMs, State.Difficulty);

            // 5. Dostosowanie trudności (DDA)
            Mechanics.AdjustDifficulty(State);

            return correct;
        }
    }
}
