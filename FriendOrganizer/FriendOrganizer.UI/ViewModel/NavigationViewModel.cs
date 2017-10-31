using FriendOrganizer.Model;
using FriendOrganizer.UI.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public class NavigationViewModel : INavigationViewModel
    {
        private IFriendLookUpDataService _friendLookUpDataService;

        public ObservableCollection<LookUpItem> Friends { get; }

        public NavigationViewModel(IFriendLookUpDataService friendLookUpDataService)
        {
            _friendLookUpDataService = friendLookUpDataService;
            Friends = new ObservableCollection<LookUpItem>();
        }

        public async Task LoadAsync()
        {
            var lookup = await _friendLookUpDataService.GetFriendLookUpAsync();
            Friends.Clear();
            foreach (var item in lookup)
            {
                Friends.Add(item);
            }
        }

    }
}
