using System.Collections.Generic;

namespace SongPromptGenerator.Models
{
    public class PromptRequest
    {
        // Section principale
        public string Language { get; set; }

        // Sélection multiple
        public List<string> Genres { get; set; } = new List<string>();
        public List<string> Themes { get; set; } = new List<string>();
        public List<string> LyricalStyles { get; set; } = new List<string>();

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