using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebChatBuilderModels;
using WebChatBuilderModels.Models;

namespace DatabaseDeployment
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Database.SetInitializer<Repository>(new RepositoryInitializer());
                using (var db = new Repository())
                {
                    db.Database.Initialize(true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            while (true)
            {
                Console.WriteLine("Type 1 and press Enter to deploy database or type exit to exit:");
                string line = Console.ReadLine();
                if (line == "exit")
                {
                    break;
                }
                if (line == "1")
                {
                    try
                    {
                        using (var repository = new Repository())
                        {
                            var load = repository.Profiles.FirstOrDefault();
                            if (load != null)
                            {
                                Console.WriteLine("Database loaded successfully");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
        }
    }
}
