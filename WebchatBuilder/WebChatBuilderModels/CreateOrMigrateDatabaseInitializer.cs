using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebChatBuilderModels
{
    public class CreateOrMigrateDatabaseInitializer<TContext, TConfiguration> : IDatabaseInitializer<TContext>
        where TContext : DbContext
        where TConfiguration : DbMigrationsConfiguration<TContext>, new()
    {
        private readonly DbMigrationsConfiguration _configuration;
        public CreateOrMigrateDatabaseInitializer()
        {
            _configuration = new TConfiguration();
        }
        public CreateOrMigrateDatabaseInitializer(string connection)
        {
            Contract.Requires(!string.IsNullOrEmpty(connection), "connection");

            _configuration = new TConfiguration
            {
                TargetDatabase = new DbConnectionInfo(connection)
            };
        }
        void IDatabaseInitializer<TContext>.InitializeDatabase(TContext context)
        {
            Contract.Requires(context != null, "context");

            if (context.Database.Exists())
            {
                var shouldCreate = false;
                try
                {
                    if (!context.Database.CompatibleWithModel(throwIfNoMetadata: true))
                    {
                        try
                        {
                            shouldCreate = true;
                            if (!context.Database.CompatibleWithModel(throwIfNoMetadata: false))
                            {
                                shouldCreate = false;
                                Console.WriteLine("Updating Database...");
                                var migrator = new DbMigrator(_configuration);
                                migrator.Update();
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                }
                catch (Exception ex)
                {
                    shouldCreate = true;
                }
                if (shouldCreate)
                {
                    try
                    {
                        Console.WriteLine("Running Migrations...");
                        var migrator = new DbMigrator(_configuration);
                        foreach (var localMigration in migrator.GetLocalMigrations())
                        {
                            migrator.Update(localMigration);
                        }
                        Seed(context);
                        context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
            else
            {
                Console.WriteLine("Creating Database...");
                context.Database.Create();
                Seed(context);
                context.SaveChanges();
            }
        }

        protected virtual void Seed(TContext context)
        {
        }
    }
}