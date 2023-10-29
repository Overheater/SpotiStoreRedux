using Avalonia.Controls;
using CsvHelper;
using ReactiveUI;
using SpotifyAPI.Web;
using SpotiStore.Models;
using SpotiStore.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace SpotiStore.ViewModels
{
    public class PlaylistFinderViewModel : ViewModelBase
    {
        /// <summary>
        /// the inputted ID for a spotify playlist query
        /// </summary>
        private string _playlistId = string.Empty;
        public string PlaylistId
        {
            get => _playlistId;
            set => this.RaiseAndSetIfChanged(ref _playlistId, value);
        }


        private string _playlistName;
        public string PlaylistName
        {
            get => _playlistName;
            set => this.RaiseAndSetIfChanged(ref _playlistName, value);
        }

        private string _accountId = string.Empty;
        public string AccountId
        {
            get => _accountId;
            set => this.RaiseAndSetIfChanged(ref _accountId, value);
        }


        private string _accountName;
        public string AccountName
        {
            get => _accountName;
            set => this.RaiseAndSetIfChanged(ref _accountName, value);
        }

        private API _client;

        public string? RetrievedPlaylistName;
        /// <summary>
        /// the reactive Command handling the playlist search button handling
        /// </summary>
        public ReactiveCommand<Unit, System.Threading.Tasks.Task<string>> SearchCommand { get; }
        /// <summary>
        /// the reactive Command handling the account search button handling
        /// </summary>
        public ReactiveCommand<Unit, System.Threading.Tasks.Task<string>> SearchAccountCommand { get; }

        public ObservableCollection<string> PlaylistTracks { get; }
        public ObservableCollection<Tuple<string, string>> AccountPlaylists { get; }
        public ObservableCollection<Tuple<string, string>> SelectedPlaylists { get; } = new ObservableCollection<Tuple<string, string>>();

        public PlaylistFinderViewModel(API client)
        {
            _client = client;
            PlaylistTracks = new ObservableCollection<string>();
            AccountPlaylists = new ObservableCollection<Tuple<string, string>>();
            var isValidPlaylistSearchQuery = this.WhenAnyValue(
                    q => q.PlaylistId,
                    q => !string.IsNullOrWhiteSpace(q));

            var isValidAccountSearchQuery = this.WhenAnyValue(
                    q => q.AccountId,
                    q => !string.IsNullOrWhiteSpace(q));

            SearchCommand = ReactiveCommand.Create(
                    () => QueryPlaylist(), isValidPlaylistSearchQuery);
            SearchAccountCommand = ReactiveCommand.Create(
                () => QueryAccount(), isValidAccountSearchQuery);
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
                playlist = await _client.QueryPlaylistAsync(PlaylistId);
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
            //pull user name
            AccountName = await _client.GetUserProfileNameAsync(AccountId);
            //pull the Id and name of every playlist associated with the queried account
            var playlistInfo = await _client.GetUserProfilePlaylistsAsync(AccountId);
            UpdateAccountPlaylists(playlistInfo);
            return "";
        }


        /// <summary>
        /// pulls the playlist data, creates a playlist model object, and converts the tracks to Song model objects
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ArchivePlaylist()
        {
            Playlist playlist;
            try
            {
                playlist = await _client.GetPlaylistAsync(PlaylistId);
            }
            catch (Exception e)
            {
                UpdatePlaylistPreviewTracks(null);
                PlaylistName = $"Playlist Archive Error! {e.Message}";
                return false;
            }
            var fileLocation = await FileIO.GetPath(playlist.PlaylistName);
            if (fileLocation == "") return false;
            using (var writer = new StreamWriter(fileLocation))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<SongMap>();
                csv.WriteRecords(playlist.PlaylistSongs.Select(p => (Song)p));
            }
            return true;
        }


        public async Task<bool> ArchivePlaylists()
        {
            foreach (var selectedPlaylist in SelectedPlaylists)
            {
                Playlist playlist;
                try
                {
                    playlist = await _client.GetPlaylistAsync(selectedPlaylist.Item1);
                }
                catch (Exception e)
                {
                    UpdatePlaylistPreviewTracks(null);
                    PlaylistName = $"Playlist Archive Error! {e.Message}";
                    return false;
                }
                var fileLocation = await FileIO.GetPath(playlist.PlaylistName);
                if (fileLocation == "") return false;
                using (var writer = new StreamWriter(fileLocation))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.Context.RegisterClassMap<SongMap>();
                    csv.WriteRecords(playlist.PlaylistSongs.Select(p => (Song)p));
                }
            }
            return true;
        }

        public void UpdateAccountPlaylists(IEnumerable<Tuple<string, string>> playlists)
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
            if (songs != null)
            {
                foreach (var song in songs)
                {
                    PlaylistTracks.Add(song.Preview);
                }
            }
        }

    }
}
