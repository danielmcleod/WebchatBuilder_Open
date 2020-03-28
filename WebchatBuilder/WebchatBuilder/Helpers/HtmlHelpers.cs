using System.Web.Mvc;

namespace WebchatBuilder.Helpers
{
    public static class HtmlHelpers
    {
        public static bool IsDebug(this HtmlHelper htmlHelper)
        {
#if DEBUG
            return true;
#else
      return false;
#endif
        }
    }
}