using ReactiveUI;
using SpotifyAPI.Web;
using SpotiStore.Services;
using System;

namespace SpotiStore.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {   
        public SpotifyClient APIClient;
        public bool createSpotifyClient()
        {
            try
            {
                var clientTask = API.CreateClient();
                APIClient = clientTask.GetAwaiter().GetResult();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}