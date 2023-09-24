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