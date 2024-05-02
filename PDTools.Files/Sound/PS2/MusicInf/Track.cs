using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Sound.PS2.MusicInf;

public class Track
{
    public Playlist ParentPlaylist { get; set; }

    public int Index { get; set; }

    public string Code { get; set; }
    public string File { get; set; }
    public string Title { get; set; }
    public string Artist { get; set; }
    public string Genre { get; set; }
}
