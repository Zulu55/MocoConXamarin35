using MocoApp.Constants;
using MocoApp.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MocoApp.Services
{
    public class ApiService
    {
        public HttpClient client = new HttpClient();



        public ApiService()
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Helpers.Settings.DisplayUserToken);
            client.Timeout = TimeSpan.FromSeconds(25);
        }

        public async Task<string> UploadImage(Stream imageStream)
        {
            try
            {


                StreamContent scontent = new StreamContent(imageStream);
                scontent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    FileName = "newimage",
                    Name = "image"
                };
                scontent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

                var client = new HttpClient();
                var multi = new MultipartFormDataContent();
                multi.Add(scontent);
                client.BaseAddress = new Uri(Constantes.ApiUrl);
                var result = await client.PostAsync("image/upload", multi);

                var response = result.Content.ReadAsStringAsync().Result;

                if (string.IsNullOrEmpty(response))
                    throw new Exception("Error");



                string image = JsonConvert.DeserializeObject<List<string>>(response).First();

                return image;



            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                //return "";

            }
            finally
            {

            }
        }

        public async Task<string> PostAsync(string json, string url)
        {

            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(Constantes.ApiUrl + url, content);

            var respostaString = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(JsonConvert.DeserializeObject<ExceptionMessage>(respostaString).Message);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                App.AppCurrent.Logout();
                throw new Exception(AppResource.alertSessionExpired);
            }

            return respostaString;

        }

        public async Task<string> PutAsync(string json, string url)
        {

            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PutAsync(Constantes.ApiUrl + url, content);

            var respostaString = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(JsonConvert.DeserializeObject<ExceptionMessage>(respostaString).Message);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                App.AppCurrent.Logout();
                throw new Exception(AppResource.alertSessionExpired);
            }

            return respostaString;

        }

        public async Task<string> GetAsync(string url)
        {
            try
            {
                string uri = Constantes.ApiUrl + url;

                var response = await client.GetAsync(uri);

                Debug.WriteLine(response);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var retorno = response.Content.ReadAsStringAsync().Result;
                    return retorno;
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
            catch (TaskCanceledException e)
            {
                throw new InvalidOperationException(AppResource.alertSlowConnection);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

        public async Task UpdateAcess(string token, string id, string deviceModel, string version)
        {
            try
            {
                string uri = Constantes.ApiUrl + string.Format("user/access?pushToken={0}&pushId={1}&deviceModel={2}&version={3}&userId={4}", token, id, deviceModel, version, Helpers.Settings.DisplayUserId);

                var response = await client.GetAsync(uri);


            }
            catch (Exception ex)
            {
               
            }
        }

        public async Task<string> Auth(string login, string password)
        {

            var headers = new Dictionary<string, string>();
            headers.Add("username", login);
            headers.Add("password", password);
            headers.Add("grant_type", "password");

            var content = new FormUrlEncodedContent(headers);
            var response = await client.PostAsync(Constantes.ApiUrl + "security/token", content);

            var respostaString = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(AppResource.alertLoginBadAuth);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {

                App.AppCurrent.Logout();
                throw new Exception(AppResource.alertSessionExpired);
            }

            return respostaString;

        }


        public async Task<string> GetMe(string pushId, string pushToken, string lat, string lon, string token)
        {
            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                string uri = Constantes.ApiUrl + string.Format("user/me?pushId={0}&pushToken={1}&lat={2}&lon={3}", pushId, pushToken, lat, lon);

                var response = await client.GetAsync(uri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var retorno = response.Content.ReadAsStringAsync().Result;
                    return retorno;
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
            catch (TaskCanceledException e)
            {
                throw new InvalidOperationException(AppResource.alertSlowConnection);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

        public class ExceptionMessage
        {
            public string Message { get; set; }
        }
    }
}
