using System;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using log4net;

namespace WebChatBuilderModels.Helper
{
    public class CustomEFInterceptor : IDbCommandInterceptor
    {
        private static readonly ILog Logger = LogManager.GetLogger("WCB");

        public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            LogInfo("NonQueryExecuted", String.Format(" IsAsync: {0}, Command Text: {1}", interceptionContext.IsAsync, command.CommandText));
        }

        public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            LogInfo("NonQueryExecuting", String.Format(" IsAsync: {0}, Command Text: {1}", interceptionContext.IsAsync, command.CommandText));
        }

        public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            LogInfo("ReaderExecuted", String.Format(" IsAsync: {0}, Command Text: {1}", interceptionContext.IsAsync, command.CommandText));
        }

        public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            LogInfo("ReaderExecuting", String.Format(" IsAsync: {0}, Command Text: {1}", interceptionContext.IsAsync, command.CommandText));
        }

        public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            LogInfo("ScalarExecuted", String.Format(" IsAsync: {0}, Command Text: {1}", interceptionContext.IsAsync, command.CommandText));
        }

        public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            LogInfo("ScalarExecuting", String.Format(" IsAsync: {0}, Command Text: {1}", interceptionContext.IsAsync, command.CommandText));
        }

        private void LogInfo(string command, string commandText)
        {
            Logger.Debug(String.Format("DB Command Intercepted on: {0} :- {1} ", command, commandText));
        }
    }
}
