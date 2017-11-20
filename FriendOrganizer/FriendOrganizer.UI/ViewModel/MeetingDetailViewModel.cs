using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.View.Services;
using FriendOrganizer.UI.Wrapper;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriendOrganizer.Model;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;
using FriendOrganizer.UI.Event;
using APIXULib;

namespace FriendOrganizer.UI.ViewModel
{
    public class MeetingDetailViewModel : DetailViewModelBase, IMeetingDetailViewModel
    {
        //Please replace this with your own key from https://www.apixu.com/ 
        private readonly string key = "412b3379a4c143f59d7115910170610";
        private MeetingWrapper _meeting;
        private IMeetingRepository _meetingRepository;
        private Friend _selectedAvailableFriend;
        private Friend _selectedAddedFriend;
        private List<Friend> _allFriends;

        public MeetingDetailViewModel(IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IMeetingRepository meetingRepository) : base(eventAggregator, messageDialogService)
        {
            _meetingRepository = meetingRepository;
            eventAggregator.GetEvent<AfterDetailSavedEvent>().Subscribe(AfterDetailSaved);
            eventAggregator.GetEvent<AfterDetailDeletedEvent>().Subscribe(AfterDetailDeleted);

            WeatherModels = new ObservableCollection<WeatherDisplayModel>();
            AddedFriends = new ObservableCollection<Friend>();
            AvailableFriends = new ObservableCollection<Friend>();
            AddFriendCommand = new DelegateCommand(OnAddFriendExecute, OnAddFriendCanExecute);
            RemoveFriendCommand = new DelegateCommand(OnRemoveFriendExecute, OnRemoveFriendCanExecute);
        }

        public ObservableCollection<WeatherDisplayModel> WeatherModels { get; set; }
        public ObservableCollection<Friend> AddedFriends { get; }
        public ObservableCollection<Friend> AvailableFriends { get; }
        public ICommand AddFriendCommand { get; }
        public ICommand RemoveFriendCommand { get; }

        public MeetingWrapper Meeting
        {
            get { return _meeting; }
            private set
            {
                _meeting = value;
                OnPropertyChanged();
            }
        }

        public Friend SelectedAvailableFriend
        {
            get { return _selectedAvailableFriend; }
            set
            {
                _selectedAvailableFriend = value;
                OnPropertyChanged();
                ((DelegateCommand)AddFriendCommand).RaiseCanExecuteChanged();
            }
        }

        public Friend SelectedAddedFriend
        {
            get { return _selectedAddedFriend; }
            set
            {
                _selectedAddedFriend = value;
                OnPropertyChanged();
                ((DelegateCommand)RemoveFriendCommand).RaiseCanExecuteChanged();
            }
        }

        public override async Task LoadAsync(int meetingId)
        {
            var meeting = meetingId > 0
                ? await _meetingRepository.GetByIdAsync(meetingId)
                : CreateNewMeeting();

            GetWeather(meeting.DateFrom, meeting.DateTo);

            Id = meetingId;

            InitializeMeeting(meeting);

            _allFriends = await _meetingRepository.GetAllFriendsAsync();

            SetupPickList();
        }

        private async void GetWeather(DateTime dateFrom, DateTime dateTo)
        {
            IRepository repo = new Repository();
            var dateNow = DateTime.Today;
            int fromForecastDays = -1;
            int toForecastDays = -1;

            if (dateFrom.Date <= dateNow.AddDays(10).Date && dateFrom.Date >= dateNow.Date)
            {
                var timeSpan = dateFrom.Date - dateNow.Date;
                fromForecastDays = timeSpan.Days;
            }

            if (dateTo.Date <= dateNow.AddDays(10).Date && dateTo.Date >= dateNow.Date)
            {
                var timeSpan = dateTo.Date - dateNow.Date;
                toForecastDays = timeSpan.Days;
            }
            
            var localWeatherResult = new WeatherModel();
            try
            {
                localWeatherResult = repo.GetWeatherDataByAutoIP(key, Days.Ten);
                //var cityWeatherResult = repo.GetWeatherData(key, GetBy.CityName, "goeteborg", Days.One);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                await MessageDialogService.ShowInfoDialog($"The Weather data could not be loaded.");
                return;
            }

            WeatherDisplayModel weatherDisplayModelFromDate;
            if (fromForecastDays >= 0 && fromForecastDays <= 10)
            {
                weatherDisplayModelFromDate = new WeatherDisplayModel()
                {
                    Icon = "http:" + localWeatherResult.forecast.forecastday[fromForecastDays].day.condition.icon,
                    Text = $"Date: {localWeatherResult.forecast.forecastday[fromForecastDays].date} " +
                    $"Temp: Min {localWeatherResult.forecast.forecastday[fromForecastDays].day.mintemp_c}°C, " +
                    $"Max {localWeatherResult.forecast.forecastday[fromForecastDays].day.maxtemp_c}°C"
                };
            }
            else
            {
                weatherDisplayModelFromDate = new WeatherDisplayModel()
                {
                    Icon = "",
                    Text = $"No weather data can be found at this date..."
                };
            }

            WeatherDisplayModel weatherDisplayModelToDate;
            if (toForecastDays >= 0 && toForecastDays <= 10)
            {
                weatherDisplayModelToDate = new WeatherDisplayModel()
                {
                    Icon = "http:" + localWeatherResult.forecast.forecastday[toForecastDays].day.condition.icon,
                    Text = $"Date: {localWeatherResult.forecast.forecastday[toForecastDays].date} " +
                    $"Temp: Min {localWeatherResult.forecast.forecastday[toForecastDays].day.mintemp_c}°C, " +
                    $"Max {localWeatherResult.forecast.forecastday[toForecastDays].day.maxtemp_c}°C"
                };
            }
            else
            {
                weatherDisplayModelToDate = new WeatherDisplayModel()
                {
                    Icon = "",
                    Text = $"No weather data can be found at this date..."
                };
            }

            WeatherModels.Clear();
            WeatherModels.Add(weatherDisplayModelFromDate);
            WeatherModels.Add(weatherDisplayModelToDate);
        }

