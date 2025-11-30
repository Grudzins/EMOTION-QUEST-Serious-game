namespace EmotionQuest.Models
{
    public class Stimulus
    {
        public string Id { get; set; }
        public string Description { get; set; }       // opis sytuacji
        public string CorrectEmotion { get; set; }    // np. "Radość", "Smutek"
    }
}
