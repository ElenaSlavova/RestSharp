using System.Text.Json.Serialization;

namespace GitHubApiTests
{
    internal class Location
    {
        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("country abbreviation")]
        public string ctryAbbreviation { get; set; }

        [JsonPropertyName("places")]
        public List<CtryCityInfo> countryInfo { get; set; }

        [JsonPropertyName("post code")]
        public string ctryZipCode { get; set; }
    }
}