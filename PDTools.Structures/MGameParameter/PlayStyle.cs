using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.ComponentModel;

using PDTools.Utils;
using PDTools.Enums;
using System.IO;

namespace PDTools.Structures.MGameParameter;

public class PlayStyle
{
    /// <summary>
    /// Defaults to <see cref="BSpecType.BOTH_A_AND_B"/>.
    /// </summary>
    public BSpecType BSpecType { get; set; } = BSpecType.BOTH_A_AND_B;

    /// <summary>
    /// Defaults to <see cref="PlayType.RACE"/>.
    /// </summary>
    public PlayType PlayType { get; set; } = PlayType.RACE;

    /// <summary>
    /// Defaults to false.
    /// </summary>
    public bool NoQuickMenu { get; set; } = false;

    /// <summary>
    /// Defaults to false.
    /// </summary>
    public bool NoInstantReplay { get; set; } = false;

    /// <summary>
    /// Defaults to true.
    /// </summary>
    public bool ReplayRecordEnable { get; set; } = true;

    /// <summary>
    /// Defaults to false.
    /// </summary>
    public bool RentCarSettingEnable { get; set; }

    /// <summary>
    /// Window count in the screen, for split
    /// </summary>
    public int WindowNum { get; set; } = 1;

    /// <summary>
    /// For demo - Defaults to 0.
    /// </summary>
    public int TimeLimit { get; set; } = 0;

    /// <summary>
    /// For demo - Defaults to 0.
    /// </summary>
    public int LeaveLimit { get; set; } = 0;

    public bool IsDefault()
    {
        var defaultObj = new PlayStyle();
        return BSpecType == defaultObj.BSpecType &&
               PlayType == defaultObj.PlayType &&
               NoQuickMenu == defaultObj.NoQuickMenu &&
               NoInstantReplay == defaultObj.NoInstantReplay &&
               ReplayRecordEnable == defaultObj.ReplayRecordEnable &&
               RentCarSettingEnable == defaultObj.RentCarSettingEnable &&
               WindowNum == defaultObj.WindowNum &&
               TimeLimit == defaultObj.TimeLimit &&
               LeaveLimit == defaultObj.LeaveLimit;
    }

    public void CopyTo(PlayStyle other)
    {
        other.BSpecType = BSpecType;
        other.PlayType = PlayType;
        other.NoQuickMenu = NoQuickMenu;
        other.NoInstantReplay = NoInstantReplay;
        other.ReplayRecordEnable = ReplayRecordEnable;
        other.RentCarSettingEnable = RentCarSettingEnable;
        other.WindowNum = WindowNum;
        other.TimeLimit = TimeLimit;
        other.LeaveLimit = LeaveLimit;
    }

    public void WriteToXml(XmlWriter xml)
    {
        xml.WriteElementValue("bspec_type", BSpecType.ToString());
        xml.WriteElementValue("play_type", PlayType.ToString());
        xml.WriteElementBool("no_quickmenu", NoQuickMenu);
        xml.WriteElementBool("no_instant_replay", NoInstantReplay);
        xml.WriteElementBool("replay_record_enable", ReplayRecordEnable);
        xml.WriteElementBool("rentcar_setting_enable", RentCarSettingEnable);
        xml.WriteElementInt("window_num", WindowNum);
        xml.WriteElementInt("time_limit", TimeLimit);
        xml.WriteElementInt("leave_limit", LeaveLimit);
    }

    public void ParseFromXml(XmlNode node)
    {
        foreach (XmlNode pNode in node.ChildNodes)
        {
            switch (pNode.Name)
            {
                case "bspec_type":
                    BSpecType = pNode.ReadValueEnum<BSpecType>();
                    break;
                case "play_type":
                    PlayType = pNode.ReadValueEnum<PlayType>();
                    break;

                case "no_quickmenu":
                    NoQuickMenu = pNode.ReadValueBool();
                    break;
                case "no_instant_replay":
                    NoInstantReplay = pNode.ReadValueBool();
                    break;
                case "replay_record_enable":
                    ReplayRecordEnable = pNode.ReadValueBool();
                    break;
                case "rentcar_setting_enable":
                    RentCarSettingEnable = pNode.ReadValueBool();
                    break;

                case "window_num":
                    WindowNum = pNode.ReadValueInt();
                    break;

                case "time_limit":
                    TimeLimit = pNode.ReadValueInt();
                    break;

                case "leave_limit":
                    LeaveLimit = pNode.ReadValueInt();
                    break;
            }
        }
    }

    public void ReadFromCache(ref BitStream reader)
    {
        uint magic = reader.ReadUInt32();
        if (magic != 0xE5E516A7 && magic != 0xE6E616A7)
            throw new Exception($"PlayStyle did not match expected magic (0xE5E516A7 or 0xE6E616A7), got 0x{magic:X8}");

        uint playstyleVersion = reader.ReadUInt32();
        if (playstyleVersion < 102)
        {
            BSpecType = (BSpecType)reader.ReadInt32();
            PlayType = (PlayType)reader.ReadInt32();
            NoQuickMenu = reader.ReadBool();
            NoInstantReplay = reader.ReadBool();
            ReplayRecordEnable = reader.ReadBool();
            if (playstyleVersion == 101)
                RentCarSettingEnable = reader.ReadBool();
            WindowNum = reader.ReadInt32();
            TimeLimit = reader.ReadInt32();
            LeaveLimit = reader.ReadInt32();
        }
    }

    public void Serialize(ref BitStream bs)
    {
        bs.WriteUInt32(0xE6_E6_16_A7);
        bs.WriteUInt32(1_01);
        bs.WriteInt32((int)BSpecType);
        bs.WriteInt32((int)PlayType);
        bs.WriteBool(NoQuickMenu);
        bs.WriteBool(NoInstantReplay);
        bs.WriteBool(ReplayRecordEnable);
        bs.WriteBool(RentCarSettingEnable);
        bs.WriteInt32(WindowNum);
        bs.WriteInt32(TimeLimit);
        bs.WriteInt32(LeaveLimit);
    }
}
