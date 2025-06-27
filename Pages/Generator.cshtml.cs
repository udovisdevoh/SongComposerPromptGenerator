using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SongPromptGenerator.Models;
using SongPromptGenerator.Services;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SongPromptGenerator.Pages
{
    public class GeneratorModel : PageModel
    {
        private readonly IDataService _dataService;

        // Propriété pour lier les données du formulaire entrant
        [BindProperty]
        public PromptRequest PromptRequest { get; set; }

        // Propriétés pour afficher les options dans les menus déroulants
        public IEnumerable<string> Languages { get; set; }
        public IEnumerable<string> Genres { get; set; }
        public IEnumerable<string> Themes { get; set; }
        public IEnumerable<string> LyricalStyles { get; set; }
        public IEnumerable<string> Chords { get; set; }
        public IEnumerable<string> Modes { get; set; }

        // Propriété pour stocker le résultat
        public string GeneratedPrompt { get; set; }

        public GeneratorModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task OnGetAsync()
        {
            // Charger toutes les données nécessaires pour afficher les formulaires
            Languages = await _dataService.GetLanguagesAsync();
            Genres = await _dataService.GetGenresAsync();
            Themes = await _dataService.GetThemesAsync();
            LyricalStyles = await _dataService.GetLyricalStylesAsync();
            Chords = await _dataService.GetChordsAsync();
            // La ligne suivante a été corrigée
            Modes = await _dataService.GetModesAsync();

            // Initialiser le modèle de requête pour éviter les erreurs de référence nulle
            PromptRequest = new PromptRequest();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Si le modèle n'est pas valide, recharger les données et réafficher la page
                await OnGetAsync();
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
            promptBuilder.AppendLine($"- **Genre Musical :** {PromptRequest.Genre}");
            promptBuilder.AppendLine($"- **Thème Principal :** {PromptRequest.Theme}");
            promptBuilder.AppendLine($"- **Style des Paroles :** {PromptRequest.LyricalStyle}");
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

            promptBuilder.AppendLine($"");
            promptBuilder.AppendLine($"Lorsqu'un mode est spécifié, assurez-vous que les accords son typiques du mode.");

            promptBuilder.AppendLine($"");
            promptBuilder.AppendLine($"Lorsque vous faites plusieurs chansons dans la même conversation, essayez que les paroles et la symbolique des parole soit originale par rapport à ce que vous aviez fait.");

            promptBuilder.AppendLine($"");
            promptBuilder.AppendLine($"À la toute fin, génère un titre pour la chanson.");

            GeneratedPrompt = promptBuilder.ToString();

            // Recharger les listes pour réafficher le formulaire correctement
            await OnGetAsync();

            return Page();
        }
    }
}