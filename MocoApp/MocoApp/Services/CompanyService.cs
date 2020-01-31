using MocoApp.Constants;
using MocoApp.Models;
using MocoApp.Models.ReportCommand;
using MocoApp.Resources;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static MocoApp.Models.Enums;

namespace MocoApp.Services
{
    public class CompanyService : ApiService
    {
        public CompanyService()
        {
        }

        public async Task<string> GetCompaniesByFilter(string name = "")
        {
            try
            {
                string uri = Constantes.ApiUrl + "company/GetCompaniesByFilter?companyType=" + (int)App.CompanyTypeSelected
                    + "&clientId=" + Helpers.Settings.DisplayUserId
                    + "&filterType=" + (int)App.FilterType
                    + "&name=" + name
                    + "&latitude=" + App.AppCurrent.Latitude
                    + "&longitude=" + App.AppCurrent.Longitude;

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

        public async Task<string> GetCompaniesByTypeAndPosition(int type)
        {
            try
            {
                string uri = Constantes.ApiUrl + "company/getNearbyCompaniesByType?companyType=" + type + "&clientId=" + Helpers.Settings.DisplayUserId + "&latitude=" + App.AppCurrent.Latitude + "&longitude=" + App.AppCurrent.Longitude;

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

        public async Task<string> AddRemoveCompanyFavorite(string companyId)
        {
            try
            {
                string uri = Constantes.ApiUrl + "company/includeClientCompanyFavorite?companyId=" + companyId + "&clientId=" + Helpers.Settings.DisplayUserId;

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

        public async Task<string> AddRemoveCheckin(string companyId, string pricetippaid = "")
        {
            try
            {
                string uri = Constantes.ApiUrl + "check/checkin?companyId=" + companyId + "&clientId=" + Helpers.Settings.DisplayUserId + "&pricetippaid=" + pricetippaid;

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

        public async Task<string> RequestCheckoutFromClient(string companyId, decimal pricetippaid, bool FromCompanyHome = false, decimal PaidInCash = 0, decimal PaidInCard = 0, bool ignoreOccupation = false, PaymentMethod method = PaymentMethod.NotInformed, string stripeID = "")
        {
            try
            {
                string uri = Constantes.ApiUrl + "check/checkin";
                var item = new
                {
                    CompanyId = companyId,
                    ClientId = Helpers.Settings.DisplayUserId,
                    PriceTipPaid = pricetippaid,
                    FromCompanyHome = FromCompanyHome,
                    PaidInCash = PaidInCash,
                    PaidInCard = PaidInCard,
                    IgnoreOccupation = ignoreOccupation,
                    PaymentMethod = method,
                    StripeId = stripeID
                };

                var json = JsonConvert.SerializeObject(item);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Helpers.Settings.DisplayUserToken);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(uri, content);


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

        public async Task<string> RequestSubCheckout(string id)
        {
            try
            {
                string uri = Constantes.ApiUrl + "check/requestSubCheckout?id=" + id;

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

        public async Task<string> RequestPayLater(string id, decimal tipPaid, decimal paidCard, decimal paidCash, bool paid, bool home = false)
        {
            try
            {
                PaymentMethod paymentMethod = PaymentMethod.NotInformed;
                if (paidCard == 0 && paidCash == 0)
                    paymentMethod = PaymentMethod.PayLater;
                else if (paidCard > paidCash)
                    paymentMethod = PaymentMethod.Card;
                else if (paidCash > paidCard)
                    paymentMethod = PaymentMethod.Money;

                var item = new
                {
                    TipPaid = tipPaid,
                    Id = id,
                    PaidInCash = paidCash,
                    PaidInCard = paidCard,
                    //Paid = paid,
                    Paid = false,
                    FromHome = home,
                    PaymentMethod = paymentMethod
                };


                string uri = Constantes.ApiUrl + "check/requestSubCheckout";
                var json = JsonConvert.SerializeObject(item);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(uri, content);

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

        public async Task<string> UpdateRequestedCheckin(string checkinId, string occupation, string quantity, CheckinStatus status, string allocationNumber, string checkinSubId = "", string denied = "")
        {
            try
            {
                string uri = Constantes.ApiUrl + "check/updateRequestedCheckin?checkinId=" + checkinId + "&occupation=" + occupation + "&quantity=" + quantity + "&status=" + status + "&allocationNumber=" + allocationNumber + "&sub=" + checkinSubId + "&reason=" + denied;

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

        public async Task<bool> AddRemoveCheckinPost(RequestCheckin requestCheckin)
        {
            try
            {
                string json = JsonConvert.SerializeObject(requestCheckin);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(Constantes.ApiUrl + "check/requestCheckin", content);

                var respostaString = response.Content.ReadAsStringAsync().Result;

                if (response.StatusCode == HttpStatusCode.OK)
                    return true;

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new Exception(JsonConvert.DeserializeObject<ExceptionMessage>(response.Content.ReadAsStringAsync().Result).Message);
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

        public async Task<bool> RequestCheckinSub(CheckinSub sub)
        {
            try
            {
                string json = JsonConvert.SerializeObject(sub);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(Constantes.ApiUrl + "check/requestCheckinSub", content);

                var respostaString = response.Content.ReadAsStringAsync().Result;

                if (response.StatusCode == HttpStatusCode.OK)
                    return true;

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new Exception(JsonConvert.DeserializeObject<ExceptionMessage>(response.Content.ReadAsStringAsync().Result).Message);
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

        public async Task<string> GetCheckinSubByLocationId(string locationId)
        {
            try
            {
                string uri = Constantes.ApiUrl + "check/getCheckinSubByLocationId?locationId=" + locationId + "&clientId=" + Helpers.Settings.DisplayUserId;

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

        public async Task<string> GetCompanyById(string id)
        {
            try
            {
                string uri = Constantes.ApiUrl + "company/getCompanyById?companyId=" + id + "&clientId=" + Helpers.Settings.DisplayUserId;
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

        public async Task<string> GetCompanyPhotos(string id)
        {
            try
            {
                string uri = Constantes.ApiUrl + "company/getCompanyPhotos?id=" + id + "&clientId=" + Helpers.Settings.DisplayUserId;

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

        public async Task<string> GetCompanyDelivery(string id)
        {
            try
            {
                string uri = Constantes.ApiUrl + "company/getCompanyDeliveries?id=" + id;
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

        public async Task<string> GetProductsByLocationId(string id)
        {
            try
            {
                string uri = Constantes.ApiUrl + "product/getProductsByLocationId?id=" + id;
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

        public async Task<string> GetProductsByCategoryAndCompanyId(string categoryId, string companyId, bool c = false)
        {
            try
            {
                string uri = Constantes.ApiUrl + "product/getProductsByCategoryId?companyId=" + companyId + "&categoryId=" + categoryId + "&clientId=" + Helpers.Settings.DisplayUserId;
                if (c)
                    uri += "&isCompany=true";
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

        public async Task<string> GetProductsByCompanyId(string companyId)
        {
            try
            {
                string uri = Constantes.ApiUrl + "product/getProductsByCompanyId?id=" + companyId;

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


        public async Task<string> GetGroupsAndProductsByCategoryId(string categoryId)
        {
            try
            {
                string uri = Constantes.ApiUrl + "group/getGroupsAndProductsByCategoryId?id=" + categoryId;

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

        /// <summary>
        /// retorna todos ate os deletados para a empresa
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public async Task<string> GetAllProductsByCategoryAndCompanyId(string groupId, bool isCompany)
        {
            try
            {
                string uri = Constantes.ApiUrl + "product/getProductsByGroupId?groupId=" + groupId + "&isCompany=" + isCompany;

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

        public async Task<string> GetAllProductsByCategoryId(string categoryId, bool isCompany)
        {
            try
            {
                string uri = Constantes.ApiUrl + "product/getProductsByCategoryId?categoryId=" + categoryId + "&isCompany=" + isCompany + "&companyId=" + Helpers.Settings.DisplayUserCompany;

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

        /// <summary>
        /// Encerra a conta
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public async Task<string> MakeCheckout(string companyId, string clientId)
        {
            try
            {
                string uri = Constantes.ApiUrl + "company/checkout?companyId=" + companyId + "&clientId=" + clientId;

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


        /// <summary>
        /// Fecha a conta pelo funcionario gerente
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public async Task<string> MakeCheckoutTax(CheckoutTax command)
        {

            string json = JsonConvert.SerializeObject(command);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(Constantes.ApiUrl + "check/checkouttax", content);

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


        public async Task<string> SolicitarAtendimento(string companyId, string locationid = "")
        {
            try
            {
                string uri = Constantes.ApiUrl + "order/serviceRequest?companyid=" + companyId + "&clientid=" + Helpers.Settings.DisplayUserId + "&locationId=" + locationid;

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

        public async Task<string> GetProductCategory()
        {
            try
            {
                string uri = Constantes.ApiUrl + "category/getCategoriesSoftByCompanyId?id=" + Helpers.Settings.DisplayUserCompany;

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


        public async Task<string> GetAllProductCategoryByLocationId(string locationId, bool company)
        {
            try
            {
                string uri = Constantes.ApiUrl + "category/getCategoriesByLocationId?id=" + locationId + "&iscompany=" + company + "&companyId=" + Helpers.Settings.DisplayUserCompany;

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
        public async Task<string> GetAllInformativeMenus(string locationId, bool company)
        {
            try
            {
                string uri = Constantes.ApiUrl + "v1/menu/" + locationId + "/location?c=" + company;

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

        public async Task<string> GetInformativeMenuById(string id)
        {
            try
            {
                string uri = Constantes.ApiUrl + "v1/menu/" + id;

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
        public async Task<string> CreateCategoryGroup(CategoryGroup group)
        {

            string json = JsonConvert.SerializeObject(group);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(Constantes.ApiUrl + "group/register", content);

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

        public async Task<string> EditCategoryGroup(CategoryGroup group)
        {

            string json = JsonConvert.SerializeObject(group);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PutAsync(Constantes.ApiUrl + "group/update", content);

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

        public async Task<string> GetAllCategoryGroupByCategoryId(string categoryId, bool isCompany)
        {
            try
            {
                string uri = Constantes.ApiUrl + "/group/getGroupsByCategoryId?id=" + categoryId + "&isCompany=" + isCompany;

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


        public async Task<string> GetAllLocationsByCompanyId()
        {
            try
            {
                string uri = Constantes.ApiUrl + "/location/getAllLocationsByCompanyId?id=" + Helpers.Settings.DisplayUserCompany;

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


        /// <summary>
        /// Traz apenas os ativos
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetLocationsByCompanyId()
        {
            try
            {
                string uri = Constantes.ApiUrl + "/location/getLocationsByCompanyId?id=" + Helpers.Settings.DisplayUserCompany;

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

        public async Task<string> CreateLocation(Location location)
        {

            string json = JsonConvert.SerializeObject(location);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(Constantes.ApiUrl + "location/register", content);

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

        public async Task<string> EditLocation(Location location)
        {

            string json = JsonConvert.SerializeObject(location);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PutAsync(Constantes.ApiUrl + "location/update", content);

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

        public async Task<string> CreateProduct(Product product)
        {

            string json = JsonConvert.SerializeObject(product);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(Constantes.ApiUrl + "product/register", content);

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

        public async Task<string> EditProduct(Product product)
        {

            string json = JsonConvert.SerializeObject(product);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PutAsync(Constantes.ApiUrl + "product/update", content);

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

        public async Task<string> CreateProductCategory(Category product)
        {

            string json = JsonConvert.SerializeObject(product);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(Constantes.ApiUrl + "category/createCategory", content);

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
        public async Task<string> CreateInformativeMenu(InformativeMenu menu)
        {

            string json = JsonConvert.SerializeObject(menu);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(Constantes.ApiUrl + "v1/menu", content);

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
        public async Task<string> EditInformativeMenu(InformativeMenu menu)
        {

            string json = JsonConvert.SerializeObject(menu);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PutAsync(Constantes.ApiUrl + "v1/menu/update", content);

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


        public async Task<string> EditProductCategory(Category product)
        {

            string json = JsonConvert.SerializeObject(product);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PutAsync(Constantes.ApiUrl + "category/updateCategory", content);

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

        public async Task<string> GetProductReport(DateTimeOffset startDate, DateTimeOffset endDate, string productId = "", string categoryId = "", string locationId = "")
        {
            try
            {
                var dateNow = DateTime.Now;
                //string uri = Constantes.ApiUrl + "report/getProductCompanyReport?companyId=" + Helpers.Settings.DisplayUserCompany + "&startDate=" + startDate + "&endDate=" + endDate + "&productId=" + productId + "&localDate=" + dateNow;
                //var response = await client.GetAsync(uri);

                var localDate = DateTimeOffset.UtcNow;

                var partilha = new ProductCompanyReportFilter()
                {
                    EndDate = endDate,
                    StartDate = startDate,
                    LocalDate = localDate,
                    CompanyId = Helpers.Settings.DisplayUserCompany,
                    ProductId = productId,
                    CategoryId = categoryId,
                    LocationId = locationId
                };

                string json = JsonConvert.SerializeObject(partilha);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                string uri = Constantes.ApiUrl + "report/productCompanyReport";

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

        public async Task<string> GetCompanyReport(DateTimeOffset startDate, DateTimeOffset endDate, TimeSpan timeInit, TimeSpan timeEnd, string employeeId = "", string locationId = "")
        {
            try
            {
                var dateNow = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                var localDate = DateTimeOffset.UtcNow;

                var tomandoBeraTal = new OrderCompanyFilter()
                {
                    EndDate = endDate.AddMinutes(timeEnd.TotalMinutes),
                    StartDate = startDate.AddMinutes(timeInit.TotalMinutes),
                    LocalDate = localDate,
                    CompanyId = Helpers.Settings.DisplayUserCompany,
                    EmployeeId = employeeId,
                    LocationId = locationId
                };

                string json = JsonConvert.SerializeObject(tomandoBeraTal);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                string uri = Constantes.ApiUrl + "v2/report/company";

                var response = await client.PostAsync(uri, content);

                //string uri = Constantes.ApiUrl + "report/getOrderCompanyReport?companyId=" + Helpers.Settings.DisplayUserCompany + "&startDate=" + startDate + "&endDate=" + endDate + "&employeeId=" + employeeId + "&localDate=" + dateNow + "&locationId=" + locationId;
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

        public async Task<string> GetEmployees()
        {
            try
            {
                string uri = Constantes.ApiUrl + "employee/getEmployeesByCompanyId?id=" + Helpers.Settings.DisplayUserCompany;

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


        public async Task<string> GetEmployeesAndManagers()
        {
            try
            {
                string uri = Constantes.ApiUrl + "user/employees-managers/" + Helpers.Settings.DisplayUserCompany;

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

        public async Task<string> EditCompanyTax(
            string taxPercentage,
            string recommendedTipPercentage,
            string prefix,
            bool creditCardAllowed,
            bool allowChatOnlyForCustomers,
            int type)
        {
            try
            {
                taxPercentage.Replace(',', '.');
                recommendedTipPercentage.Replace(',', '.');
                string uri = Constantes.ApiUrl +
                   "company/updateCompanyTax?companyId=" + Helpers.Settings.DisplayUserCompany +
                   "&taxPercentage=" + taxPercentage +
                   "&recommendedTipPercentage=" + recommendedTipPercentage +
                   "&prefix=" + prefix +
                   "&creditCardAllowed=" + creditCardAllowed +
                   "&allowChatOnlyForCustomers=" + allowChatOnlyForCustomers +
                   "&currency=" + type;

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


        public async Task<bool> IsCheckedIn(string companyId, string token)
        {
            try
            {
                string uri = Constantes.ApiUrl + "check/checkedIn?c=" + companyId;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync(uri);


                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var retorno = response.Content.ReadAsStringAsync().Result;
                    var convert = JsonConvert.DeserializeObject<bool>(retorno);

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

        public async Task<Checkin> IsCheckedInWithCheckin(string companyId, string token)
        {
            try
            {
                string uri = Constantes.ApiUrl + "check/checkedInWithLocation?c=" + companyId;
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

        public async Task<bool> IsCheckedInSub(string locationId, string token)
        {
            try
            {
                string uri = Constantes.ApiUrl + "check/checkedInSub?l=" + locationId;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync(uri);


                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var retorno = response.Content.ReadAsStringAsync().Result;
                    var convert = JsonConvert.DeserializeObject<bool>(retorno);

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

    public class RequestCheckin
    {
        public string CompanyId { get; set; }
        public string Occupation { get; set; }
        public string ClientId { get; set; }
        public string ClientQuantity { get; set; }

        public string AllocationNumber { get; set; }
        public string LocationId { get; set; }
    }
}
