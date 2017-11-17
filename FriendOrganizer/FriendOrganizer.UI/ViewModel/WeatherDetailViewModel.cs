﻿using APIXULib;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.View.Services;
using FriendOrganizer.UI.Wrapper;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public class WeatherDetailViewModel : DetailViewModelBase
    {
        //Replace this with your own key from https://www.apixu.com/ 
        private readonly string key = "412b3379a4c143f59d7115910170610";

        public WeatherDetailViewModel(IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService) 
            : base(eventAggregator, messageDialogService)
        {
            Title = "Weather Details";
            WeatherPropList = new ObservableCollection<WeatherModel>();
        }

        public WeatherModel WeatherProp { get; set; }

        public Current CurrentWeatherProp { get; set; }

        public ObservableCollection<WeatherModel> WeatherPropList { get; }

        public async override Task LoadAsync(int id)
        {
            IRepository repo = new Repository();
            var testWeatherModel = CreateTestWeatherModel();
            var cityWeatherResult = repo.GetWeatherData(key, GetBy.CityName, "goeteborg", Days.One);
            var localWeatherResult = repo.GetWeatherDataByAutoIP(key, Days.One);

            localWeatherResult = FixWeatherIconLink(localWeatherResult);

            WeatherProp = localWeatherResult;

            Id = id;

            await Task.FromResult(0);
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
                        icon = "https://cdn3.iconfinder.com/data/icons/weather-and-forecast/51/Weather_icons_grey-03-512.png"
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

        private static WeatherModel FixWeatherIconLink(WeatherModel weatherModel)
        {
            weatherModel.current.condition.icon = "http:" + weatherModel.current.condition.icon;
            return weatherModel;
        }

        protected override void OnDeleteExecute()
        {
            throw new NotImplementedException();
        }

        protected override bool OnSaveCanExecute()
        {
            throw new NotImplementedException();
        }

        protected override void OnSaveExecute()
        {
            throw new NotImplementedException();
        }
    }
}
