using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotiStore.Models
{
    public sealed class SongMap : ClassMap<Song>
    {
        public SongMap()
        {
            Map(m => m.SongName);
            Map(m => m.SongArtist);
            Map(m => m.AlbumName);
            Map(m => m.ReleaseDate);
            Map(m => m.AddedDate);
            Map(m => m.SpotifySongID);
            Map(m => m.DiscogsLink);
        }
    }
}
