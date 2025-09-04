using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using System.ComponentModel;

using PDTools.Utils;
using PDTools.Enums;
using PDTools.Enums.PS3;
using PDTools.Structures.PS3;

namespace PDTools.Structures.MGameParameter;

public class Reward
{
    /// <summary>
    /// Credit prizes for each rank.
    /// </summary>
    public List<int> PrizeTable { get; set; } = [];

    /// <summary>
    /// Point/XP prizes for each rank (GT5).
    /// </summary>
    public List<int> PointTable { get; set; } = [];

    /// <summary>
    /// Star tables requirements (GT6).
    /// </summary>
    public List<FinishResult> StarTable { get; set; } = [];

    /// <summary>
    /// Present rewards.
    /// </summary>
    public List<EventPresent> Present { get; set; } = [];

    /// <summary>
    /// Defaults to 0.
    /// </summary>
    public int SpecialRewardCode { get; set; } = 0;

    /// <summary>
    /// Whether prizes are cummulative - i.e finishing 1st will give all rewards across all positions, otherwise if false, just 1st rewards. Defaults to false.
    /// </summary>
    public bool PrizeType { get; set; } = false;

    /// <summary>
    /// Defaults to 0.
    /// </summary>
    public short PPBase { get; set; } = 0;

    /// <summary>
    /// Defaults to 0.
    /// </summary>
    public short PercentAtPP100 { get; set; } = 0;

    /// <summary>
    /// Rewards can only be obtained once. Defaults to false.
    /// </summary>
    public bool IsOnce { get; set; } = false;

    /// <summary>
    /// How presents should be given. Defaults to <see cref="RewardPresentType.ORDER"/>
    /// </summary>
    public RewardPresentType PresentType { get; set; } = RewardPresentType.ORDER;

    /// <summary>
    /// GT6 Only. Participation/Entry present rewards.
    /// </summary>
    public List<EventPresent> EntryPresent { get; set; } = [];

    /// <summary>
    /// GT6 Only. How presents should be given (participation/entry). Defaults to <see cref="RewardEntryPresentType.FINISH"/>.
    /// </summary>
    public RewardEntryPresentType EntryPresentType { get; set; } = RewardEntryPresentType.FINISH;

    /// <summary>
    /// Used for custom car presents.
    /// </summary>
    public EntryBase? TunedEntryPresent { get; set; }

    public void SetRewardPresent(int index, EventPresent present)
        => Present[index] = present;

    public void SetParticipatePresent(int index, EventPresent present)
        => EntryPresent[index] = present;

    public bool IsDefault()
    {
        var defaultReward = new Reward();
        return PrizeTable.Count == 0 &&
            PointTable.Count == 0 &&
            StarTable.Count == 0 &&
            Present.Count == 0 &&
            SpecialRewardCode == defaultReward.SpecialRewardCode &&
            PrizeType == defaultReward.PrizeType &&
            PPBase == defaultReward.PPBase &&
            PercentAtPP100 == defaultReward.PercentAtPP100 &&
            IsOnce == defaultReward.IsOnce &&
            PresentType == defaultReward.PresentType &&
            EntryPresent.Count == 0 &&
            EntryPresentType == defaultReward.EntryPresentType;
            // TODO TunedEntryPresent
    }

    public void CopyTo(Reward other)
    {
        for (int i = 0; i < PrizeTable.Count; i++)
            other.PrizeTable.Add(PrizeTable[i]);

        for (int i = 0; i < PointTable.Count; i++)
            other.PointTable.Add(PointTable[i]);

        for (int i = 0; i < StarTable.Count; i++)
            other.StarTable.Add(StarTable[i]);

        for (int i = 0; i < Present.Count; i++)
        {
            var present = new EventPresent();
            Present[i].CopyTo(present);
            other.Present.Add(present);
        }

        other.SpecialRewardCode = SpecialRewardCode;
        other.PrizeType = PrizeType;
        other.PPBase = PPBase;
        other.PercentAtPP100 = PercentAtPP100;
        other.IsOnce = IsOnce;
        other.PresentType = PresentType;
        other.EntryPresentType = EntryPresentType;

        if (other.TunedEntryPresent is null)
        {
            other.TunedEntryPresent = new EntryBase();
            TunedEntryPresent?.CopyTo(other.TunedEntryPresent);
        }
    }

