// Helpers/Settings.cs
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace MocoApp.Helpers
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants

        private const string UserIdKey = "user_id";
        private static readonly string UserIdDefault = "";

        private const string UserNameKey = "user_name";
        private static readonly string UserNameDefault = string.Empty;

        private const string UserPhotoKey = "user_photo";
        private static readonly string UserPhotoDefault = "ic_avatar";

        private const string UserRoleKey = "user_role";
        private static readonly string UserRoleDefault = string.Empty;

        private const string UserCompanyKey = "user_company";
        private static readonly string UserCompanyDefault = string.Empty;

        private const string UserTokenKey = "user_token";
        private static readonly string UserTokenDefault = string.Empty;

        private const string CompanyHasLocationKey = "company_haslocation";
        private static readonly bool CompanyHasLocationDefault = false;

        private const string UserOffsetKey = "user_offset";
        private static readonly decimal UserOffsetDefault = 0;

        private const string UserIdiomKey = "user_idiom";
        private static readonly string UserIdiomDefault = "en-us";
        #endregion

        public static string DisplayUserIdiom
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(UserIdiomKey, UserIdiomDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(UserIdiomKey, value);
            }
        }


        public static bool DisplayHasLocation
        {
            get
            {
                return AppSettings.GetValueOrDefault<bool>(CompanyHasLocationKey, CompanyHasLocationDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<bool>(CompanyHasLocationKey, value);
            }
        }

        public static string DisplayUserId
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(UserIdKey, UserIdDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(UserIdKey, value);
            }
        }

        public static string DisplayUserToken
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(UserTokenKey, UserTokenDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(UserTokenKey, value);
            }
        }

        public static string DisplayUserRole
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(UserRoleKey, UserRoleDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(UserRoleKey, value);
            }
        }

        public static string DisplayUserPhoto
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(UserPhotoKey, UserPhotoDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(UserPhotoKey, value);
            }
        }

        public static string DisplayUserName
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(UserNameKey, UserNameDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(UserNameKey, value);
            }
        }

        public static string DisplayUserCompany
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(UserCompanyKey, UserCompanyDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(UserCompanyKey, value);
            }
        }

        public static decimal DisplayMyOffset
        {
            get
            {
                return AppSettings.GetValueOrDefault<decimal>(UserOffsetKey, UserOffsetDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<decimal>(UserOffsetKey, value);
            }
        }

    }
}