using System;
using System.Collections.Generic;
using EmotionQuest.Models;
using EmotionQuest.Services;
using System.IO;
using Newtonsoft.Json;

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

            _stimuli = LoadStimuliFromJson() ?? LoadDemoStimuli();
            ShuffleDeck();
            NextRound();
        }

        private List<Stimulus> LoadDemoStimuli()
        {
            // 
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
            if (State.IsFinished)
                return;

            if (_deck == null || _deck.Count == 0)
                return;

           
            if (_deckIndex >= _deck.Count)
            {
                ShuffleDeck();
            }

            CurrentStimulus = _deck[_deckIndex];
            _deckIndex++;
        }

        
        public bool ProcessUserChoice(string emotion, int reactionTimeMs)
        {
            if (State.IsFinished)
                return false;

            bool correct = Mechanics.Evaluate(CurrentStimulus, emotion);
            State.Update(correct);

            Storage.SaveProgress(State);
            Storage.AppendMetrics(CurrentStimulus, emotion, correct, reactionTimeMs, State.Difficulty);

            Mechanics.AdjustDifficulty(State);

            return correct;
        }

        public void ResetGame()
        {
            State.Reset();
            ShuffleDeck();
            NextRound();
            Storage.SaveProgress(State);
        }
        private List<Stimulus> LoadStimuliFromJson()
        {
            try
            {
                
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "stimuli.json");

                if (!File.Exists(path))
                    return null;

                string json = File.ReadAllText(path);
                var list = JsonConvert.DeserializeObject<List<Stimulus>>(json);

                if (list == null || list.Count == 0)
                    return null;

                return list;
            }
            catch
            {
               
                return null;
            }
        }
        
        private List<Stimulus> _deck;
        private int _deckIndex;
        private void ShuffleDeck()
        {
            _deck = new List<Stimulus>(_stimuli);

           
            for (int i = _deck.Count - 1; i > 0; i--)
            {
                int j = _rnd.Next(i + 1);
                var temp = _deck[i];
                _deck[i] = _deck[j];
                _deck[j] = temp;
            }

            _deckIndex = 0;
        }
    }
}
