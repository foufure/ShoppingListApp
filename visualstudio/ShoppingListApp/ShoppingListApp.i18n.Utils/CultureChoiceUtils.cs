using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Web;
using ShoppingListApp.I18N.Resources;

namespace ShoppingListApp.I18N.Utils
{
    public static class CultureChoiceUtils
    {
        private const string UserCultureCookieName = "UserCultureCookie";
        private const string DefaultCultureMatch = "en-US";

        public static void SaveSupportedCultureChosenByUserInACookie(HttpCookieCollection cookies, string userCulture)
        {
            if (cookies != null && !string.IsNullOrEmpty(userCulture))
            {
                HttpCookie userCultureCookie = new HttpCookie(UserCultureCookieName, userCulture);
                userCultureCookie.Expires = DateTime.Now.AddYears(10);

                cookies.Set(userCultureCookie);
            }
            else 
            {
                throw new ArgumentOutOfRangeException("Internal Error: the cookies collection is null or the name of the user culture is null or empty. Please enter a valid cookie collection or user culture name", (Exception)null);
            }
        }

        public static void SetBestSupportedCultureMatchBetweenUserBrowserAndDefault(HttpRequest request)
        {
            if (request != null)
            {
                // If a culture was chosen explicitely by the user it will be taken automatically
                string culture = (request.Cookies[UserCultureCookieName] != null) ? request.Cookies[UserCultureCookieName].Value : null;

                // If the user has not chosen a culture explicitely, then the browser preferences will be taken if they contain supported cultures
                if (culture == null)
                {
                    // Cultures configured in the browser will be matched with the supported cultures and the best match will be chosen
                    if (request.UserLanguages != null && request.UserLanguages.Length > 0)
                    {
                        culture = BestCultureMatch(ApplicationCultures.SupportedCultures, request.UserLanguages);
                    }
                    else
                    {
                        // As last resort the default supported culture will be taken
                        culture = DefaultCultureMatch;
                    }
                }

                // Culture of the application background remains "en-US" for example for writing in the XML files for dates
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(DefaultCultureMatch);

                // Culture of the UI changes according to the preferences of the user
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(culture);
            }
            else 
            {
                throw new ArgumentOutOfRangeException("Internal Error: the HttpRequest is null. Please use a valid HttpRequest", (Exception)null);
            }
        }

        private static string BestCultureMatch(ReadOnlyDictionary<string, string> supportedCultures, string[] configuredBrowserCultures)
        {
            string bestMatch = DefaultCultureMatch;

            // Search for a Perfect Match between supported cultures and configured cultures in the browser
            KeyValuePair<int, string> perfectMatch = PerfectMatch(supportedCultures, configuredBrowserCultures);
            
            // Search for an Invariant Match ignoring the specific cultures settings
            KeyValuePair<int, string> bestInvariantMatch = InvariantMatch(supportedCultures, configuredBrowserCultures);

            if (perfectMatch.Key <= bestInvariantMatch.Key && perfectMatch.Value != null)
            {
                bestMatch = perfectMatch.Value;
            }

            if (bestInvariantMatch.Key < perfectMatch.Key && bestInvariantMatch.Value != null)
            {
                bestMatch = bestInvariantMatch.Value;
            }

            return bestMatch;
        }

        private static KeyValuePair<int, string> PerfectMatch(ReadOnlyDictionary<string, string> supportedCultures, string[] configuredBrowserCultures)
        {
            // Format of user-cultures: de, en-US;q=0.8 - weighting must be taken away
            return cultureMatch(supportedCultures, configuredBrowserCultures, (supportedCulture, browserCulture) => supportedCulture == browserCulture.Split(';')[0]);
        }

        private static KeyValuePair<int, string> InvariantMatch(ReadOnlyDictionary<string, string> supportedCultures, string[] configuredBrowserCultures)
        {
            // Format of user-cultures: de, en-US;q=0.8 - weighting and strong culture must be taken away
            return cultureMatch(supportedCultures, configuredBrowserCultures, (supportedCulture, browserCulture) => supportedCulture.Split('-')[0] == browserCulture.Split('-', ';')[0]);
        }

        private static KeyValuePair<int, string> cultureMatch(ReadOnlyDictionary<string, string> supportedCultures, string[] configuredBrowserCultures, Func<string, string, bool> CompareSupportedCultureAndConfiguredCultureForMatch)
        {
            // Position - Value
            KeyValuePair<int, string> match;
            int matchPosition = 0;

            foreach (string userCulture in configuredBrowserCultures)
            {
                foreach (string supportedCulture in supportedCultures.Values)
                { 
                    if (CompareSupportedCultureAndConfiguredCultureForMatch(supportedCulture, userCulture))
                    {
                        return match = new KeyValuePair<int, string>(matchPosition, supportedCulture);
                    }
                }

                matchPosition++;
            }

            return match = new KeyValuePair<int, string>(matchPosition, null);
        }
    }
}
