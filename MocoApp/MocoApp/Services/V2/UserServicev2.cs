using MocoApp.Constants;
using MocoApp.Models;
using MocoApp.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MocoApp.Services.V2
{
    public class UserServicev2 : ApiService
    {

        public async Task<bool> AddCard(UserWallet command, string token)
        {
            try
            {
                var json = JsonConvert.SerializeObject(command);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(Constantes.ApiUrl + "user/add-card", content);

                var respostaString = response.Content.ReadAsStringAsync().Result;

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new Exception(JsonConvert.DeserializeObject<ExceptionMessage>(respostaString).Message);
                }
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    App.AppCurrent.Logout();
                    throw new Exception(AppResource.alertSessionExpired);
                }

                return false;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> AddCardV2(string tokenStripe, string token)
        {
            try
            {
                var json = JsonConvert.SerializeObject(new { Token = tokenStripe });
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Helpers.Settings.DisplayUserToken);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(Constantes.ApiUrl + "v1/payment/card", content);

                var respostaString = response.Content.ReadAsStringAsync().Result;

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new Exception(JsonConvert.DeserializeObject<ExceptionMessage>(respostaString).Message);
                }
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    App.AppCurrent.Logout();
                    throw new Exception(AppResource.alertSessionExpired);
                }

                return false;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<UserWallet>> GetMyWallet()
        {
            try
            {
                string uri = Constantes.ApiUrl + "user/wallet/" + Helpers.Settings.DisplayUserId;

                var response = await client.GetAsync(uri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var retorno = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<List<UserWallet>>(retorno);
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


        public async Task<List<UserWallet>> GetMyWalletV2()
        {
            try
            {
                string uri = Constantes.ApiUrl + "v1/payment/cards";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Helpers.Settings.DisplayUserToken);

                var response = await client.GetAsync(uri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var retorno = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<List<UserWallet>>(retorno);
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

        public async Task<bool> SwitchMainCard(string id)
        {
            try
            {
                string uri = Constantes.ApiUrl + "v1/payment/card/switch/" + id;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Helpers.Settings.DisplayUserToken);

                var response = await client.GetAsync(uri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var retorno = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<bool>(retorno);
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
