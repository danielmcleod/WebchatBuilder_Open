using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebchatBuilder.Controllers;

namespace WebchatBuilder.Services
{
    public class GoogleAnalyticsServices
    {
        private static string googleURL = "http://www.google-analytics.com/collect";
        private static string googleVersion = "1";
        private static string googleCategory = "WCB";


        public static void TrackEvent(string action, string clientId, string ip = null, string ua = null, string profile = null)
        {
            try
            {
                if (ChatController.EnableGoogleAnalytics && !String.IsNullOrWhiteSpace(ChatController.GoogleAnalyticsTrackingId))
                {
                    var ht = new Hashtable();
                    ht.Add("v", googleVersion);                                 // Version.
                    ht.Add("tid", ChatController.GoogleAnalyticsTrackingId);    // Tracking ID / Web property / Property ID.
                    ht.Add("t", "event");                                       // Event hit type
                    ht.Add("ec", googleCategory);                               // Event Category. Required.
                    ht.Add("ea", action);                                       // Event Action. Required.
                    ht.Add("cid", clientId);                                    // Client Id. Required.
                    if (ip != null) ht.Add("uip", ip);                          // IP Override.
                    if (ua != null) ht.Add("ua", ua);                           // User Agent Override.
                    if (profile != null) ht.Add("el", "Profile: " + profile);   // Event label.

                    //PostData(ht);
                    Task.Run(() => PostData(ht));

                    //var data = new List<KeyValuePair<string, string>>
                    //{
                    //    new KeyValuePair<string, string>("v", googleVersion),
                    //    new KeyValuePair<string, string>("tid", ChatController.GoogleAnalyticsTrackingId),
                    //    new KeyValuePair<string, string>("t", "event"),
                    //    new KeyValuePair<string, string>("ec", googleCategory),
                    //    new KeyValuePair<string, string>("ea", action),
                    //    new KeyValuePair<string, string>("cid", clientId)
                    //};
                    //if (ip != null) data.Add(new KeyValuePair<string, string>("uip", ip));
                    //if (ua != null) data.Add(new KeyValuePair<string, string>("ua", ua));
                    //if (label != null) data.Add(new KeyValuePair<string, string>("el", label));
                    //if (value != null) data.Add(new KeyValuePair<string, string>("ev", value));
                    ////PostData(data);
                    //Task.Run(() => PostData(data));
                }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
            }
        }

        private static async Task PostData(Hashtable values)
        {
            var data = "";
            foreach (var key in values.Keys)
            {
                if (data != "") data += "&";
                if (values[key] != null) data += key.ToString() + "=" + HttpUtility.UrlEncode(values[key].ToString());
            }
            LoggingService.GetInstance().LogNote(data);

            using (var client = new WebClient())
            {
                var result = await client.UploadStringTaskAsync(googleURL, "POST", data);
                //LoggingService.GetInstance().LogToFile(result, ChatServices.FolderNameSafeDate(), true);
            }
        }

        //private static async void PostData(List<KeyValuePair<string, string>> values)
        //{
        //    HttpContent content = new FormUrlEncodedContent(values);
        //    using (var httpClient = new HttpClient())
        //    {
        //        HttpResponseMessage response = await httpClient.PostAsync(googleURL, content);
        //        //LoggingService.GetInstance().LogToFile(res, ChatServices.FolderNameSafeDate(), true);
        //    }
        //}
    }
}