
using SpotifyAPI.Web;
using SpotiStore.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpotiStore.Services
{
    public class API
    {

        /// <summary>
        /// the Api Client that is passed to the Mainwindow
        /// </summary>
        public  SpotifyClient ApiClient { get; set; }
        public API()
        {
            ApiClient = null;
        }
        public async Task<SpotifyClient> CreateClient()
        {
            var config = SpotifyClientConfig.CreateDefault();
            var request = new ClientCredentialsRequest(APICredentials.ClientId, APICredentials.ClientSecret);
            try
            {
                var response =  new OAuthClient(config).RequestToken(request).GetAwaiter().GetResult();
                var client = new SpotifyClient(config.WithToken(response.AccessToken));
                ApiClient = client;
                return client;
            }
            catch (Exception ex)
            {
                throw new Exception("Spotify Api Client could not be instatiated", ex);
            }

        }

        public async Task<Playlist> GetPlaylistAsync(string playlistId)
        {
            FullPlaylist fullPlaylist;
            Playlist playlist;
            //attempt to get the playlist
            try
            {
                fullPlaylist = await ApiClient.Playlists.Get(playlistId);
                playlist = new Playlist(fullPlaylist);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not access Playlist via ID", ex);
            }
            //for each page of tracks (50 per page), attempt to add the track to the Playlist object we created above.
            try
            {
                await foreach (var item in ApiClient.Paginate(fullPlaylist.Tracks))
                {
                    playlist.AddPlaylistTrack(item);
                }
            }
            catch (Exception ex)
            {

                throw new Exception($"Could not access the full playlist tracklist for {fullPlaylist.Name}",ex);
            }
            return playlist;
        }

        public async Task<FullPlaylist> QueryPlaylistAsync(string playlistId)
        {
            SpotifyAPI.Web.FullPlaylist playlist;
            try
            {
                playlist = await ApiClient.Playlists.Get(playlistId);
                return playlist;

            }
            catch (Exception ex)
            {

                //log error
                throw ex;
            }

        }


        public async Task<string> GetUserProfileNameAsync(string accountId)
        {
            try
            {
                var profile = await ApiClient.UserProfile.Get(accountId);
                return profile.DisplayName;
            }
            catch (Exception ex)
            {

                return "No User Account was found!";
            }

            
        }
        /// <summary>
        /// gets the name and ID of each playlist associated with a user profile.
        /// </summary>
        /// <param name="accountId"> the User Profile ID </param>
        /// <returns>a list of tuples, where the first item is the playlist ID and the second item is the playlist name</returns>
        internal async Task<List<Tuple<string,string>>> GetUserProfilePlaylistsAsync(string accountId)
        {
            try
            {
                var playlistInformation = new List<Tuple<string, string>>();
                var playlists = await ApiClient.Playlists.GetUsers(accountId);
                await foreach (var playlist in ApiClient.Paginate(playlists))
                {
                    playlistInformation.Add(new Tuple<string, string>(playlist.Id, playlist.Name));
                }
                return playlistInformation;
            }
            catch (Exception ex)
            {

                return new List<Tuple<string, string>>();
            }
        }
    }
}

