using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.FacebookModels;

namespace PTLMFGPLUS.SERVICE
{
    

    public class FacebookService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private const string GraphBase = "https://graph.facebook.com/v19.0";

        public FacebookService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        // STEP A: Build the OAuth URL to redirect the user to Facebook
        public string GetOAuthUrl()
        {
            var appId = _config["Facebook:AppId"];
            var redirect = Uri.EscapeDataString(_config["Facebook:RedirectUri"]);
            var scopes = Uri.EscapeDataString(_config["Facebook:Scopes"]);
            var state = Guid.NewGuid().ToString(); // CSRF protection

            return $"https://www.facebook.com/v25.0/dialog/oauth" +
                   $"?client_id={appId}" +
                   $"&redirect_uri={redirect}" +
                   $"&scope={scopes}" +
                   $"&response_type=code" +
                   $"&state={state}";
        }

        // STEP B: Exchange the code for a User Access Token
        public async Task<string> ExchangeCodeForTokenAsync(string code)
        {
            var appId = _config["Facebook:AppId"];
            var appSecret = _config["Facebook:AppSecret"];
            var redirect = Uri.EscapeDataString(_config["Facebook:RedirectUri"]);

            var url = $"{GraphBase}/oauth/access_token" +
                      $"?client_id={appId}" +
                      $"&client_secret={appSecret}" +
                      $"&redirect_uri={redirect}" +
                      $"&code={code}";

            var response = await _http.GetStringAsync(url);
            var json = JObject.Parse(response);
            return json["access_token"]?.ToString();
        }

        // STEP C: Get list of Pages the user manages
        public async Task<List<FacebookPage>> GetPagesAsync(string userAccessToken)
        {
            var url = $"{GraphBase}/me/accounts?access_token={userAccessToken}&fields=id,name,access_token";
            var response = await _http.GetStringAsync(url);
            var json = JObject.Parse(response);

            return json["data"]?.ToObject<List<FacebookPage>>() ?? new();
        }
        public async Task<JArray> GetFormLeadsAsync(string formId, string pageAccessToken)
        {
            try
            {
                var url = $"{GraphBase}/{formId}/leads" +
                          $"?access_token={pageAccessToken}" +
                          $"&fields=id,created_time,field_data";

                var response = await _http.GetStringAsync(url);
                var json = JObject.Parse(response);

                return json["data"] as JArray ?? new JArray();
            }
            catch (Exception ex)
            {
                // Return error info so we can see what went wrong
                var error = new JArray();
                error.Add(new JObject { ["error"] = ex.Message });
                return error;
            }
        }
        // STEP D: Get Lead Forms for a selected Page
        public async Task<List<LeadForm>> GetLeadFormsAsync(string pageId, string pageAccessToken)
        {
            var url = $"{GraphBase}/{pageId}/leadgen_forms" +
                      $"?access_token={pageAccessToken}" +
                      $"&fields=id,name,status";
            var response = await _http.GetStringAsync(url);
            var json = JObject.Parse(response);

            return json["data"]?.ToObject<List<LeadForm>>() ?? new();
        }

        // STEP E: Subscribe the Page to Webhook (leadgen field)
        public async Task<bool> SubscribePageToWebhookAsync(string pageId, string pageAccessToken)
        {
            var url = $"{GraphBase}/{pageId}/subscribed_apps" +
                      $"?access_token={pageAccessToken}" +
                      $"&subscribed_fields=leadgen";

            var response = await _http.PostAsync(url, null);
            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);
            return json["success"]?.Value<bool>() == true;
        }

        // STEP F: Fetch full lead details when webhook fires
        public async Task<JObject> GetLeadDetailsAsync(string leadId, string pageAccessToken)
        {
            var url = $"{GraphBase}/{leadId}?access_token={pageAccessToken}&fields=field_data,created_time,ad_id,form_id";
            var response = await _http.GetStringAsync(url);
            return JObject.Parse(response);
        }
    }
}
