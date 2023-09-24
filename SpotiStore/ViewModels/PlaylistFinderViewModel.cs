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

namespace SpotiStore.ViewModels
{
    public class PlaylistFinderViewModel: ViewModelBase
    {
        /// <summary>
        /// the inputted ID for a spotify playlist query
        /// </summary>
        private string _playlistId  = string.Empty;
        private string _playlistName;
        private SpotifyClient _client;
        /// <summary>
        /// the name of the retrieved playlist
        /// </summary>
        public string? RetrievedPlaylistName;
        /// <summary>
        /// the reactive Command handling the playlist search button handling
        /// </summary>
        public ReactiveCommand<Unit,System.Threading.Tasks.Task<string>> SearchCommand { get;}
        public ObservableCollection<string> PlaylistTracks { get; }

        public PlaylistFinderViewModel(SpotifyClient client)
        {
            PlaylistTracks = new ObservableCollection<string>();
            var isValidSearchQuery = this.WhenAnyValue(
                    q => q.PlaylistId,
                    q => !string.IsNullOrWhiteSpace(q));
            SearchCommand = ReactiveCommand.Create(
                    () => QueryPlaylist(), isValidSearchQuery);
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

        /// <summary>
        /// pulls the playlist data, creates a playlist model object, and converts the tracks to Song model objects
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ArchivePlaylist()
        {
            if (PlaylistId == null)
            {
                //TODO: Add error handling here for when a user hasn't queried a playlist yet.

                return false;
            }
            var spotifyPlaylist = await _client.Playlists.Get(PlaylistId);
            var playlist = new Playlist(spotifyPlaylist);
            await foreach (var item in _client.Paginate(spotifyPlaylist.Tracks))
            {
                playlist.AddPlaylistTrack(item);
            }
            var fileLocation = await GetPath();
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
        public async Task<string> GetPath()
        {

            if (Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Title = "Choose file name",
                    // TODO: add the playlist name as a default name 
                };
                saveFileDialog.Filters.Add(new FileDialogFilter { Name = "spreadsheets", Extensions = { "csv" } });
                var outPathStrings = await saveFileDialog.ShowAsync(desktop.MainWindow).ConfigureAwait(false);

                //var fileresult = task.Result;
                return String.Join(" ", outPathStrings);
            }

            return "";
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
