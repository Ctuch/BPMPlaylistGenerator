using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using SpotifyAPI.Web;
using System.Threading.Tasks;

namespace BPMPlaylistGenerator.Pages
{
    public class SpotifyBase : ComponentBase
    {
        protected List<string> selectedGenres = new List<string>();
        protected string displaySelectedValue { get; set; }
        protected string? minBPMInput;
        protected string? maxBPMInput;
        protected bool _validInput = false;
        protected SpotifyClient spotify;

        protected async Task ShowSelectedValues()
        {
            if (!IsValidInput())
            {
                displaySelectedValue = "Input is not valid";
            } else
            {
                var recommendations = await GetRecommendations();
                foreach (var rec in recommendations)
                {
                    displaySelectedValue += rec.Name + ", ";
                }
            }
            
            StateHasChanged();
        }

        protected bool IsValidInput()
        {
            var validGenres = CheckGenreInput();
            var validBPM = CheckBPMInput();
            return validGenres && validBPM;
        }

        private bool CheckGenreInput()
        {
            return selectedGenres.Count > 0 && selectedGenres.Count <= 5;
        }

        private bool CheckBPMInput()
        {
            if (int.TryParse(minBPMInput, out int minBPM) && int.TryParse(maxBPMInput, out int maxBPM))
            {
                return minBPM < maxBPM && maxBPM < 500 && minBPM >= 0;
            }
            return false;
        }

        private async Task<List<SimpleTrack>> GetRecommendations()
        {
            var recommendationsRequest = new RecommendationsRequest
            {
                Limit = 100
            };

            foreach (var genre in selectedGenres)
            {
                recommendationsRequest.SeedGenres.Add(genre);
            }
            recommendationsRequest.Max.Add("tempo", maxBPMInput);
            recommendationsRequest.Min.Add("tempo", minBPMInput);

            var recommendations = await spotify.Browse.GetRecommendations(recommendationsRequest);
            return recommendations.Tracks;
        }
    }
}
