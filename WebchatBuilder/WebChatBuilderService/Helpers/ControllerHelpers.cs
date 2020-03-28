using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace WebChatBuilderService.Helpers
{
    public class AuthorizeIpAddressAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext context)
        {
            var syncValue = "";
            try
            {
                syncValue = WebChatBuilderModels.Shared.SyncKey.GetSyncValue();
                var trustedServer = context.Request.Headers.GetValues("Wcb-Sync").FirstOrDefault() == syncValue;
                if (!context.Request.IsLocal() || !trustedServer)
                {
                    throw new HttpException(403, "Forbidden");
                }
            }
            catch (Exception)
            {
                var response = context.Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Forbidden");
                context.Response = response;
                response.Headers.Add("Wcb-Sync", syncValue);
            }

            base.OnActionExecuting(context);
        }
    }
}
