using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Web;
using ShoppingListApp.I18N.Resources;

namespace ShoppingListApp.I18N.Utils
{
    public static class CultureHelper
    {
        private const string WantedUserCultureCookieName = "WantedUserCultureCookie";

        public static void ApplyUserCulture(HttpRequest request)
        {
            string culture = null;

            if (request == null)
            {
                return;
            }

            // Verify if the user has selected himself a culture explicitely on the Website
            if ((culture = GetWantedUserCulture(request.Cookies)) == null)
            {
                // If not then take the preferred culture from the browser
                if (request.UserLanguages != null && request.UserLanguages.Length > 0)
                {
                    culture = BestCultureMatch(SupportedCultures.Cultures, request.UserLanguages);
                }
                else
                {
                    culture = "en-US";
                }
            }

            // Culture of the application background remains "en-US" for example for writing in the XML files for dates
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            // Culture of the UI changes according to the preferences of the user
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(culture);
        }

        public static void SetWantedUserCulture(HttpCookieCollection cookies, string userCulture)
        {
            if (userCulture != null)
            { 
                HttpCookie wantedUserCultureCookie = new HttpCookie(WantedUserCultureCookieName, userCulture);
                wantedUserCultureCookie.Expires = DateTime.Now.AddDays(30);
                if (cookies != null)
                {
                    cookies.Set(wantedUserCultureCookie);
                }
            }
        }

        private static string GetWantedUserCulture(HttpCookieCollection cookies)
        { 
            HttpCookie cookie = null;
            if ((cookie = cookies[WantedUserCultureCookieName]) != null)
            {
                return cookie.Value;
            }

            return null;
        }

        private static KeyValuePair<int, string> PerfectMatchPosition(ReadOnlyCollection<string> supportedCultures, string[] userCultures)
        {
            // Position - Value
            KeyValuePair<int, string> perfectMatch;
            int perfectMatchPosition = 0;

            foreach (string userCulture in userCultures)
            {
                foreach (string supportedCulture in supportedCultures)
                {
                    // Format of user-cultures: de, en-US;q=0.8 - weighting must be taken away
                    if (supportedCulture == userCulture.Split(';')[0])
                    {
                        return perfectMatch = new KeyValuePair<int, string>(perfectMatchPosition, supportedCulture);
                    }

                    perfectMatchPosition++;
                }
            }

            return perfectMatch = new KeyValuePair<int, string>(0, null);
        }

        private static string InvariantMatch(ReadOnlyCollection<string> supportedCultures, string[] userCultures)
        {
            string invariantMatch = null;

            foreach (string userCulture in userCultures)
            {
                foreach (string supportedCulture in supportedCultures)
                {
                    // Format of user-cultures: de, en-US;q=0.8 - weighting and strong culture must be taken away
                    if (supportedCulture.Split('-')[0] == userCulture.Split('-', ';')[0])
                    {
                        return invariantMatch = supportedCulture;
                    }
                }
            }

            return invariantMatch;
        }

        private static string BestMatch(KeyValuePair<int, string> perfectMatch, string invariantMatch)
        {
            string bestMatch = null;

            if (perfectMatch.Key == 0 && perfectMatch.Value != null)
            {
                bestMatch = perfectMatch.Value;
            }
            else 
            {
                if (invariantMatch != null)
                {
                    bestMatch = invariantMatch;
                }
                else 
                {
                    bestMatch = perfectMatch.Value;
                }
            }

            return bestMatch;
        }

        private static string BestCultureMatch(ReadOnlyCollection<string> supportedCultures, string[] userCultures)
        {
            string defaultCultureMatch = "en-US";

            // Search for Perfect Match
            KeyValuePair<int, string> perfectMatch = PerfectMatchPosition(supportedCultures, userCultures);

            // Search for Invariant Match
            string invariantBestMatch = InvariantMatch(supportedCultures, userCultures);

            // Compare Perfect and Invariant Match for Best Match
            string bestCultureMatch = BestMatch(perfectMatch, invariantBestMatch);

            return bestCultureMatch ?? defaultCultureMatch;
        }
    }
}
