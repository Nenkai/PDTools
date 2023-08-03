using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using System.IO;

using Syroot.BinaryData;

using PDTools.Utils;
using PDTools.Enums;
using System.Text;

namespace PDTools.Structures.MGameParameter
{
    public class GameParameter
    {
        public ulong FolderId { get; set; }
        public List<Event> Events { get; } = new List<Event>();
        public OnlineRoomParameter OnlineRoom { get; } = new OnlineRoomParameter();
        public Reward SeriesRewards { get; } = new Reward();
        public Information SeriesInformation { get;  } = new Information();
        public EditorInfo EditorInfo { get; } = new EditorInfo();
        public Event Event
        {
            get
            {
                if (Events.Count == 0)
                    Events.Add(new Event());

                return Events[0];
            }
        }

        public bool Championship { get; set; }
        public bool Arcade { get; set; }

        public const int XMLVersionLatest = 106;

        /// <summary>
        /// Writes the structure to XML. Note: Unsorted XML nodes.
        /// </summary>
        /// <param name="xml"></param>
        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteStartElement("GameParameter"); xml.WriteAttributeString("version", XMLVersionLatest.ToString());
            xml.WriteElementULong("folder_id", FolderId);

            xml.WriteStartElement("events");
            foreach (var evnt in Events)
                evnt.WriteToXml(xml);
            xml.WriteEndElement();

            // online_room

            if (!SeriesRewards.IsDefault())
            {
                xml.WriteStartElement("series_reward");
                SeriesRewards.WriteToXml(xml);
                xml.WriteEndElement();
            }

            if (!SeriesInformation.IsDefault())
            {
                xml.WriteStartElement("series_information");
                SeriesInformation.WriteToXml(xml);
                xml.WriteEndElement();
            }

            xml.WriteElementBool("championship", Championship);
            xml.WriteElementBool("arcade", Arcade);

            xml.WriteEndElement();
        }

        /// <summary>
        /// Writes to Xml. Nodes will be sorted alphabetically.
        /// </summary>
        /// <param name="writer"></param>
        public void WriteToXmlSorted(XmlWriter writer)
        {
            using (var ms = new MemoryStream())
            {
                using (var tempWriter = XmlWriter.Create(ms))
                {
                    WriteToXml(tempWriter);
                }

                ms.Position = 0;
                XmlDocument sortedDoc = new XmlDocument();
                sortedDoc.Load(ms);

                SortXml(sortedDoc["GameParameter"]);
                sortedDoc["GameParameter"].WriteTo(writer);
            }
        }

        private static void SortXml(XmlNode node)
        {
            // Load the child elements into a collection.
            List<XmlNode> childElements = new List<XmlNode>();

            foreach (XmlNode childNode in node.ChildNodes)
            {
                childElements.Add(childNode);

                // Call recursively if the child is not a leaf.
                if (childNode.HasChildNodes)
                {
                    SortXml(childNode);
                }
            }

            node.RemoveAll();

            // Re-add the child elements (sorted).
            foreach (var childNode in childElements.OrderBy(element => element.Name))
            {
                // Make sure folder_id isn't at the very bottom
                if (childNode.Name == "folder_id")
                {
                    node.InsertBefore(childNode, childElements[1]);
                    continue;
                }

                node.AppendChild(childNode);
            }
        }

        public void ParseFromXmlNode(XmlNode node)
        {
            foreach (XmlNode childNode in node.ChildNodes)
            {
                switch (childNode.Name)
                {
                    case "folder_id":
                        FolderId = childNode.ReadValueULong(); break;

                    case "events":
                        foreach (XmlNode eventNode in childNode.SelectNodes("event"))
                        {
                            var newEvent = new Event();
                            newEvent.ParseFromXml(eventNode);
                            newEvent.FixInvalidNodesIfNeeded();
                            Events.Add(newEvent);
                        }
                        break;

                    case "online_room":
                        throw new NotImplementedException("Implement online_room parsing!");
                    case "series_reward":
                        SeriesRewards.ParseFromXml(childNode); break;
                    case "series_information":
                        SeriesInformation.ParseFromXml(childNode); break;
                    case "editor_info":
                        EditorInfo.ParseFromXml(childNode); break;
                    case "championship":
                        Championship = childNode.ReadValueBool(); break;
                    case "arcade":
                        Arcade = childNode.ReadValueBool(); break;
                }
            }
        }

