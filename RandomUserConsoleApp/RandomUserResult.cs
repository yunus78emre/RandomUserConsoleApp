using Newtonsoft.Json;
using RandomUserConsoleApp.UserModels;


namespace RandomUserConsoleApp
{
    public class RandomUserResult
    {
        [JsonProperty("results")]
        public List<User> Results { get; set; }
    }
}
