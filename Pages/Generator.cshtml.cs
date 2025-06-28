using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SongPromptGenerator.Models;
using SongPromptGenerator.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SongPromptGenerator.Pages
{
    public class GeneratorModel : PageModel
    {
        private readonly IDataService _dataService;

        [BindProperty]
        public PromptRequest PromptRequest { get; set; }

        public IEnumerable<string> Languages { get; set; }
        public IEnumerable<string> Genres { get; set; }
        public IEnumerable<string> Themes { get; set; }
        public IEnumerable<string> LyricalStyles { get; set; }
        public IEnumerable<string> Chords { get; set; }
        public IEnumerable<string> Modes { get; set; }

        public string GeneratedPrompt { get; set; }

        public GeneratorModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task OnGetAsync()
        {
            Languages = await _dataService.GetLanguagesAsync();
            Genres = await _dataService.GetGenresAsync();
            Themes = await _dataService.GetThemesAsync();
            LyricalStyles = await _dataService.GetLyricalStylesAsync();
            Chords = await _dataService.GetChordsAsync();
            Modes = await _dataService.GetModesAsync();

            if (PromptRequest == null)
            {
                PromptRequest = new PromptRequest();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadInitialData();
                return Page();
            }

            var promptBuilder = new StringBuilder();

            promptBuilder.AppendLine("Génère une chanson complète en respectant les instructions suivantes :");
            promptBuilder.AppendLine();

            // --- Section Format ---
            promptBuilder.AppendLine("## FORMAT DE SORTIE IMPÉRATIF");
            promptBuilder.AppendLine("La chanson doit être structurée avec des marqueurs de section comme [Verse], [Chorus], [Bridge], [Intro], [Outro], [Solo].");
            promptBuilder.AppendLine("Chaque section doit être un paragraphe distinct.");
            promptBuilder.AppendLine("Au-dessus de chaque ligne de paroles, ajoute une ligne contenant les accords de guitare ou de piano synchronisés avec les syllabes des paroles.");
            promptBuilder.AppendLine("Exemple de format attendu :");
            promptBuilder.AppendLine("[Verse]");
            promptBuilder.AppendLine("C       G");
            promptBuilder.AppendLine("Voici la première ligne de texte");
            promptBuilder.AppendLine("Am      F");
            promptBuilder.AppendLine("Et en voici une deuxième");
            promptBuilder.AppendLine();

            // --- Section Contenu ---
            promptBuilder.AppendLine("## CONTENU DE LA CHANSON");
            promptBuilder.AppendLine($"- **Langue :** {PromptRequest.Language}");

            if (PromptRequest.Genres?.Count > 1)
            {
                promptBuilder.AppendLine($"- **Genres Musicaux :** {string.Join(", ", PromptRequest.Genres)}. Tu peux les fusionner de manière créative.");
            }
            else if (PromptRequest.Genres?.Count == 1)
            {
                promptBuilder.AppendLine($"- **Genre Musical :** {PromptRequest.Genres.First()}");
            }

            if (PromptRequest.Themes?.Any() == true)
            {
                promptBuilder.AppendLine($"- **Thèmes Principaux :** {string.Join(", ", PromptRequest.Themes)}");
            }
            if (PromptRequest.LyricalStyles?.Any() == true)
            {
                promptBuilder.AppendLine($"- **Styles des Paroles :** {string.Join(", ", PromptRequest.LyricalStyles)}");
            }

            if (!string.IsNullOrWhiteSpace(PromptRequest.GeneralDescription))
            {
                promptBuilder.AppendLine($"- **Description Générale :** {PromptRequest.GeneralDescription}");
            }
            promptBuilder.AppendLine();

            // --- Section Harmonie ---
            promptBuilder.AppendLine("## HARMONIE ET STRUCTURE MUSICALE");
            if (!string.IsNullOrWhiteSpace(PromptRequest.RootChord) || !string.IsNullOrWhiteSpace(PromptRequest.RootMode))
            {
                promptBuilder.AppendLine($"- **Tonalité Générale :** {(string.IsNullOrWhiteSpace(PromptRequest.RootChord) ? "(non spécifié)" : PromptRequest.RootChord)} {(string.IsNullOrWhiteSpace(PromptRequest.RootMode) ? "(non spécifié)" : PromptRequest.RootMode)}.");
            }
            if (!string.IsNullOrWhiteSpace(PromptRequest.VerseChord) || !string.IsNullOrWhiteSpace(PromptRequest.VerseMode))
            {
                promptBuilder.AppendLine($"- **Tonalité pour les Couplets (Verse) :** {(string.IsNullOrWhiteSpace(PromptRequest.VerseChord) ? "(par défaut)" : PromptRequest.VerseChord)} {(string.IsNullOrWhiteSpace(PromptRequest.VerseMode) ? "(par défaut)" : PromptRequest.VerseMode)}.");
            }
            if (!string.IsNullOrWhiteSpace(PromptRequest.ChorusChord) || !string.IsNullOrWhiteSpace(PromptRequest.ChorusMode))
            {
                promptBuilder.AppendLine($"- **Tonalité pour les Refrains (Chorus) :** {(string.IsNullOrWhiteSpace(PromptRequest.ChorusChord) ? "(par défaut)" : PromptRequest.ChorusChord)} {(string.IsNullOrWhiteSpace(PromptRequest.ChorusMode) ? "(par défaut)" : PromptRequest.ChorusMode)}.");
            }
            if (!string.IsNullOrWhiteSpace(PromptRequest.BridgeChord) || !string.IsNullOrWhiteSpace(PromptRequest.BridgeMode))
            {
                promptBuilder.AppendLine($"- **Tonalité pour le Pont (Bridge) :** {(string.IsNullOrWhiteSpace(PromptRequest.BridgeChord) ? "(par défaut)" : PromptRequest.BridgeChord)} {(string.IsNullOrWhiteSpace(PromptRequest.BridgeMode) ? "(par défaut)" : PromptRequest.BridgeMode)}.");
            }
            if (PromptRequest.UseModalMixture)
            {
                promptBuilder.AppendLine("- **Complexité Harmonique :** Utilise des techniques de mixture modale (accords empruntés à des modes parallèles) pour enrichir l'harmonie. Sois créatif.");
            }

            promptBuilder.AppendLine();
            promptBuilder.AppendLine("Lorsqu'un mode est spécifié, assurez-vous que les accords son typiques du mode.");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("Lorsque vous faites plusieurs chansons dans la même conversation, essayez que les paroles et la symbolique des parole soit originale par rapport à ce que vous aviez fait.");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("À la toute fin, génère un titre pour la chanson. Le titre ne doit pas être sous la forme de 'Le blues de...' ni 'La balade de...'.");

            GeneratedPrompt = promptBuilder.ToString();

            await LoadInitialData();

            return Page();
        }

        private async Task LoadInitialData()
        {
            Languages = await _dataService.GetLanguagesAsync();
            Genres = await _dataService.GetGenresAsync();
            Themes = await _dataService.GetThemesAsync();
            LyricalStyles = await _dataService.GetLyricalStylesAsync();
            Chords = await _dataService.GetChordsAsync();
            Modes = await _dataService.GetModesAsync();
        }
    }
}