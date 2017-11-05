namespace FriendOrganizer.DataAccess.Migrations
{
    using FriendOrganizer.Model;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FriendOrganizer.DataAccess.FriendOrganizerDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(FriendOrganizer.DataAccess.FriendOrganizerDbContext context)
        {
            //  This method will be called after migrating to the latest version.
            context.Friends.AddOrUpdate(
                f => f.FirstName,
                new Friend { FirstName = "Jacob", LastName = "Gandis" },
                new Friend { FirstName = "Anders", LastName = "Svensson" },
                new Friend { FirstName = "Samuel", LastName = "Samuelsson" },
                new Friend { FirstName = "Julia", LastName = "Kvist" },
                new Friend { FirstName = "Lisa", LastName = "Larsson" }
                );

            context.ProgrammingLanguages.AddOrUpdate(
                pl => pl.Name,
                new ProgrammingLanguage { Name = "C#" },
                new ProgrammingLanguage { Name = "TypeScript" },
                new ProgrammingLanguage { Name = "F#" },
                new ProgrammingLanguage { Name = "Swift" },
                new ProgrammingLanguage { Name = "Java" }
                );

            context.SaveChanges();

            context.FriendPhoneNumbers.AddOrUpdate(
                pn => pn.Number,
                new FriendPhoneNumber { Number = "+46 705123456", FriendId = context.Friends.First().Id });

        }
    }
}
