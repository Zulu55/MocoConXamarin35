using MocoApp.Constants;
using MocoApp.Resources;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static MocoApp.Services.ApiService;

namespace MocoApp.Services.V2
{
    public class BaseHttpService
    {

        public BaseHttpService()
        {

        }

        public async Task<T> Get<T>(string endpoint)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Helpers.Settings.DisplayUserToken);
                    client.Timeout = TimeSpan.FromSeconds(25);

                    client.BaseAddress = new Uri(Constantes.ApiUrl);

                    var response = await client.GetAsync(endpoint);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var retorno = response.Content.ReadAsStringAsync().Result;

                        return JsonConvert.DeserializeObject<T>(retorno) ;
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        App.AppCurrent.Logout();
                        throw new Exception(AppResource.alertSessionExpired);
                    }
                    else
                    {
                        throw new Exception(JsonConvert.DeserializeObject<ExceptionMessage>(response.Content.ReadAsStringAsync().Result).Message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
