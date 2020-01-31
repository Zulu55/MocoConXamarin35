using MocoApp.Models;
using MocoApp.Resources;
using MocoApp.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MocoApp.Models.Enums;

namespace MocoApp.Views.CompanyFluxo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateEditGroupPage : ContentPage
    {

        CategoryGroup Group;
        bool isEdit = false;
        CompanyService companyService = new CompanyService();
        string categoryId = "";
        EMenuType _type;
        public CreateEditGroupPage(CategoryGroup group = null, string _categoryId = "", EMenuType type = EMenuType.Undefined)
        {
            InitializeComponent();

            Group = group;
            categoryId = _categoryId;
            _type = type;

            if (Group == null)
            {
                LoadTotals();
                this.Title = AppResource.lblCreateGroup;
                swtActive.IsToggled = true;
            }
            else
            {
                isEdit = true;
                this.Title = AppResource.lblEditGroup;

                txtName.Text = Group.Name;
                swtActive.IsToggled = !Group.IsDisabled;
                txtPosition.Text = group.OrderingNumber.ToString();
                lblTotalPositions.Text += group.TotalItens;
            }

            this.BindingContext = Group;

        }
        public async void LoadTotals()
        {
            LocationService locationService = new LocationService();

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);

                var result = await locationService.GetTotalPositionsGroup(categoryId);
                var total = JsonConvert.DeserializeObject<int>(result);

                lblTotalPositions.Text += total;
                txtPosition.Text = (total + 1).ToString();
            }
            catch (Exception ex)
            {
                this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, ex.Message, AppResource.textOk);
            }
            finally
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }

        }
        private void btnSave_Clicked(object sender, EventArgs e)
        {
            if (!isEdit)
                Create();
            else
                Edit();
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {

        }

        public async void Create()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);



                CategoryGroup group = new CategoryGroup();


                group.Name = txtName.Text;
                group.IsDisabled = !swtActive.IsToggled;
                group.CompanyId = Helpers.Settings.DisplayUserCompany;
                group.CategoryId = categoryId;
                group.OrderingNumber = Convert.ToInt32(txtPosition.Text);
                group.MenuType = _type;

                var result = await companyService.CreateCategoryGroup(group);

                Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.lblItemCreatedSucess);
                await App.AppCurrent.NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, ex.Message, AppResource.textOk);

            }
            finally
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }

        }

        public async void Edit()
        {

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading(AppResource.alertLoading);
                

                Group.Name = txtName.Text;
                Group.IsDisabled = !swtActive.IsToggled;
                Group.OrderingNumber = Convert.ToInt32(txtPosition.Text); 

                var result = companyService.EditCategoryGroup(Group);

                Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.lblItemUpdatedSucess);

            }
            catch (Exception ex)
            {
                this.DisplayAlert(MocoApp.Resources.AppResource.alertAlert, ex.Message, AppResource.textOk);

            }
            finally
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }

        }

        private async void btnDelet_Clicked(object sender, EventArgs e)
        {
            var answer = await DisplayAlert(AppResource.alertDelete, AppResource.alertInfoDeleteCategory, AppResource.textOk, AppResource.alertCancel);

            if (!answer)
                return;


            try
            {
                ApiService service = new ApiService();

                await service.GetAsync("group/deletegroup?id=" + Group.Id);

                Acr.UserDialogs.UserDialogs.Instance.Toast(AppResource.alertItemDeletedSucess);

            }
            catch (Exception ex)
            {


            }
        }
    }
}