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

            promptBuilder.AppendLine("G�n�re une chanson compl�te en respectant les instructions suivantes :");
            promptBuilder.AppendLine();

            // --- Section Format ---
            promptBuilder.AppendLine("## FORMAT DE SORTIE IMP�RATIF");
            promptBuilder.AppendLine("La chanson doit �tre structur�e avec des marqueurs de section comme [Verse], [Chorus], [Bridge], [Intro], [Outro], [Solo].");
            promptBuilder.AppendLine("Chaque section doit �tre un paragraphe distinct.");
            promptBuilder.AppendLine("Au-dessus de chaque ligne de paroles, ajoute une ligne contenant les accords de guitare ou de piano synchronis�s avec les syllabes des paroles.");
            promptBuilder.AppendLine("Exemple de format attendu :");
            promptBuilder.AppendLine("[Verse]");
            promptBuilder.AppendLine("C       G");
            promptBuilder.AppendLine("Voici la premi�re ligne de texte");
            promptBuilder.AppendLine("Am      F");
            promptBuilder.AppendLine("Et en voici une deuxi�me");
            promptBuilder.AppendLine();

            // --- Section Contenu ---
            promptBuilder.AppendLine("## CONTENU DE LA CHANSON");
            promptBuilder.AppendLine($"- **Langue :** {PromptRequest.Language}");

            if (PromptRequest.Genres?.Count > 1)
            {
                promptBuilder.AppendLine($"- **Genres Musicaux :** {string.Join(", ", PromptRequest.Genres)}. Tu peux les fusionner de mani�re cr�ative.");
            }
            else if (PromptRequest.Genres?.Count == 1)
            {
                promptBuilder.AppendLine($"- **Genre Musical :** {PromptRequest.Genres.First()}");
            }

            if (PromptRequest.Themes?.Any() == true)
            {
                promptBuilder.AppendLine($"- **Th�mes Principaux :** {string.Join(", ", PromptRequest.Themes)}");
            }
            if (PromptRequest.LyricalStyles?.Any() == true)
            {
                promptBuilder.AppendLine($"- **Styles des Paroles :** {string.Join(", ", PromptRequest.LyricalStyles)}");
            }

            if (!string.IsNullOrWhiteSpace(PromptRequest.GeneralDescription))
            {
                promptBuilder.AppendLine($"- **Description G�n�rale :** {PromptRequest.GeneralDescription}");
            }
            promptBuilder.AppendLine();

            // --- Section Harmonie ---
            promptBuilder.AppendLine("## HARMONIE ET STRUCTURE MUSICALE");
            if (!string.IsNullOrWhiteSpace(PromptRequest.RootChord) || !string.IsNullOrWhiteSpace(PromptRequest.RootMode))
            {
                promptBuilder.AppendLine($"- **Tonalit� G�n�rale :** {(string.IsNullOrWhiteSpace(PromptRequest.RootChord) ? "(non sp�cifi�)" : PromptRequest.RootChord)} {(string.IsNullOrWhiteSpace(PromptRequest.RootMode) ? "(non sp�cifi�)" : PromptRequest.RootMode)}.");
            }
            if (!string.IsNullOrWhiteSpace(PromptRequest.VerseChord) || !string.IsNullOrWhiteSpace(PromptRequest.VerseMode))
            {
                promptBuilder.AppendLine($"- **Tonalit� pour les Couplets (Verse) :** {(string.IsNullOrWhiteSpace(PromptRequest.VerseChord) ? "(par d�faut)" : PromptRequest.VerseChord)} {(string.IsNullOrWhiteSpace(PromptRequest.VerseMode) ? "(par d�faut)" : PromptRequest.VerseMode)}.");
            }
            if (!string.IsNullOrWhiteSpace(PromptRequest.ChorusChord) || !string.IsNullOrWhiteSpace(PromptRequest.ChorusMode))
            {
                promptBuilder.AppendLine($"- **Tonalit� pour les Refrains (Chorus) :** {(string.IsNullOrWhiteSpace(PromptRequest.ChorusChord) ? "(par d�faut)" : PromptRequest.ChorusChord)} {(string.IsNullOrWhiteSpace(PromptRequest.ChorusMode) ? "(par d�faut)" : PromptRequest.ChorusMode)}.");
            }
            if (!string.IsNullOrWhiteSpace(PromptRequest.BridgeChord) || !string.IsNullOrWhiteSpace(PromptRequest.BridgeMode))
            {
                promptBuilder.AppendLine($"- **Tonalit� pour le Pont (Bridge) :** {(string.IsNullOrWhiteSpace(PromptRequest.BridgeChord) ? "(par d�faut)" : PromptRequest.BridgeChord)} {(string.IsNullOrWhiteSpace(PromptRequest.BridgeMode) ? "(par d�faut)" : PromptRequest.BridgeMode)}.");
            }
            if (PromptRequest.UseModalMixture)
            {
                promptBuilder.AppendLine("- **Complexit� Harmonique :** Utilise des techniques de mixture modale (accords emprunt�s � des modes parall�les) pour enrichir l'harmonie. Sois cr�atif.");
            }

            promptBuilder.AppendLine();
            promptBuilder.AppendLine("Lorsqu'un mode est sp�cifi�, assurez-vous que les accords son typiques du mode.");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("Lorsque vous faites plusieurs chansons dans la m�me conversation, essayez que les paroles et la symbolique des parole soit originale par rapport � ce que vous aviez fait.");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("� la toute fin, g�n�re un titre pour la chanson. Le titre ne doit pas �tre sous la forme de 'Le blues de...' ni 'La balade de...'.");

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