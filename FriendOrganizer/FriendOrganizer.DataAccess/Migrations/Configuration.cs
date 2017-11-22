namespace FriendOrganizer.DataAccess.Migrations
{
    using FriendOrganizer.Model;
    using System;
    using System.Collections.Generic;
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

            context.Meetings.AddOrUpdate(m => m.Title,
            new Meeting
            {
                Title = "Watching Floorball",
                DateFrom = DateTime.Now.AddDays(1),
                DateTo = DateTime.Now.AddDays(8),
                Friends = new List<Friend>
                {
                    context.Friends.Single(f => f.FirstName == "Jacob" && f.LastName == "Gandis"),
                    context.Friends.Single(f => f.FirstName == "Anders" && f.LastName == "Svensson")
                }
            });

            context.Meetings.AddOrUpdate(m => m.Title,
            new Meeting
            {
                Title = "Learning WPF",
                DateFrom = DateTime.Now.AddDays(-1),
                DateTo = DateTime.Now.AddDays(5),
                Friends = new List<Friend>
                {
                    context.Friends.Single(f => f.FirstName == "Jacob" && f.LastName == "Gandis"),
                    context.Friends.Single(f => f.FirstName == "Samuel" && f.LastName == "Samuelsson")
                }
            });

            context.Meetings.AddOrUpdate(m => m.Title,
            new Meeting
            {
                Title = "Important Stuff",
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(12),
                Friends = new List<Friend>
                {
                    context.Friends.Single(f => f.FirstName == "Julia" && f.LastName == "Kvist"),
                    context.Friends.Single(f => f.FirstName == "Anders" && f.LastName == "Svensson")
                }
            });

        }
    }
}
