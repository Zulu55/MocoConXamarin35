using MocoApp.Constants;
using MocoApp.Models;
using MocoApp.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MocoApp.Services.V2
{
    public class OrderServicev2 : ApiService
    {

        public async Task<bool> MakeCartOrder(CartModel command, string token)
        {
            try
            {
                var json = JsonConvert.SerializeObject(command);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(Constantes.ApiUrl + "order/cart", content);

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
    }
}
