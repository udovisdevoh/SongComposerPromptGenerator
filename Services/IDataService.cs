using System.Collections.Generic;
using System.Threading.Tasks;

namespace SongPromptGenerator.Services
{
    public interface IDataService
    {
        Task<IEnumerable<string>> GetLanguagesAsync();
        Task<IEnumerable<string>> GetGenresAsync();
        Task<IEnumerable<string>> GetThemesAsync();
        Task<IEnumerable<string>> GetLyricalStylesAsync();
        Task<IEnumerable<string>> GetChordsAsync();
        Task<IEnumerable<string>> GetModesAsync();
    }
}