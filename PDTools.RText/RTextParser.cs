using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Syroot.BinaryData;

namespace PDTools.RText;

public class RTextParser
{
    public RTextBase RText { get; set; }

    public string LocaleCode { get; set; }

    public static readonly Dictionary<string, string> Locales = new()
    {
        { "BP", "Portuguese (Brazillian)" },
        { "CN", "Chinese (China)" },
        { "CZ", "Czech" },
        { "DK", "Danish" }, // Stubbed in GT6
        { "DE", "German" },
        { "EL", "Greek" },
        { "ES", "Spanish" },
        { "FI", "Finnish" }, // Stubbed in GT6
        { "FR", "French" },
        { "GB", "British" },
        { "HU", "Magyar (Hungary)" },
        { "IT", "Italian" },
        { "JP", "Japanese" },
        { "KR", "Korean" },
        { "MS", "Spanish (Mexican)" },
        { "NO", "Norwegian" },
        { "NL", "Dutch" },
        { "PL", "Polish" },
        { "PT", "Portuguese" },
        { "RU", "Russian" },
        { "SE", "Swedish" },
        { "TR", "Turkish" },
        { "TW", "Chinese (Taiwan)" },
        { "US", "American" }
    };

    public RTextParser()
    {
        
    }

    public void Read(byte[] data)
    {
        using var ms = new MemoryStream(data);
        using var reader = new BinaryStream(ms);
        switch (reader.ReadUInt32())
        {
            case 0x35305450: // RT03
                RText = new RT03();
                break;

            case 0x35305451: // RT04
                RText = new RT04();
                break;

            case 0x35305452: // RT05
                RText = new RT05();
                break;

            case 0x52543035: // 50TR
                try
                {
                    RText = new _50TR();
                    RText.Read(data);
                    return;
                }
                catch { }

                // Failed, try GT7
                RText = new _50TR(gt7: true);
                break;
            default:
                throw new InvalidDataException("Unknown header magic.");
        }

        RText.Read(data);
    }
}
