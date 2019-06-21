using BusinessLogic.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class GoogleReCaptchaService : IReCaptchaService
    {
        private readonly ReCaptchaOptions _options;
        private readonly HttpClient _httpClient;

        public GoogleReCaptchaService(IOptions<AppOptions> optionsAccessor)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://www.google.com")
            };

            _options = optionsAccessor.Value.ReCaptcha;
        }

        public async Task<ReCaptchaResponse> Validate(IFormCollection form)
        {
            var gRecaptchaResponse = form["g-recaptcha-response"];
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("secret", _options.SecretKey),
                new KeyValuePair<string, string>("response", gRecaptchaResponse)
            });

            var response = await _httpClient.PostAsync("/recaptcha/api/siteverify", content);
            var resultContent = await response.Content.ReadAsStringAsync();
            var captchaResponse = JsonConvert.DeserializeObject<ReCaptchaResponse>(resultContent);

            return captchaResponse;
        }
    }
}
