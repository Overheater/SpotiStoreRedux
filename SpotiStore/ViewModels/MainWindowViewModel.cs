using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using CsvHelper;
using SpotifyAPI.Web;

using SpotiStore.Models;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace SpotiStore.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel() {
            createSpotifyClient();
            PlaylistFinder = new PlaylistFinderViewModel(APIClient);
        }
        public PlaylistFinderViewModel PlaylistFinder { get; }
    }
}