using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LastFmSnarls
{
    public class Track
    {
        public string Artist { get; set; }
        public string Name { get; set; }
        public string Album { get; set; }
        public string AlbumArtSmall { get; set; }
        public string AlbumArtMedium { get; set; }
        public string AlbumArtLarge { get; set; }
        public string AlbumArtExtraLarge { get; set; }
        public string Link { get; set; }
        public bool NowPlaying { get; set; }

        public Track()
        {
            NowPlaying = false;
            Artist = "Unknown";
            Name = "Unknown";
            Album = "Unknown";
            Link = "";

        }

        public string getBestAlbumArt()
        {
            if (AlbumArtExtraLarge != null)
            {
                return AlbumArtExtraLarge;
            }
            else if (AlbumArtLarge != null)
            {
                return AlbumArtLarge;
            }
            else if (AlbumArtMedium != null)
            {
                return AlbumArtMedium;
            }
            else if (AlbumArtSmall != null)
            {
                return AlbumArtSmall;
            }
            else
            {
                return "";
            }
        }
    }
}
