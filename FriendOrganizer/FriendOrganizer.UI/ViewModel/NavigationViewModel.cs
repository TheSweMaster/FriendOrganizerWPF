using FriendOrganizer.Model;
using FriendOrganizer.UI.Data;
using FriendOrganizer.UI.Event;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        private IFriendLookUpDataService _friendLookUpDataService;
        private IEventAggregator _eventAggregator;

        public ObservableCollection<LookUpItem> Friends { get; }

        public NavigationViewModel(IFriendLookUpDataService friendLookUpDataService, 
            IEventAggregator eventAggreator)
        {
            _friendLookUpDataService = friendLookUpDataService;
            _eventAggregator = eventAggreator;
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

        private LookUpItem _selectedFriend;

        public LookUpItem SelectedFriend
        {
            get { return _selectedFriend; }
            set
            {
                _selectedFriend = value;
                OnPropertyChanged();
                if (_selectedFriend != null)
                {
                    _eventAggregator.GetEvent<OpenFriendDetailViewEvent>()
                        .Publish(_selectedFriend.Id);
                }
            }
        }

    }
}
