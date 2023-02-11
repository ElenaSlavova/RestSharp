using System.Text.Json.Serialization;

namespace GitHubApiTests
{
    internal class CtryCityInfo
    {


        [JsonPropertyName("place name")]
        public string city { get; set; }
        
        [JsonPropertyName("state")]
        public string state { get; set; }

        [JsonPropertyName("state abbreviation")]
        public string stateAbbreviation { get; set; }


        
    }
}