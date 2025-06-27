namespace SongPromptGenerator.Models
{
    public class PromptRequest
    {
        // Section principale
        public string Language { get; set; }
        public string Genre { get; set; }
        public string Theme { get; set; }
        public string LyricalStyle { get; set; }
        public string GeneralDescription { get; set; }

        // Section Harmonie
        public string RootChord { get; set; }
        public string RootMode { get; set; }
        public string VerseChord { get; set; }
        public string VerseMode { get; set; }
        public string ChorusChord { get; set; }
        public string ChorusMode { get; set; }
        public string BridgeChord { get; set; }
        public string BridgeMode { get; set; }
        public bool UseModalMixture { get; set; }
    }
}