        public void ReadFromBuffer(ref BitStream reader)
        {
            uint magic = reader.ReadUInt32();
            if (magic != 0xE5E54F17 && magic != 0xE6E64F17)
                ;

            int version = reader.ReadInt32();
            FolderId = reader.ReadUInt64();
            int event_index = reader.ReadInt32();
            int event_count = reader.ReadInt32();

            for (int i = 0; i < event_count; i++)
            {
                Event evnt = new Event();
                evnt.ReadFromCache(ref reader);
                Events.Add(evnt);
            }
        }

        public void ReadFromBuffer(Span<byte> buffer)
        {
            var reader = new BitStream(BitStreamMode.Read, buffer);
            ReadFromBuffer(ref reader);
        }

        public byte[] Serialize()
        {
            BitStream bs = new BitStream(BitStreamMode.Write, 1024);
            bs.WriteUInt32(0xE6_E6_4F_17);
            bs.WriteInt32(1_01); // Version
            bs.WriteUInt64(FolderId);
            bs.WriteInt32(0); // Event Index;

            bs.WriteInt32(Events.Count);
            foreach (var evnt in Events)
                evnt.WriteToCache(ref bs);

            OnlineRoom.WriteToCache(ref bs);
            SeriesRewards.Serialize(ref bs);
            SeriesInformation.Serialize(ref bs);
            EditorInfo.Serialize(ref bs);
            bs.WriteBool(Championship);
            bs.WriteBool(Arcade); // arcade
            bs.WriteBool(false); // keep_sequence
            bs.WriteByte((byte)LaunchContext.NONE); // launch_context

            bs.WriteUInt32(0xE6_E6_4F_18); // End Magic Terminator

            return bs.GetSpan().ToArray();
        }

        public byte[] SerializeToSTStruct()
        {
            // We could use a PD struct utility for all this, but eh, effort, its just two fields
            using (var ms = new MemoryStream())
            using (var bs = new BinaryStream(ms, ByteConverter.Big))
            {
                bs.WriteByte(0x0E); // PD Struct version
                bs.Position += 4; // Write symbols later
                bs.WriteByte(0x0A);
                bs.WriteByte(9);
                bs.WriteInt32(2);
                bs.WriteByte(7);
                bs.WriteInt16(6);

                bs.Position += 4; // Write GP Size later

                var gp = Serialize();

                bs.WriteBytes(gp);
                bs.WriteByte(7);
                bs.WriteByte(1);
                bs.WriteByte(3);
                bs.WriteInt32(101); // GP VERSION

                long symbolsOffset = bs.Position;
                bs.WriteByte(2);
                bs.WriteByte(4); bs.WriteString("data", StringCoding.Raw);
                bs.WriteByte(7); bs.WriteString("version", StringCoding.Raw);

                bs.Position = 1;
                bs.WriteUInt32((uint)symbolsOffset);

                bs.Position = 14;
                bs.WriteInt32(gp.Length);

                return ms.ToArray();
            }
        }

