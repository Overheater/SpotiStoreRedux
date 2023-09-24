using Avalonia;
using DynamicData;
using ReactiveUI;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            var isValidSearchQuery = this.WhenAnyValue(
                    q => q.PlaylistId,
                    q => !string.IsNullOrWhiteSpace(q));
            SearchCommand = ReactiveCommand.Create(
                    () => QueryPlaylist(), isValidSearchQuery);
            _client = client;
        }

        public async Task<string> QueryPlaylist()
        {
            SpotifyAPI.Web.FullPlaylist playlist;
            try
            {
                playlist = await _client.Playlists.Get(PlaylistId);
                PlaylistName = playlist.Name;



                return playlist.Name;

            }
            catch (Exception e)
            {

                return "No Playlist Found";
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




        public void UpdatePlaylistTracks(IEnumerable<string> tracks)
        {
            PlaylistTracks.Clear();
            PlaylistTracks.Add(tracks);
        }

    }
}
