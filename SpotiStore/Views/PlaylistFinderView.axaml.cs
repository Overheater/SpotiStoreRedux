using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SpotiStore.Views
{
    public partial class PlaylistFinderView : UserControl
    {
        public PlaylistFinderView()
        {
            InitializeComponent();
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}