    public void WriteToXml(XmlWriter xml)
    {
        xml.WriteStartElement("point_table");
        for (int i = 0; i < PointTable.Count; i++)
            xml.WriteElementInt("point", PointTable[i]);
        xml.WriteEndElement();
        
        xml.WriteStartElement("prize_table");
        for (int i = 0; i < PrizeTable.Count; i++)
           xml.WriteElementInt("prize", PrizeTable[i]);
        xml.WriteEndElement();
        
        xml.WriteStartElement("star_table");
        for (int i = 0; i < StarTable.Count; i++)
            xml.WriteElementValue("star", StarTable[i].ToString());
        xml.WriteEndElement();

        if (Present.Count > 0)
        {
            xml.WriteStartElement("present");
            foreach (var present in Present)
                present.WriteToXml(xml);
            xml.WriteEndElement();
        }
        
        xml.WriteElementInt("special_reward_code", SpecialRewardCode);
        xml.WriteElementBool("prize_type", PrizeType);
        xml.WriteElementInt("pp_base", PPBase);
        xml.WriteElementInt("percent_at_pp100", PercentAtPP100);
        xml.WriteElementBool("is_once", IsOnce);
        xml.WriteElementValue("present_type", PresentType.ToString());

        if (Present.Count > 0)
        {
            xml.WriteStartElement("entry_present");
            foreach (var present in Present)
                present.WriteToXml(xml);
            xml.WriteEndElement();
        }
        
        if (EntryPresentType != RewardEntryPresentType.FINISH)
            xml.WriteElementValue("entry_present_type", EntryPresentType.ToString());
        
        TunedEntryPresent?.WriteToXml(xml);
    }

    public void ParseFromXml( XmlNode node)
    {
        foreach (XmlNode rewardNode in node.ChildNodes)
        {
            switch (rewardNode.Name)
            {
                case "is_once":
                    IsOnce = rewardNode.ReadValueBool(); break;
                case "percent_at_pp100":
                    PercentAtPP100 = rewardNode.ReadValueShort(); break;
                case "pp_base":
                    PPBase = rewardNode.ReadValueShort(); break;

                case "present_type":
                    PresentType = rewardNode.ReadValueEnum<RewardPresentType>(); break;

                case "present":
                    {
                        var itemNodes = node.SelectNodes("item");
                        if (itemNodes is not null)
                        {
                            foreach (XmlNode presentNode in itemNodes)
                            {
                                var present = new EventPresent();
                                present.ParseFromXml(presentNode);
                                Present.Add(present);
                            }
                        }
                    }
                    break;

                case "entry_present_type":
                    EntryPresentType = rewardNode.ReadValueEnum<RewardEntryPresentType>(); break;

                case "entry_present":
                    {
                        var itemNodes = node.SelectNodes("item");
                        if (itemNodes is not null)
                        {
                            foreach (XmlNode presentNode in itemNodes)
                            {
                                var present = new EventPresent();
                                present.ParseFromXml(presentNode);
                                EntryPresent.Add(present);
                            }
                        }
                    }
                    break;

                case "prize_type":
                    PrizeType = rewardNode.ReadValueBool(); break;

                case "point_table":
                    {
                        var pointNodes = rewardNode.SelectNodes("point");
                        if (pointNodes is not null)
                        {
                            foreach (XmlNode pointNode in pointNodes)
                                PointTable.Add(pointNode.ReadValueInt());
                        }
                    }
                    break;

                case "prize_table":
                    {
                        var prizeNodes = rewardNode.SelectNodes("prize");
                        if (prizeNodes is not null)
                        {
                            foreach (XmlNode prizeNode in prizeNodes)
                                PrizeTable.Add(prizeNode.ReadValueInt());
                        }
                    }
                    break;

                case "star_table":
                    {
                        var starNodes = rewardNode.SelectNodes("star");
                        if (starNodes is not null)
                        {
                            foreach (XmlNode starNode in starNodes)
                                StarTable.Add(starNode.ReadValueEnum<FinishResult>());
                        }
                    }

                    break;

                case "entry_base":
                    TunedEntryPresent = new EntryBase();
                    TunedEntryPresent.ReadFromXml(rewardNode);
                    break;
            }
        }
    }

