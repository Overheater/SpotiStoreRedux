
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SpotiStore.Services
{
    public class API
    {
        /// <summary>
        /// the api key needed to access the spotify api
        /// </summary>
        private string ApiKey { get; set; }
        /// <summary>
        /// the Api Client that is passed to the Mainwindow
        /// </summary>
        public static SpotifyClient ApiClient { get; set; }

        public static async Task<SpotifyClient> CreateClient()
        {
            var config = SpotifyClientConfig.CreateDefault();
            var request = new ClientCredentialsRequest(APICredentials.ClientId, APICredentials.ClientSecret);
            var response = new OAuthClient(config).RequestToken(request).GetAwaiter().GetResult();
            try
            {
                var client = new SpotifyClient(config.WithToken(response.AccessToken));
                return client;
            }
            catch (Exception ex)
            {
                throw new Exception("Spotify Api Client could not be instatiated", ex);
                return null;
            }

        }
        public async Task<string> QueryPlaylist(string playlistId)
        {
            SpotifyAPI.Web.FullPlaylist playlist;
            try
            {
                playlist = await ApiClient.Playlists.Get(playlistId);
                return playlist.Name;

            }
            catch (Exception e)
            {

                return "No Playlist Found";
            }

        }

    }
}

