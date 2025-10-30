using System;
using UnityEngine;

namespace GyeMong.UISystem.Social
{
    public class AccountContext
    {
        public static string ServerUrl = "https://www.monolong.shop:6203";
        public static string JwtTokenKey = "account-jwt";
        
        public static string JwtToken = null;
        
        public static PageType CurrentPage = PageType.AutoLogin;
        
        public static bool IsLoggedIn => !string.IsNullOrEmpty(JwtToken);

        public static bool IsLoading;
        
        public static string GetAuthorizationHeader()
        {
            return $"Bearer {JwtToken}";
        }

        public static void OnLogout()
        {
            JwtToken = null;
            CurrentPage = PageType.LOGIN;
            PlayerPrefs.SetString(JwtTokenKey, "");
        }
        
        public static void OnLogin(string jwt)
        {
            JwtToken = jwt;
            CurrentPage = PageType.MY;
            PlayerPrefs.SetString(JwtTokenKey, jwt);
        }
        
        public static void OnJoin(string jwt)
        {
            JwtToken = jwt;
            CurrentPage = PageType.MY;
            PlayerPrefs.SetString(JwtTokenKey, jwt);
        }
        
        public enum PageType
        {
            LOGIN,
            JOIN,
            MY,
            LOADING,
            AutoLogin
        }
    }
}