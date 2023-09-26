using ReactiveUI;
using SpotifyAPI.Web;
using SpotiStore.Services;
using System;

namespace SpotiStore.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {   
        public API APIClient;
        public bool CreateApiSession()
        {
            try
            {
                APIClient = new API();
                APIClient.CreateClient().GetAwaiter().GetResult();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}