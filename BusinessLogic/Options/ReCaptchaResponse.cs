using Newtonsoft.Json;
using System.Collections.Generic;

namespace BusinessLogic.Options
{
    public class ReCaptchaResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("error-codes")]
        public List<string> ErrorCodes { get; set; }
    }
}