        public static GameParameter CreateSingleRace(int course_code, int entry_num, int arcade_laps, int ai_skill = -1, int enemy_lv = -1, 
            int boost_lv = -1, PenaltyLevelTypes penalty_level = PenaltyLevelTypes.OFF, List<int> prize_table = null, bool one_make = false)
        {
            var gp = new GameParameter();
            if (entry_num > 16)
                entry_num = 16;

            var evnt = gp.Event;
            evnt.GameMode = GameMode.SINGLE_RACE;

            var playStyle = evnt.PlayStyle;
            playStyle.PlayType = PlayType.RACE;
            playStyle.ReplayRecordEnable = true;

            var entrySet = evnt.EntrySet;
            var entryGenerate = entrySet.EntryGenerate;
            entryGenerate.EntryNum = entry_num;
            entryGenerate.PlayerPos = (entry_num - 1) / 2;

            if (ai_skill != -1)
                entryGenerate.AISkill = ai_skill;

            if (enemy_lv != -1)
                entryGenerate.EnemyLevel = enemy_lv;

            if (one_make)
                entryGenerate.GenerateType = EntryGenerateType.ONE_MAKE;
            else
                entryGenerate.GenerateType = EntryGenerateType.ENEMY_LIST;

            Track track = evnt.Track;
            track.CourseCode = course_code;

            RaceParameter rp = evnt.RaceParameter;
            rp.RaceType = RaceType.COMPETITION;
            rp.StartType = StartType.GRID;
            rp.CompleteType = CompleteType.BYLAPS;
            rp.FinishType = FinishType.TARGET;
            rp.TimeToFinish = TimeSpan.FromSeconds(30);
            rp.RaceLimitLaps = (short)arcade_laps;
            rp.EnablePit = false;
            rp.EntryMax = (short)entry_num;
            rp.RacersMax = (short)entry_num;
            rp.PenaltyLevel = penalty_level;

            if (boost_lv != -1)
                rp.BoostLevel = (byte)boost_lv;
            rp.LineGhostPlayMax = 0;
            rp.LineGhostRecordType = LineGhostRecordType.OFF;

            evnt.Reward.PrizeTable = prize_table ?? new List<int>();
            return gp;
        }

        public static GameParameter CreateTimeAttack(int course_code)
        {
            var gp = new GameParameter();

            var evnt = gp.Event;
            evnt.GameMode = GameMode.TIME_ATTACK;

            var playStyle = evnt.PlayStyle;
            playStyle.PlayType = PlayType.RACE;
            playStyle.NoQuickMenu = false;
            playStyle.ReplayRecordEnable = true;

            var entrySet = evnt.EntrySet;
            entrySet.Entries.Add(new Entry());
            entrySet.Entries[0].PlayerNumber = 0;

            RaceParameter rp = evnt.RaceParameter;
            rp.RaceType = RaceType.TIMEATTACK;
            rp.StartType = StartType.ATTACK;
            rp.CompleteType = CompleteType.NONE;
            rp.GhostType = GhostType.ONELAP;
            rp.PenaltyLevel = PenaltyLevelTypes.NO_TIME1;
            rp.EntryMax = 1;
            rp.RacersMax = 1;
            rp.LineGhostPlayMax = 10;
            rp.LineGhostRecordType = LineGhostRecordType.ONE;

            Track track = evnt.Track;
            track.CourseCode = course_code;

            return gp;
        }

        public static GameParameter CreateDriftAttack(int course_code, int layout = -1, bool endless = false)
        {
            var gp = new GameParameter();

            var evnt = gp.Event;
            evnt.GameMode = GameMode.DRIFT_ATTACK;

            var playStyle = evnt.PlayStyle;
            playStyle.PlayType = PlayType.RACE;
            playStyle.NoQuickMenu = false;
            playStyle.ReplayRecordEnable = true;

            var entrySet = evnt.EntrySet;
            entrySet.Entries.Add(new Entry());
            entrySet.Entries[0].PlayerNumber = 0;

            short num = 1;
            RaceParameter rp = evnt.RaceParameter;
            rp.RaceType = RaceType.DRIFTATTACK;
            if (endless)
                rp.StartType = StartType.ATTACK;
            else
                rp.StartType = num > 1 ? StartType.COURSEINFO : StartType.COURSEINFO_ROLLING;

            rp.StartType = StartType.ATTACK;
            rp.PenaltyLevel = PenaltyLevelTypes.NO_TIME1;
            rp.EntryMax = num;
            rp.RacersMax = num;
            rp.CompleteType = CompleteType.NONE;
            rp.GhostType = GhostType.NONE;
            rp.Endless = endless;
            rp.TimeToStart = TimeSpan.FromMilliseconds(1900);
            rp.TimeToFinish = TimeSpan.FromMilliseconds(2000);

            rp.LineGhostPlayMax = 0;
            rp.LineGhostRecordType = LineGhostRecordType.OFF;

            Track track = evnt.Track;
            track.CourseCode = course_code;
            if (layout != -1)
                track.CourseLayoutNumber = layout;

            return gp;
        }

