// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

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
                // GT7 uses 0x18-byte page entries with 64-bit offsets: {name i64, pair count, unused,
                // pairs i64}. Read with GT Sport's 0x10-byte u32 entries, 0x24 is then the upper half
                // of GT7's name offset and 0x2C its unused dword, both 0 - but 0x2C is GT Sport's pair
                // offset for page 0, which can never be 0. Without this check the GT Sport layout can
                // "succeed" on a small GT7 file and yield garbage page names and no strings.
                if (data.Length >= 0x30 && BitConverter.ToUInt32(data, 0x24) == 0 && BitConverter.ToUInt32(data, 0x2C) == 0)
                {
                    RText = new _50TR(gt7: true);
                    break;
                }

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