        private WeatherModel CreateTestWeatherModel()
        {
            return new WeatherModel()
            {
                current = new Current()
                {
                    temp_c = 22.0,
                    condition = new Condition()
                    {
                        text = "Cloudy",
                        icon = "http://cdn.apixu.com/weather/64x64/day/302.png"
                    },
                    wind_kph = 5.0,
                    cloud = 420
                },
                location = new Location()
                {
                    country = "Sweden",
                    name = "My City"
                }
            };
        }

        private void SetupPickList()
        {
            var meetingFriendIds = Meeting.Model.Friends.Select(f => f.Id).ToList();
            var addedFriends = _allFriends.Where(f => meetingFriendIds.Contains(f.Id)).OrderBy(f => f.FirstName);
            var availableFriends = _allFriends.Except(addedFriends).OrderBy(f => f.FirstName);

            AddedFriends.Clear();
            AvailableFriends.Clear();
            foreach (var addedFriend in addedFriends)
            {
                AddedFriends.Add(addedFriend);
            }
            foreach (var availableFriend in availableFriends)
            {
                AvailableFriends.Add(availableFriend);
            }
        }

        private void InitializeMeeting(Meeting meeting)
        {
            Meeting = new MeetingWrapper(meeting);
            Meeting.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _meetingRepository.HasChanges();
                }

                if (e.PropertyName == nameof(Meeting.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
                if (e.PropertyName == nameof(Meeting.Title))
                {
                    SetTitle();
                }
            };
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();

            if (Meeting.Id == 0)
            {
                // Little trick to trigger the validation
                Meeting.Title = "";
            }
            SetTitle();
        }

        private void SetTitle()
        {
            Title = Meeting.Title;
        }

        private Meeting CreateNewMeeting()
        {
            var meeting = new Meeting
            {
                DateFrom = DateTime.Now.Date,
                DateTo = DateTime.Now.Date
            };
            _meetingRepository.Add(meeting);
            return meeting;
        }

        protected async override void OnDeleteExecute()
        {
            var result = await MessageDialogService.ShowOkCancelDialogAsync($"Do you really want to delete the meeting {Meeting.Title}?", "Question");
            if (result == MessageDialogResult.Ok)
            {
                _meetingRepository.Remove(Meeting.Model);
                await _meetingRepository.SaveAsync();
                RaiseDetailDeletedEvent(Meeting.Id);
            }
        }

        protected override bool OnSaveCanExecute()
        {
            return Meeting != null && !Meeting.HasErrors && HasChanges;
        }

        protected override async void OnSaveExecute()
        {
            await _meetingRepository.SaveAsync();
            HasChanges = _meetingRepository.HasChanges();
            Id = Meeting.Id;
            RaiseDetailSavedEvent(Meeting.Id, Meeting.Title);
            //Updates weather when saving
            GetWeather(Meeting.Model.DateFrom, Meeting.Model.DateTo);
        }

        private bool OnRemoveFriendCanExecute()
        {
            return SelectedAddedFriend != null;
        }

        private bool OnAddFriendCanExecute()
        {
            return SelectedAvailableFriend != null;
        }

        private void OnRemoveFriendExecute()
        {
            var friendToRemove = SelectedAddedFriend;

            Meeting.Model.Friends.Remove(friendToRemove);
            AddedFriends.Remove(friendToRemove);
            AvailableFriends.Add(friendToRemove);
            HasChanges = _meetingRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private void OnAddFriendExecute()
        {
            var friendToAdd = SelectedAvailableFriend;

            Meeting.Model.Friends.Add(friendToAdd);
            AddedFriends.Add(friendToAdd);
            AvailableFriends.Remove(friendToAdd);
            HasChanges = _meetingRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private async void AfterDetailSaved(AfterDetailSavedEventArgs args)
        {
            if (args.ViewModelName == nameof(FriendDetailViewModel))
            {
                await  _meetingRepository.ReloadFriendAsync(args.Id);
                _allFriends = await _meetingRepository.GetAllFriendsAsync();
                SetupPickList();
            }
        }

        private async void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            if (args.ViewModelName == nameof(FriendDetailViewModel))
            {
                _allFriends = await _meetingRepository.GetAllFriendsAsync();
                SetupPickList();
            }
        }

    }
}
