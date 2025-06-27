using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace SongPromptGenerator.Services
{
    public class JsonDataService : IDataService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly JsonSerializerOptions _jsonOptions;

        public JsonDataService(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        private async Task<IEnumerable<string>> ReadDataFileAsync(string fileName)
        {
            var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Data", fileName);
            if (!File.Exists(filePath))
            {
                return new List<string>();
            }

            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return await JsonSerializer.DeserializeAsync<List<string>>(stream, _jsonOptions);
        }

        public Task<IEnumerable<string>> GetLanguagesAsync() => ReadDataFileAsync("languages.json");
        public Task<IEnumerable<string>> GetGenresAsync() => ReadDataFileAsync("genres.json");
        public Task<IEnumerable<string>> GetThemesAsync() => ReadDataFileAsync("themes.json");
        public Task<IEnumerable<string>> GetLyricalStylesAsync() => ReadDataFileAsync("lyricalStyles.json");
        public Task<IEnumerable<string>> GetChordsAsync() => ReadDataFileAsync("chords.json");
        public Task<IEnumerable<string>> GetModesAsync() => ReadDataFileAsync("modes.json");
    }
}