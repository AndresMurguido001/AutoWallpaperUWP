using AutoWallpaperUWP.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AutoWallpaper.Background.Models;

namespace AutoWallpaperUWP.ViewModels
{
    public class CollectionsViewModel : INotifyPropertyChanged
    {
        MainPage rootPage = MainPage.Current;
        private readonly WallpaperService unsplashApi = new WallpaperService();
        private ObservableCollection<Collection> sampleCollections;

        public event PropertyChangedEventHandler PropertyChanged;

        public CollectionsViewModel()
        {
            rootPage.IsLoading = true;
            sampleCollections = new ObservableCollection<Collection>();
        }

        public ObservableCollection<Collection> Collections
        {
            get
            {
                return this.sampleCollections;
            }

            set
            {
                sampleCollections = value;
                NotifyPropertyChanged();
            }
        }

        public IEnumerable<Collection> SelectedCollections
        {
            get
            {
                return this.sampleCollections.Where(c => c.Selected);
            }
        }


        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public async void InitializeCollections()
        {
            try
            {
                
                var result = await unsplashApi.ListCollections();

                Collections = result;
                rootPage.IsLoading = false;
                
            } catch (Exception e)
            {
                rootPage.DisplayError("Error!", e.Message);
            }
        }
    }
}