    public void Serialize(ref BitStream bs)
    {
        bs.WriteUInt32(0xE6_E6_A1_07);
        bs.WriteUInt32(1_03);

        bs.WriteInt32(PrizeTable.Count);
        for (int i = 0; i < PrizeTable.Count; i++)
            bs.WriteInt32(PrizeTable[i]);

        bs.WriteInt32(PointTable.Count);
        for (int i = 0; i < PointTable.Count; i++)
            bs.WriteInt32(PointTable[i]);

        bs.WriteInt32(StarTable.Count);
        for (int i = 0; i < StarTable.Count; i++)
            bs.WriteInt32((int)StarTable[i]);

        bs.WriteInt32(Present.Count);
        foreach (var present in Present)
            present.Serialize(ref bs);

        bs.WriteInt32(SpecialRewardCode);
        bs.WriteBool2(PrizeType);
        bs.WriteInt16(PPBase);
        bs.WriteInt16(PercentAtPP100);
        bs.WriteBool(IsOnce);
        bs.WriteBool(false); // unk field_0x4b

        bs.WriteInt32(EntryPresent.Count);
        foreach (var present in EntryPresent)
            present.Serialize(ref bs);

        bs.WriteByte((byte)EntryPresentType);

        EntryBase entryBaseReward = TunedEntryPresent ?? new EntryBase();
        entryBaseReward.Serialize(ref bs);
    }
}

public class EventPresent
{
    public GameItemType TypeID { get; set; }
    public GameItemCategory CategoryID { get; set; }
    public int Argument1 { get; set; } = 0;
    public int Argument2 { get; set; } = 0;
    public int Argument3 { get; set; } = 0;
    public int Argument4 { get; set; } = -1;

    // Only parsed as a blob in Seasonals Root
    public string? FName { get; set; }

    public void CopyTo(EventPresent other)
    {
        other.TypeID = TypeID;
        other.CategoryID = CategoryID;
        other.Argument1 = Argument1;
        other.Argument2 = Argument2;
        other.Argument3 = Argument3;
        other.Argument4 = Argument4;
        other.FName = FName;
    }

    public static EventPresent FromCar(string carLabel)
    {
        var present = new EventPresent();
        present.FName = carLabel;
        return present;
    }

    public static EventPresent FromCarParameter(MCarParameter carParameter)
    {
        throw new NotImplementedException("Implement this when MCarParameter serialization is completed");

        /*
        var present = new EventPresent();
        present.TypeID = GameItemType.SPECIAL;
        var data = carParameter.ExportToBlob();
        present.FName = Convert.ToBase64String(MiscUtils.ZlibCompress(data));
        return present;
        */
    }

    public static EventPresent FromPaint(int paintID)
    {
        var present = new EventPresent();
        present.TypeID = GameItemType.DRIVER_ITEM;
        present.Argument1 = paintID;
        return present;
    }

    public static EventPresent FromSuit(int suitID)
    {
        var present = new EventPresent();
        present.TypeID = GameItemType.NONE;
        present.Argument1 = 0;
        present.Argument4 = suitID;
        return present;
    }

    public void Serialize(ref BitStream bs)
    {
        bs.WriteUInt32(0x_E6_E6_D2_B3);
        bs.WriteUInt32(1_00);

        // type_id
        bs.WriteUInt32((uint)TypeID);
        bs.WriteInt32((int)CategoryID);
        bs.WriteInt32(Argument1);
        bs.WriteInt32(Argument2);
        bs.WriteInt32(Argument3);
        bs.WriteInt32(Argument4);
        bs.WriteNullStringAligned4(FName);

        // Blob Size
        bs.WriteInt32(0);
    }

    public void WriteToXml(XmlWriter xml)
    {
        xml.WriteStartElement("item");
        {
            xml.WriteAttributeString("type_id", TypeID.ToString());
            xml.WriteAttributeString("category_id", CategoryID.ToString());
            xml.WriteAttributeString("argument1", Argument1.ToString());
            xml.WriteAttributeString("argument2", Argument2.ToString());
            xml.WriteAttributeString("argument3", Argument3.ToString());
            xml.WriteAttributeString("argument4", Argument4.ToString());
            xml.WriteAttributeString("f_name", FName);
        }
        xml.WriteEndElement();
    }