        public static GameParameter CreateOnlineTimeAttack(int course_code)
        {
            var gp = CreateTimeAttack(course_code);

            var evnt = gp.Event;
            evnt.GameMode = GameMode.ONLINE_TIME_ATTACK;
            evnt.BeginDate = DateTime.Parse("1999/04/01 00:00:00");
            evnt.EndDate = DateTime.Parse("2999/04/01 00:00:00");

            var ranking = evnt.Ranking;
            ranking.BeginDate = DateTime.Parse("1999/04/01 00:00:00");
            ranking.EndDate = DateTime.Parse("2999/04/01 00:00:00");
            ranking.IsLocal = false;
            ranking.BoardID = 2000199;

            RaceParameter rp = evnt.RaceParameter;
            rp.LineGhostPlayMax = 10;
            rp.LineGhostRecordType = LineGhostRecordType.ONE;
            rp.EnableDamage = true;

            return gp;
        }

        public static GameParameter CreateOnlineDriftAttack(int course_code, int layout = -1, bool endless = false)
        {
            var gp = CreateTimeAttack(course_code);

            var evnt = gp.Event;
            evnt.GameMode = GameMode.ONLINE_DRIFT_ATTACK;
            return gp;
        }

        public static GameParameter CreateArcadeStyleRace(int course_code)
        {
            var gp = new GameParameter();
            short nb_entries = 10;

            var evnt = gp.Event;
            evnt.GameMode = GameMode.ARCADE_STYLE_RACE;

            var playStyle = evnt.PlayStyle;
            playStyle.PlayType = PlayType.RACE;
            playStyle.ReplayRecordEnable = true;

            var entrySet = evnt.EntrySet;
            for (var i = 0; i < nb_entries; i++)
                entrySet.Entries.Add(new Entry());

            var entryGenerate = entrySet.EntryGenerate;
            entryGenerate.EntryNum = nb_entries;
            entryGenerate.PlayerPos = 0;
            entryGenerate.AISkill = 10;
            entryGenerate.EnemyLevel = 0;

            entryGenerate.GenerateType = EntryGenerateType.SPEC_DB;

            RaceParameter rp = evnt.RaceParameter;
            rp.RaceType = RaceType.TIMEATTACK;
            rp.StartType = StartType.PIT;
            rp.CompleteType = CompleteType.BYLAPS;
            rp.FinishType = FinishType.TARGET;
            rp.RaceLimitLaps = 1;
            // race initial laps = 0
            rp.GhostType = GhostType.NONE;
            rp.PenaltyLevel = PenaltyLevelTypes.OFF;
            rp.EntryMax = nb_entries;
            rp.RacersMax = nb_entries;
            rp.LineGhostPlayMax = 10;
            rp.LineGhostRecordType = LineGhostRecordType.ONE;
            rp.EnableDamage = false;
            rp.BehaviorDamage = BehaviorDamageType.WEAK;

            Track track = evnt.Track;
            track.CourseCode = course_code;

            return gp;

        }

        public enum LaunchContext
        {
            NONE,
            ACACDEMY,
        }
    }
}
