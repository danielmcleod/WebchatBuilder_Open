using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using WebchatBuilder.Controllers;
using WebchatBuilder.Services;

namespace WebchatBuilder.Helpers
{
    public class AuthorizeIpAddressAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string ipAddress = HttpContext.Current.Request.UserHostAddress;
            LoggingService.GetInstance().LogNote("IPAddress of request: " + ipAddress);
            if (ipAddress == null || (ChatServices.BlockedIpAddresses.Any() && IsIpAddressBlocked(ipAddress)) || (!IsPrivateIpAddress(ipAddress)))
            {
                if (!IsIpAddressAllowed(ipAddress))
                {
                    context.Result = new HttpStatusCodeResult(403);
                }
            }

            base.OnActionExecuting(context);
        }

        private bool IsIpAddressBlocked(string ipAddress)
        {
            if (!string.IsNullOrWhiteSpace(ipAddress))
            {
                try
                {
                    return ChatServices.BlockedIpAddresses.Any(a => a.Trim().Equals(ipAddress, StringComparison.InvariantCultureIgnoreCase));
                }
                catch (Exception)
                {
                }
            }
            return true;
        }

        private bool IsIpAddressAllowed(string ipAddress)
        {
            if (!string.IsNullOrWhiteSpace(ipAddress))
            {
                try
                {
                    var anyAllowed = ChatServices.AllowedIpAddresses.Any(a => a.Trim().Equals(ipAddress, StringComparison.InvariantCultureIgnoreCase)) || ChatServices.AllowedIpAddresses.Any(a => a.Trim().Equals("*", StringComparison.InvariantCulture)) && HttpContext.Current.Request.IsSecureConnection;
                    return anyAllowed;
                }
                catch (Exception)
                {
                }
            }
            return false;
        }

        private static bool IsPrivateIpAddress(string ipAddress)
        {
            try
            {
                //Todo: check is allowed here too

                var ip = IPAddress.Parse(ipAddress);
                var octets = ip.GetAddressBytes();

                if (IPAddress.IsLoopback(ip))
                {
                    return true;
                }

                var is24BitBlock = octets[0] == 10;
                if (is24BitBlock) return true; // Return to prevent further processing

                var is20BitBlock = octets[0] == 172 && octets[1] >= 16 && octets[1] <= 31;
                if (is20BitBlock) return true; // Return to prevent further processing

                var is16BitBlock = octets[0] == 192 && octets[1] == 168;
                if (is16BitBlock) return true; // Return to prevent further processing

                var isLinkLocalAddress = octets[0] == 169 && octets[1] == 254;
                return isLinkLocalAddress;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public class AllowCrossDomainAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var request = HttpContext.Current.Request;

            if (!ChatServices.Licensed)
            {
                context.Result = new HttpStatusCodeResult(403);
            }

            bool hasOrigin = request.Headers.AllKeys.Contains("Origin");
            HttpContext.Current.Response.Headers.Remove("Access-Control-Allow-Origin");
            if (hasOrigin)
            {
                var origin = request.Headers.Get("Origin");
                if (ChatServices.AllowedDomains.Any(a => origin.ToLower().EndsWith(a.ToLower())))
                {
                    //HttpContext.Current.Response.Headers.Add("Access-Control-Allow-Headers", "WCB-SESSION-ID");
                    HttpContext.Current.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
                    //HttpContext.Current.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    HttpContext.Current.Response.Headers.Add("Access-Control-Allow-Origin", origin);
                    HttpContext.Current.Response.Headers.Add("Access-Control-Allow-Methods", "GET,POST,OPTIONS");
                }
            }

            if (!ChatController.Iframes())
            {
                string ipAddress = HttpContext.Current.Request.UserHostAddress;

                if (ipAddress == null || !IsPrivateIpAddress(ipAddress))
                {
                    if (!IsIpAddressAllowed(ipAddress))
                    {
                        HttpContext.Current.Response.Headers.Add("X-FRAME-OPTIONS", "DENY");
                    }
                }
            }

            base.OnActionExecuting(context);
        }

        private bool IsIpAddressAllowed(string ipAddress)
        {
            if (!String.IsNullOrWhiteSpace(ipAddress))
            {
                try
                {
                    var anyAllowed = ChatServices.AllowedIpAddresses.Any(a => a.Trim().Equals(ipAddress, StringComparison.InvariantCultureIgnoreCase)) || ChatServices.AllowedIpAddresses.Any(a => a.Trim().Equals("*", StringComparison.InvariantCulture)) && HttpContext.Current.Request.IsSecureConnection;
                    return anyAllowed;
                }
                catch (Exception)
                {
                }
            }
            return false;
        }

        private static bool IsPrivateIpAddress(string ipAddress)
        {
            try
            {
                //Todo: check is allowed here too

                var ip = IPAddress.Parse(ipAddress);
                var octets = ip.GetAddressBytes();

                if (IPAddress.IsLoopback(ip))
                {
                    return true;
                }

                var is24BitBlock = octets[0] == 10;
                if (is24BitBlock) return true; // Return to prevent further processing

                var is20BitBlock = octets[0] == 172 && octets[1] >= 16 && octets[1] <= 31;
                if (is20BitBlock) return true; // Return to prevent further processing

                var is16BitBlock = octets[0] == 192 && octets[1] == 168;
                if (is16BitBlock) return true; // Return to prevent further processing

                var isLinkLocalAddress = octets[0] == 169 && octets[1] == 254;
                return isLinkLocalAddress;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}