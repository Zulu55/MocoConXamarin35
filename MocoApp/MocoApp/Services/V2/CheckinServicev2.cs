using MocoApp.Constants;
using MocoApp.Models;
using MocoApp.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MocoApp.Services.V2
{
    public class CheckinServicev2 : ApiService
    {


        public async Task<Checkin> GetActiveCheckin(string companyId, string token)
        {
            try
            {
                string uri = Constantes.ApiUrl + "check/active-checkin/" + companyId;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync(uri);


                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var retorno = response.Content.ReadAsStringAsync().Result;
                    var convert = JsonConvert.DeserializeObject<Checkin>(retorno);

                    return convert;
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
