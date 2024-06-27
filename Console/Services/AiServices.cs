using ChildHealthBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildHealthBot.Services;

public class BardServices
{
    string BaseUri = @"https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:";
    string ApiKey = @"AIzaSyCagUChqtWPJ_5ULldwK3rRrUPDkSREFOI";

    HttpClient client;

    public BardServices()
    {
        client = new HttpClient();
    }

    public async Task<string> GetResponse(PromptModel Query)
    {

        try
        {
            string jsonPromptString = JsonConvert.SerializeObject(Query);

            var request = new HttpRequestMessage(HttpMethod.Post, BaseUri + "generateContent?key=" + ApiKey);

            var ClientBodyContentString = new StringContent(jsonPromptString, null, "application/json");
            request.Content = ClientBodyContentString;
            client.DefaultRequestHeaders.Add("Accept-Language", "bn");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
            {
                var JsonResponse = await response.Content.ReadAsStringAsync();

                //Perse Jsonstring to object
                var ResponseContent = JsonConvert.DeserializeObject<ResponseModels>(JsonResponse);

                var message = ResponseContent.Candidates[0].Content.Parts[0].Text;

                return message;
            }
            else return "Error";

        }
        catch (Exception ex)
        {
            return "Exception";
        }
    }

}

