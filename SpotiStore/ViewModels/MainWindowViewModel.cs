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
using SpotiStore.Services;

namespace SpotiStore.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel() {
            API APIClient;
            APIClient = new API();
            APIClient.CreateClient().GetAwaiter().GetResult();
            PlaylistFinder = new PlaylistFinderViewModel(APIClient);
        }
        public PlaylistFinderViewModel PlaylistFinder { get; }
    }
}