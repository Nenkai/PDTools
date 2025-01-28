using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Sound.PS2.MusicInf;

public class Playlist
{
    public string Name { get; set; }
    public List<Track> Tracks { get; set; } = [];
    public string TrackIndexesNames
    {
        get
        {
            if (Tracks.Count == 0)
                return $"<WARNING> : No tracks in playlist!";
            else if (Tracks.Count == 1)
                return "1 track";
            else
                return $"{Tracks.Count} tracks";

        }
    }

    public void AddTrack(Track track)
        => Tracks.Add(track);

    public void RemoveTrack(Track track)
        => Tracks.Remove(track);

    public override string ToString()
        => Name;
}
