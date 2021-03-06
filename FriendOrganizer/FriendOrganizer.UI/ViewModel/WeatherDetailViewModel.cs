﻿using APIXULib;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.View.Services;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    public class WeatherDetailViewModel : DetailViewModelBase
    {
        //Please replace this with your own key from https://www.apixu.com/ 
        private readonly string key = "412b3379a4c143f59d7115910170610";

        public WeatherDetailViewModel(IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService) 
            : base(eventAggregator, messageDialogService)
        {
            Title = "Weather Details";
            UpdateWeatherCommand = new DelegateCommand(OnUpdateWeatherExecute);
        }

        public WeatherModel WeatherProp { get; set; }

        public ICommand UpdateWeatherCommand { get; }

        public async override Task LoadAsync(int id)
        {
            WeatherProp = await GetWeather();
            Id = id;

            await Task.CompletedTask;
        }

        private async Task<WeatherModel> GetWeather()
        {
            IRepository repo = new Repository();
            //var testWeatherModel = CreateTestWeatherModel();
            WeatherModel localWeatherResult = new WeatherModel();
            try
            {
                localWeatherResult = repo.GetWeatherDataByAutoIP(key, Days.One);
                localWeatherResult = FixWeatherIconLink(localWeatherResult);
                localWeatherResult = ConvertWeatherKmphToMps(localWeatherResult);
                //var cityWeatherResult = repo.GetWeatherData(key, GetBy.CityName, "goeteborg", Days.One);
            }
            catch (Exception)
            {
                await MessageDialogService.ShowInfoDialog($"The Weather data could not be loaded.");
            }
            
            return localWeatherResult;
        }

        private async void OnUpdateWeatherExecute()
        {
            WeatherProp = await GetWeather();
        }

        private WeatherModel ConvertWeatherKmphToMps(WeatherModel weatherModel)
        {
            weatherModel.current.wind_kph = Math.Round(weatherModel.current.wind_kph / 3.60, 2);
            return weatherModel;
        }

        private static WeatherModel FixWeatherIconLink(WeatherModel weatherModel)
        {
            weatherModel.current.condition.icon = "http:" + weatherModel.current.condition.icon;
            return weatherModel;
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
