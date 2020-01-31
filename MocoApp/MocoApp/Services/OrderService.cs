using MocoApp.Constants;
using MocoApp.Models;
using MocoApp.Models.ReportCommand;
using MocoApp.Resources;
using MocoApp.Views.CompanyFluxo;
using MocoApp.Views.ManagerCheckinFlow;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static MocoApp.Models.Enums;

namespace MocoApp.Services
{
    public class OrderService : ApiService
    {
        public OrderService()
        {

        }

        public async Task<string> GetcheckinsByClientId()
        {
            try
            {
                string uri = Constantes.ApiUrl + "check/getCheckinsByClientId?id=" + Helpers.Settings.DisplayUserId;

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

        public async Task<string> GetLocationCheckedInDTO(string checkinId)
        {
            try
            {
                string uri = Constantes.ApiUrl + "location/getLocationsByCheckinId?id=" + checkinId;

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

        public async Task<string> GetOrdersByCheckinId(string checkinId)
        {
            try
            {
                string uri = Constantes.ApiUrl + "order/getOrdersByCheckinId?id=" + checkinId;

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

        public async Task<string> GetOrderGrouped(string checkinId, string companyId)
        {
            try
            {
                string uri = Constantes.ApiUrl + "order/getOrderGrouped?checkinId=" + checkinId + "&companyId=" + companyId;

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

        public async Task<string> GetOrdersByClienteIdAndCompanyId(string companyId)
        {
            try
            {
                string uri = Constantes.ApiUrl + "order/getClientOrdersByCompanyId?companyId=" + companyId + "&clientId=" + Helpers.Settings.DisplayUserId;

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

        public async Task<string> GetCheckinByClienteAndCompanyId(string companyId)
        {
            try
            {
                string uri = Constantes.ApiUrl + "check/getCheckinByClientIdCompanyId?companyId=" + companyId + "&clientId=" + Helpers.Settings.DisplayUserId;

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

        public async Task<string> GetCheckinsSubsByCheckinId(string checkId)
        {
            try
            {
                string uri = Constantes.ApiUrl + "check/GetCheckinsSubsByCheckinId?id=" + checkId;

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

        public async Task<string> UpdateRequestedSubCheckoutTax(CheckoutSubTax command)
        {

            var json = JsonConvert.SerializeObject(command);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PutAsync(Constantes.ApiUrl + "check/close-location", content);

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



        public async Task<string> UpdateRequestedSubCheckout(string id, CheckinSubStatus status)
        {
            try
            {
                string uri = Constantes.ApiUrl + "/check/updateRequestedSubCheckout?id=" + id + "&status=" + status;

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

        public async Task<string> CreateOrder(CreateOrder co)
        {

            var json = JsonConvert.SerializeObject(co);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(Constantes.ApiUrl + "order/create", content);

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
        public async Task<string> CreateOrderV2(CreateOrderCommandV2 co)
        {
            var json = JsonConvert.SerializeObject(co);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(Constantes.ApiUrl + "v2/order/create", content);

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

        public async Task<string> GetOrdersOpenedByCompanyId()
        {
            try
            {
                string uri = Constantes.ApiUrl + "order/getOrdersByCompanyIdAndStatus?companyId=" + Helpers.Settings.DisplayUserCompany + "&status=1";

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

        //teste xnd
        //
        //
        // ' vlwwwwwwwwwww bdk gente boa xd

        public async Task<string> GetOrderHistory(DateTimeOffset startDate, DateTimeOffset endDate, string locationId = "")
        {
            try
            {
                var localDate = DateTimeOffset.UtcNow;

                var visao = new OrderHistoryFilter()
                {
                    EndDate = endDate,
                    StartDate = startDate,
                    LocationId = locationId,
                    LocalDate = localDate,
                    Id = Helpers.Settings.DisplayUserCompany
                };

                string json = JsonConvert.SerializeObject(visao);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                string uri = Constantes.ApiUrl + "report/ordersByCompany";

                var response = await client.PostAsync(uri, content);


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

        //vlw vacuo, fim parcero entao imrao

        //public async Task<string> GetOrderHistory(string startDate, string endDate, string locationId = "")
        //{
        //    try
        //    {
        //        string localDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        //        string uri = Constantes.ApiUrl + "report/getOrdersByCompany?id=" + Helpers.Settings.DisplayUserCompany + "&startDate=" + startDate + "&endDate=" + endDate + "&localDate=" + localDate + "&locationId=" + locationId;

        //        var response = await client.GetAsync(uri);


        //        if (response.StatusCode == HttpStatusCode.OK)
        //        {
        //            var retorno = response.Content.ReadAsStringAsync().Result;
        //            return retorno;
        //        }
        //        else if (response.StatusCode == HttpStatusCode.Unauthorized)
        //        {
        //            App.AppCurrent.Logout();
        //            throw new Exception(AppResource.alertSessionExpired);
        //        }
        //        else
        //        {
        //            throw new Exception(JsonConvert.DeserializeObject<ExceptionMessage>(response.Content.ReadAsStringAsync().Result).Message);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        public async Task<string> GetMyOrdersAccepted()
        {
            try
            {
                string uri = Constantes.ApiUrl + "order/getOrdersByEmployeeId?userId=" + Helpers.Settings.DisplayUserId + "&companyId=" + Helpers.Settings.DisplayUserCompany;

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

        public async Task<string> GetCheckinsByCompanyId(string locationId = "", DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, DateTimeOffset? localDAte = null)
        {
            try
            {
    

                var command = new CheckinFilter()
                {
                    EndDate = endDate,
                    StartDate = startDate,
                    LocalDate = localDAte,
                    Id = Helpers.Settings.DisplayUserCompany,
                    LocationId = locationId
                };

                string json = JsonConvert.SerializeObject(command);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                string uri = Constantes.ApiUrl + "check/checkinsByCompanyId";

                var response = await client.PostAsync(uri, content);





                //string uri = Constantes.ApiUrl + "check/getCheckinsByCompanyId?id=" + Helpers.Settings.DisplayUserCompany + "&locationId=" + locationId + "&startDate=" + startDate + "&endDate=" + endDate + "&localDAte=" + localDAte;

                //var response = await client.GetAsync(uri);





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

        public async Task<string> UpdateOrderStatus(UpdateOrder up)
        {

            string json = JsonConvert.SerializeObject(up);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(Constantes.ApiUrl + "order/v2/updateOrderStatus", content);

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

        public async Task<string> UpdateAllOrderStatus(UpdateOrder up)
        {

            string json = JsonConvert.SerializeObject(up);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(Constantes.ApiUrl + "order/update-all", content);

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
    }
}
