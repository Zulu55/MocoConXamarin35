using MocoApp.Constants;
using MocoApp.Models;
using MocoApp.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MocoApp.Services
{
    public class LocationService : ApiService
    {
        public LocationService()
        {

        }

        public async Task<string> GetTotalPositions()
        {
            try
            {
                string uri = Constantes.ApiUrl + "location/gettotals/" + Helpers.Settings.DisplayUserCompany;

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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> GetTotalPositionsCategory(string id)
        {
            try
            {
                string uri = "";
                if (Helpers.Settings.DisplayHasLocation)
                {
                     uri = Constantes.ApiUrl + "category/gettotals/" + id + "?l=true";
                }
                else
                {
                    uri = Constantes.ApiUrl + "category/gettotals/" +Helpers.Settings.DisplayUserCompany+ "?l=false";
                }

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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<string> GetTotalPositionsMenu(string id)
        {
            try
            {
                string uri = "";
                if (Helpers.Settings.DisplayHasLocation)
                {
                    uri = Constantes.ApiUrl + "v1/menu/gettotals/" + id + "?l=true";
                }
                else
                {
                    uri = Constantes.ApiUrl + "v1/menu/gettotals/" + Helpers.Settings.DisplayUserCompany + "?l=false";
                }

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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<string> GetTotalPositionsGroup(string id)
        {
            try
            {
                string uri = Constantes.ApiUrl + "group/gettotals/" + id;

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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<string> GetTotalPositionsProduct(string id)
        {
            try
            {
                string uri = Constantes.ApiUrl + "product/gettotals/" + id;

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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Location> GetLocationById(string id)
        {
            try
            {
                string uri = Constantes.ApiUrl + "location/getLocationById?id=" + id;

                var response = await client.GetAsync(uri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var retorno = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<Location>(retorno) ;
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
