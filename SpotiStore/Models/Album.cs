using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpotiStore.Models
{
    public class Album
    {
        public string Name { get; set; }
        public string ID { get; set; }

        public string ArtistName { get; set; }
        /// <summary>
        /// while many albums have the correct date, some lesser known albums only have a year, which seems to cause date parse functions to never return. currently set as a string
        /// </summary>
        //TODO: find a better date parse system to allow for year releases.
        public string ReleaseDate { get; set; }

        public Album(SimpleAlbum album)
        {
            Name = album.Name;
            ArtistName = album.Artists[0].Name;
            ID = album.Id;
            ReleaseDate = album.ReleaseDate;

        }
    }



}
