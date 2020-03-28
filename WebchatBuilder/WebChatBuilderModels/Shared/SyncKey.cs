using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebChatBuilderModels.Shared
{
    public static class SyncKey
    {
        public static string GetSyncValue()
        {
            var now = DateTime.UtcNow;
            var syncValue = (now.Month + now.Hour.ToString() + now.Year + now.Day);
            var bytes = Encoding.UTF8.GetBytes(syncValue);
            return Convert.ToBase64String(bytes);
        }
    }
}
