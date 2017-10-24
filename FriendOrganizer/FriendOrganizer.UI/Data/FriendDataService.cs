using FriendOrganizer.Model;
using System.Collections.Generic;

namespace FriendOrganizer.UI.Data
{
    public class FriendDataService : IFriendDataService
    {
        public IEnumerable<Friend> GetAll()
        {
            // TODO: Load data from database
            yield return new Friend { FirstName = "Jacob", LastName = "Gandis" };
            yield return new Friend { FirstName = "Anders", LastName = "Svensson" };
            yield return new Friend { FirstName = "Samuel", LastName = "Samuelsson" };
            yield return new Friend { FirstName = "Julia", LastName = "Assange" };
        }

    }
}
