using System;
using System.Threading.Tasks;
using FriendOrganizer.UI.View.Services;
using Prism.Events;
using FriendOrganizer.UI.Data.Repositories;
using System.Collections.ObjectModel;
using FriendOrganizer.UI.Wrapper;
using System.ComponentModel;
using Prism.Commands;
using System.Linq;
using System.Windows.Input;
using FriendOrganizer.Model;
using System.Collections.Generic;
using APIXULib;

namespace FriendOrganizer.UI.ViewModel
{
    public class ProgrammingLanguageDetailViewModel : DetailViewModelBase
    {
        //Replace this with your own key from https://www.apixu.com/ 
        private readonly string key = "412b3379a4c143f59d7115910170610";
        private IProgrammingLanguageRepository _programmingLanguageRepository;
        private ProgrammingLanguageWrapper _selectedProgrammingLanguage;

        public ProgrammingLanguageDetailViewModel(IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IProgrammingLanguageRepository programmingLanguageRepository) 
            : base(eventAggregator, messageDialogService)
        {
            _programmingLanguageRepository = programmingLanguageRepository;
            Title = "Programming Languages";
            ProgrammingLanguages = new ObservableCollection<ProgrammingLanguageWrapper>();

            AddCommand = new DelegateCommand(OnAddExecute);
            RemoveCommand = new DelegateCommand(OnRemoveExecute, OnRemoveCanExecute);
        }

        private bool OnRemoveCanExecute()
        {
            return SelectedProgrammingLanguage != null;
        }

        private async void OnRemoveExecute()
        {
            var isReferenced =
                await _programmingLanguageRepository.IsReferencedByFriendAsync(
                    SelectedProgrammingLanguage.Id);
            if (isReferenced)
            {
                await MessageDialogService.ShowInfoDialog($"The language {SelectedProgrammingLanguage.Name}" +
                  $" can't be removed, as it is referenced by at least one friend");
                return;
            }
            
            SelectedProgrammingLanguage.PropertyChanged -= Wrapper_PropertyChanged;
            _programmingLanguageRepository.Remove(SelectedProgrammingLanguage.Model);
            ProgrammingLanguages.Remove(SelectedProgrammingLanguage);
            SelectedProgrammingLanguage = null;
            HasChanges = _programmingLanguageRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private void OnAddExecute()
        {
            var wrapper = new ProgrammingLanguageWrapper(new ProgrammingLanguage());
            wrapper.PropertyChanged += Wrapper_PropertyChanged;
            _programmingLanguageRepository.Add(wrapper.Model);
            ProgrammingLanguages.Add(wrapper);

            // Trigger the validation
            wrapper.Name = "";
        }

        public ObservableCollection<ProgrammingLanguageWrapper> ProgrammingLanguages { get; }

        public WeatherModel WeatherProp { get; set; }

        public Current CurrentWeatherProp { get; set; }

        public ICommand AddCommand { get; }

        public ICommand RemoveCommand { get; }

        public ProgrammingLanguageWrapper SelectedProgrammingLanguage
        {
            get { return _selectedProgrammingLanguage; }
            set
            {
                _selectedProgrammingLanguage = value;
                OnPropertyChanged();
                ((DelegateCommand)RemoveCommand).RaiseCanExecuteChanged();
            }
        }

        public async override Task LoadAsync(int id)
        {
            IRepository repo = new Repository();
            var getCityForecastWeatherResult = repo.GetWeatherData(key, GetBy.CityName, "goeteborg", Days.One);

            //To make the icon a real http link
            getCityForecastWeatherResult.current.condition.icon = "http:" + getCityForecastWeatherResult.current.condition.icon;

            WeatherProp = getCityForecastWeatherResult;

            var testWeatherModel = new WeatherModel() {
                current = new Current()
                {
                    temp_c = 22.0,
                    condition = new Condition() { text = "Cloudy", icon = "https://cdn3.iconfinder.com/data/icons/weather-and-forecast/51/Weather_icons_grey-03-512.png" },
                    wind_kph = 5.0,
                    cloud = 420
                },
                location = new Location()
                {
                    country = "Sweden",
                    name = "My City"
                }
            };

            Id = id;

            foreach (var wrapper in ProgrammingLanguages)
            {
                wrapper.PropertyChanged -= Wrapper_PropertyChanged;
            }

            ProgrammingLanguages.Clear();

            var languages = await _programmingLanguageRepository.GetAllAsync();

            foreach (var model in languages)
            {
                var wrapper = new ProgrammingLanguageWrapper(model);
                wrapper.PropertyChanged += Wrapper_PropertyChanged;
                ProgrammingLanguages.Add(wrapper);
            }
        }

        private void Wrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = _programmingLanguageRepository.HasChanges();
            }
            if (e.PropertyName == nameof(ProgrammingLanguageWrapper.HasErrors))
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        protected override void OnDeleteExecute()
        {
            throw new NotImplementedException();
        }

        protected override bool OnSaveCanExecute()
        {
            return HasChanges && ProgrammingLanguages.All(p => !p.HasErrors);
        }

        protected async override void OnSaveExecute()
        {
            try
            {
                await _programmingLanguageRepository.SaveAsync();
                HasChanges = _programmingLanguageRepository.HasChanges();
                RaiseCollectionSavedEvent();
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                await MessageDialogService.ShowInfoDialog("Error while saving the entities, " +
                    "the data will be reloaded. Details: " + ex.Message);
                await LoadAsync(Id);
            }
            
        }
    }
}
