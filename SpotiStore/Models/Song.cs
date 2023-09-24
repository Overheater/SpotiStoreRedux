using Avalonia.Controls;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace SpotiStore.Models
{
    public class Song : PlaylistItem
    {
        public string SongName { get; set; }
        public string SongArtist { get; set; }
        //switched to string due to the limitations in CSVhelper
        //public List<Artist> Artist { get; set; }
        public Album SongAlbum { get; set; }
        public string AlbumName { get; set; }
        public string ReleaseDate { get; set; }
        public DateTime? AddedDate { get; set; }
        public string SpotifySongID { get; set; }
        public string Preview {  get; set; }
        public string DiscogsLink { get; set; }


        //TODO: Build out a check on the playlist constructor that makes sure the iplayable Item is a track.
        public Song(FullTrack track, DateTime? addedDate)
        {
            SongName = track.Name;
            SongArtist = GetArtists(track);
            SongAlbum = new Album(track.Album);
            AlbumName = track.Album.Name;
            ReleaseDate = SongAlbum.ReleaseDate;
            SpotifySongID = track.Id;
            Preview = $"{track.Name} - {track.Artists.First().Name}";
            AddedDate = addedDate;
            DiscogsLink = CreateDiscogsLink();

        }
        public Song(PlaylistTrack<IPlayableItem> playlistItem)
        {
            if(playlistItem.Track is FullTrack track)
            {
                SongName = track.Name;
                SongArtist = GetArtists(track);
                SongAlbum = new Album(track.Album);
                AlbumName = track.Album.Name;
                ReleaseDate = SongAlbum.ReleaseDate;
                SpotifySongID = track.Id;
                Preview = $"{track.Name} - {track.Artists.First().Name}";

                DiscogsLink = CreateDiscogsLink();
            }
        }

        private string CreateDiscogsLink()
        {
            StringBuilder link = new StringBuilder();
            link.Append("https://www.discogs.com/search/?type=release");
            var formattedAlbum = SongAlbum.Name.Replace(" ", "+");
            var formattedArtist = SongAlbum.ArtistName.Replace(" ", "+");
            if (formattedArtist == "Various+Artists")
            {
                link.AppendLine($"&title={formattedAlbum}");
                return link.ToString();
            }
            link.AppendLine($"&title={formattedAlbum}&artist={formattedArtist}");
            return link.ToString();
        }


        public string GetArtists(FullTrack track)
        {
            string ArtistList;
            if (track.Artists.Count > 1)
                ArtistList = string.Join(", ", track.Artists.Select(a => a.Name));
            else
                ArtistList = track.Artists[0].Name;
            return ArtistList;
        }
    }
}
