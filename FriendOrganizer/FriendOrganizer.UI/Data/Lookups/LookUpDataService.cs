using FriendOrganizer.DataAccess;
using FriendOrganizer.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Lookups
{
    public class LookUpDataService : IFriendLookUpDataService, IProgrammingLanguageLookUpDataService
    {
        private Func<FriendOrganizerDbContext> _contextCreator;

        public LookUpDataService(Func<FriendOrganizerDbContext> contextCreator)
        {
            _contextCreator = contextCreator;
        }

        public async Task<IEnumerable<LookUpItem>> GetFriendLookUpAsync()
        {
            using (var ctx = _contextCreator())
            {
                return await ctx.Friends.AsNoTracking()
                    .Select(f =>
                    new LookUpItem
                    {
                        Id = f.Id,
                        //DisplayMember = $"{f.FirstName} {f.LastName}" Why no work? :(
                        DisplayMember = f.FirstName + " " + f.LastName
                    })
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<LookUpItem>> GetProgrammingLanguageLookUpAsync()
        {
            using (var ctx = _contextCreator())
            {
                return await ctx.ProgrammingLanguages.AsNoTracking()
                    .Select(f =>
                    new LookUpItem
                    {
                        Id = f.Id,
                        DisplayMember = f.Name
                    })
                    .ToListAsync();
            }
        }

    }
}
