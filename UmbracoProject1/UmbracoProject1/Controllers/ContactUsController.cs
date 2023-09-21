using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common.Filters;
using Umbraco.Cms.Web.Website.Controllers;
using UmbracoProject1.Models;
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core.Models.ContentEditing;

namespace UmbracoProject1.Controllers
{
    public class ContactUsController : SurfaceController
    {
        private readonly IContentService _contentService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContactUsController(
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IHttpContextAccessor httpContextAccessor,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider,
            IContentService contentService) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _contentService = contentService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("Form");
        }


        [HttpPost]
        [ValidateUmbracoFormRouteString]
        public async Task<IActionResult> Submit(ContactUsModel contactUs)
        {
            string captchaToken = HttpContext.Request.Form["GoogleCaptchaToken"];
            var isCaptchaValid = await IsCaptchaValid(contactUs.GoogleCaptchaToken);
            if (ModelState.IsValid)
            {
                string name = contactUs.Name;
                string email = contactUs.Email;
                string subject = contactUs.Subject;
                string message = contactUs.Message;

                var parntId = new Guid("7106312b-65b8-4589-96fa-0c3e48469d9c");

                var clientContent = _contentService.Create(contactUs.Name, parntId, "Form");
                clientContent.SetValue("ClientName", contactUs.Name);
                clientContent.SetValue("ClientEmail", contactUs.Email);
                clientContent.SetValue("ClinetSubject", contactUs.Subject);
                clientContent.SetValue("ClientMassage", contactUs.Message);


                _contentService.SaveAndPublish(clientContent);
                return RedirectToCurrentUmbracoPage();
            }
            else
            {
                ModelState.AddModelError("GoogleRecaptcha", "The Captcha is not valid");
            }

            return View(contactUs);
        }

        private async Task<bool> IsCaptchaValid(string response)
        {
            try
            {
                var secret = "6LcjgUIoAAAAAGm0L7zO5cQkaP4E5CEUo1oD8t9s";
                var userHostAddress = HttpContext.Connection.RemoteIpAddress?.ToString(); // Get the user's IP address

                using (var client = new HttpClient())
                {
                    var values = new Dictionary<string, string>
                        {
                            {"secret", secret},
                            {"response", response},
                            {"remoteip", userHostAddress}

                        };

                    var content = new FormUrlEncodedContent(values);
                    var verify = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
                    var captchaResponseJson = await verify.Content.ReadAsStringAsync();
                    var captchaResult = JsonConvert.DeserializeObject<CaptchaResponseViewModel>(captchaResponseJson);
                    return captchaResult.Success
                           && captchaResult.Action == "/Submit"
                           && captchaResult.Score > 0.5;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}