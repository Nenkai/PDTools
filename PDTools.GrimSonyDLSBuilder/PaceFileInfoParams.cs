using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;
using System.Security.Cryptography;

namespace PDTools.GrimSonyDLSBuilder;

public class PaceFileInfoParams
{
    public ulong Size { get; set; }

    private string _name;
    public string Name
    {
        get => _name;
        set
        {
            if (value.Length > 63)
                throw new ArgumentException("Name too long (max 63 chars).");

            _name = value;
        }
    }

    private string _trackerUrl;
    public string TrackerUrl
    {
        get => _trackerUrl;
        set
        {
            if (value.Length > 511)
                throw new ArgumentException("Tracker URL too long (max 511 chars).");

            _trackerUrl = value;
        }
    }
}