    public void ParseFromXml(XmlNode node)
    {
        if (node.Attributes is null)
            return;

        foreach (XmlAttribute? attr in node.Attributes)
        {
            if (attr is null)
                continue;

            switch (attr.Name)
            {
                case "type_id":
                    if (int.TryParse(attr.Value, out int type_id))
                        TypeID = (GameItemType)type_id;
                    break;

                case "category_id":
                    if (int.TryParse(attr.Value, out int category_id))
                        CategoryID = (GameItemCategory)category_id;
                    break;

                case "argument1":
                    if (int.TryParse(attr.Value, out int value))
                        Argument1 = value;
                    break;
                case "argument2":
                    if (int.TryParse(attr.Value, out int value2))
                        Argument2 = value2;
                    break;
                case "argument3":
                    if (int.TryParse(attr.Value, out int value3))
                        Argument3 = value3;
                    break;
                case "argument4":
                    if (int.TryParse(attr.Value, out int value4))
                        Argument4 = value4;
                    break;

                case "f_name":
                    FName = attr.Value;
                    break;

            }
        }
    }
}


public enum LicenseResultType
{
    [Description("Result: Empty/None")]
    EMPTY,

    [Description("Result: Fail")]
    FAILURE,

    [Description("Result: Clear")]
    CLEAR,

    [Description("Result: Bronze")]
    BRONZE,

    [Description("Result: Silver")]
    SILVER,

    [Description("Result: Gold")]
    GOLD
}

public enum LicenseDisplayModeType
{
    [Description("None")]
    NONE,

    [Description("Pylon/Cone Time")]
    PYLON_TIME,

    [Description("Pylon/Cone Number")]
    PYLON_NUM,

    [Description("Fuel Distance")]
    FUEL_DIST,

    [Description("Fuel Time")]
    FUEL_TIME,

    [Description("Drift Score")]
    DRIFT_SCORE,
}

public enum LicenseConnectionType
{
    OR,
    AND,
    XOR
}

public enum LicenseConditionType
{
    [Description("Equal (==)")]
    EQUAL,

    [Description("Not Equal (!=)")]
    NOTEQUAL,

    [Description("Greater (>)")]
    GREATER,

    [Description("Less (<)")]
    LESS,

    [Description("Greater Equal (>=)")]
    GREATER_EQUAL,

    [Description("Less Equal (<=)")]
    LESS_EQUAL,
}

public enum LicenseCheckType
{
    [Description("Ranking")]
    RANK,

    [Description("Other Submode")]
    OTHER_SUBMODE,

    [Description("Total Time")]
    TOTAL_TIME,

    [Description("Lap Time")]
    LAP_TIME,

    [Description("Best Lap Time")]
    BEST_LAP_TIME,

    [Description("Lap Count")]
    LAP_COUNT,

    [Description("Velocity/Speed")]
    VELOCITY,

    [Description("VCoord")]
    V_POSITION,

    [Description("Gadget Hits")]
    GADGET_COUNT,

    [Description("Course Outs")]
    COURSE_OUT,

    [Description("Hit Count")]
    HIT_COUNT,

    [Description("Hit Power")]
    HIT_POWER,

    [Description("Wall Hits")]
    HIT_WALL,

    [Description("Fuel Amount")]
    FUEL_AMOUNT,

    [Description("Complete Flag")]
    COMPLETE_FLAG,

    [Description("Wrongway Count")]
    WRONG_WAY_COUNT,

    [Description("Road Distance")]
    ROAD_DISTANCE,

    [Description("Standing Time")]
    STANDING_TIME,

    [Description("Course-out Time")]
    COURSE_OUT_TIME,

    [Description("Fuel Capacity")]
    FUEL_CONSUMPTION,

    [Description("Floating Time")]
    FLOATING_TIME,
    ILLEGAL,
}

[Flags]
public enum FailCondition
{
    NONE,
    COURSE_OUT,
    HIT_WALL_HARD,
    HIT_CAR_HARD,
    HIT_CAR,
    PYLON,
    HIT_WALL,
    SPIN_FULL,
    SPIN_HALF,
    WHEEL_SPIN,
    LOCK_BRAKE,
    SLIP_ANGLE,
    LESS_SPEED,
    MORE_SPEED,
    MORE_GFORCE,
    PENALTY_ROAD,
    LOW_MU_ROAD,
    SLALOM,
    WRONGWAY,
    WRONGWAY_LOOSE,
    MAX,
}
