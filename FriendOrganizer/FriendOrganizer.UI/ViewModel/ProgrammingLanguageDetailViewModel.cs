using System;
using System.Threading.Tasks;
using FriendOrganizer.UI.View.Services;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
    public class ProgrammingLanguageDetailViewModel : DetailViewModelBase
    {
        public ProgrammingLanguageDetailViewModel(IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService) 
            : base(eventAggregator, messageDialogService)
        {
            Title = "Programming Languages";
        }

        public override Task LoadAsync(int id)
        {
            //todo load data here
            Id = id;
            return Task.Delay(0);
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
