using System;
using Engine;
using Gui.Core;

namespace Gui.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {

        public RelayCommand HomeViewCommand { get; set; }

        public RelayCommand UploadContentCommand { get; set; }

        public RelayCommand AboutCommand { get; set; }


        public HomepageViewModel HomepageVM { get; set; }

        public UploadContentViewModel UploadContentVM { get; set; }

        public AboutViewModel AboutVM { get; set; }


        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set {
                _currentView = value;
                OnPropertyChanged();
            }
        }


        public MainViewModel()
        {
            HomepageVM = new HomepageViewModel();
            UploadContentVM = new UploadContentViewModel();
            AboutVM = new AboutViewModel();

            CurrentView = HomepageVM;

            HomeViewCommand = new RelayCommand(o =>
            {
                CurrentView = HomepageVM;
            });

            UploadContentCommand = new RelayCommand(O =>
            {
               CurrentView = UploadContentVM;
            });

            AboutCommand = new RelayCommand(o =>
            {
                CurrentView = AboutVM;
            });
            
            Connector.GenerateDb();
        }
    }
}
