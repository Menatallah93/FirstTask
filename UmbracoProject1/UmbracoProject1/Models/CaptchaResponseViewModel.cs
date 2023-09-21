using Newtonsoft.Json;

namespace UmbracoProject1.Models
{
    public class CaptchaResponseViewModel
    {
        public string SecretKey { get; set; }
        public string SiteKey { get; set; }


        public bool Success { get; set; }

        [JsonProperty(PropertyName = "error-codes")]
        public IEnumerable<string> ErrorCodes { get; set; }

        [JsonProperty(PropertyName = "challenge_ts")]
        public DateTime ChallengeTime { get; set; }

        public string HostName { get; set; }
        public double Score { get; set; }
        public string Action { get; set; }
    }
}
