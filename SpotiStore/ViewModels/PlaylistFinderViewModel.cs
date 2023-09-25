using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using CsvHelper;
using DynamicData;
using ReactiveUI;
using SpotifyAPI.Web;
using SpotiStore.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using static SpotifyAPI.Web.PlaylistRemoveItemsRequest;

namespace SpotiStore.ViewModels
{
    public class PlaylistFinderViewModel: ViewModelBase
    {
        /// <summary>
        /// the inputted ID for a spotify playlist query
        /// </summary>
        private string _playlistId  = string.Empty;

        private string _playlistName;

        private string _accountId = string.Empty;
        
        private string _accountName;

        private SpotifyClient _client;

        public string? RetrievedPlaylistName;
        /// <summary>
        /// the reactive Command handling the playlist search button handling
        /// </summary>
        public ReactiveCommand<Unit,System.Threading.Tasks.Task<string>> SearchCommand { get;}
        public ReactiveCommand<Unit, System.Threading.Tasks.Task<string>> SearchAccountCommand { get; }

        public ObservableCollection<string> PlaylistTracks { get; }
        public ObservableCollection<Tuple<string,string>> AccountPlaylists { get; }
        public ObservableCollection<Tuple<string, string>> SelectedPlaylists { get; } = new ObservableCollection<Tuple<string,string>>();

        public PlaylistFinderViewModel(SpotifyClient client)
        {
            PlaylistTracks = new ObservableCollection<string>();
            AccountPlaylists = new ObservableCollection<Tuple<string,string>>();
            var isValidPlaylistSearchQuery = this.WhenAnyValue(
                    q => q.PlaylistId,
                    q => !string.IsNullOrWhiteSpace(q));
            var isValidAccountSearchQuery = this.WhenAnyValue(
                    q => q.AccountId,
                    q => !string.IsNullOrEmpty(q));

            SearchCommand = ReactiveCommand.Create(
                    () => QueryPlaylist(), isValidPlaylistSearchQuery);
            SearchAccountCommand = ReactiveCommand.Create(
                () => QueryAccount(), isValidAccountSearchQuery);
            _client = client;
        }
        /// <summary>
        /// queries for the playlist using the inputted PlaylistId, and fills out the preview information for the user.
        /// </summary>
        /// <returns></returns>
        public async Task<string> QueryPlaylist()
        {
            SpotifyAPI.Web.FullPlaylist playlist;
            try
            {
                var previewTracks = new List<Song>();
                //TODO: create null playlist handling.
                playlist = await _client.Playlists.Get(PlaylistId);
                foreach (var playlistItem in playlist.Tracks.Items)
                {
                    previewTracks.Add(new Song(playlistItem));
                }
                UpdatePlaylistPreviewTracks(previewTracks);
                PlaylistName = playlist.Name;
                return playlist.Name;

            }
            catch (Exception e)
            {
                UpdatePlaylistPreviewTracks(null);

                PlaylistName = "No playlist found!";
                return "No Playlist Found";
            }

        }

        public async Task<string> QueryAccount()
        {
            SpotifyAPI.Web.PublicUser user;
            //pull user name
            try
            {
                var test = await _client.UserProfile.Get(AccountId);
                AccountName = test.DisplayName;
            }
            catch (Exception e)
            {

                AccountName = "No User Account was found!";
                return "No User Account was found!";
            }
            //pull the Id and Name of every playlist associated with the queried account
            try
            {
                var playlistInformation = new List<Tuple<string, string>>();
                var playlists = await _client.Playlists.GetUsers(AccountId);
                await foreach (var playlist in _client.Paginate(playlists))
                {
                    playlistInformation.Add(new Tuple<string, string>(playlist.Id, playlist.Name));
                }

                UpdateAccountPlaylists(playlistInformation);
                return null;

            }
            catch (Exception e)
            {
                return null;
            }
        }


        /// <summary>
        /// pulls the playlist data, creates a playlist model object, and converts the tracks to Song model objects
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ArchivePlaylist()
        {
            FullPlaylist spotifyPlaylist;
            Playlist playlist;
            try
            {
                 spotifyPlaylist = await _client.Playlists.Get(PlaylistId);
                 playlist = new Playlist(spotifyPlaylist);
            }
            catch (Exception e)
            {

                UpdatePlaylistPreviewTracks(null);

                PlaylistName = "The targeted playlist was not found!";
                //TODO: Add error handling here for when a user hasn't queried a playlist yet.

                return false;
            }

            await foreach (var item in _client.Paginate(spotifyPlaylist.Tracks))
            {
                playlist.AddPlaylistTrack(item);
            }
            var fileLocation = await GetPath(spotifyPlaylist.Name);
            if (fileLocation == "") return false;
            using (var writer = new StreamWriter(fileLocation))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<SongMap>();
                csv.WriteRecords(playlist.PlaylistSongs.Select(p => (Song)p));
            }
            return true;
        }

        /// <summary>
        /// prompts the user to choose a save location for the CSV backup
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetPath(string name)
        {

            if (Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Title = "Choose file name",
                    
                    // TODO: add the playlist name as a default name 
                };
                saveFileDialog.InitialFileName = Path.GetInvalidFileNameChars().Aggregate(name, (current, c) => current.Replace(c, '_'));
                saveFileDialog.Filters.Add(new FileDialogFilter { Name = "spreadsheets", Extensions = { "csv" } });
                var outPathStrings = await saveFileDialog.ShowAsync(desktop.MainWindow).ConfigureAwait(false);

                //var fileresult = task.Result;
                return String.Join(" ", outPathStrings);
            }

            return "";
        }


        public void UpdateAccountPlaylists(IEnumerable<Tuple<string,string>> playlists)
        {

            AccountPlaylists.Clear();
            if (playlists != null)
            {
                foreach (var playlist in playlists)
                {
                    AccountPlaylists.Add(playlist);
                }
            }
        }


        public void UpdatePlaylistPreviewTracks(IEnumerable<Song>? songs)
        {
            
            PlaylistTracks.Clear();
            if(songs != null)
            {
                foreach (var song in songs)
                {
                    PlaylistTracks.Add(song.Preview);
                }
            }
        }
        public string AccountName
        {
            get => _accountName;
            set => this.RaiseAndSetIfChanged(ref _accountName, value);
        }

        public string AccountId
        {
            get => _accountId;
            set => this.RaiseAndSetIfChanged(ref _accountId, value);
        }

        public string PlaylistId {
            get => _playlistId;
            set => this.RaiseAndSetIfChanged(ref _playlistId, value);
        }
        public string PlaylistName
        {
            get => _playlistName;
            set => this.RaiseAndSetIfChanged(ref _playlistName, value);
        }


    }
}
