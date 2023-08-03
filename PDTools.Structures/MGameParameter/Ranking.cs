using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

using PDTools.Enums;
using PDTools.Utils;

namespace PDTools.Structures.MGameParameter
{
    public class Ranking
    {
        /// <summary>
        /// Defaults to <see cref="RankingType.NONE"/>.
        /// </summary>
        public RankingType Type { get; set; } = RankingType.NONE;

        /// <summary>
        /// Defaults to true.
        /// </summary>
        public bool IsLocal { get; set; } = true;

        /// <summary>
        /// Number of ranks where replays are uploaded. 10 would be top 10 replays are uploaded. Defaults to 0. 
        /// </summary>
        public short ReplayRankLimit { get; set; }

        /// <summary>
        /// How many ranks are displayed on the ui. Defaults to 100 (top 100).
        /// </summary>
        public short DisplayRankLimit { get; set; } = 100;

        /// <summary>
        /// Ranking Board ID.
        /// </summary>
        public ulong BoardID { get; set; }

        /// <summary>
        /// Defaults to 0.
        /// </summary>
        public short Registration { get; set; } = 0;

        /// <summary>
        /// GT6 Only. Defaults to <see cref="RegistrationType.NORMAL"/>.
        /// </summary>
        public RegistrationType RegistrationType { get; set; } = RegistrationType.NORMAL;

        /// <summary>
        /// When the ranking registration period starts for this event.
        /// </summary>
        public DateTime BeginDate { get; set; }

        /// <summary>
        /// When the ranking registration period ends for this event.
        /// </summary>
        public DateTime EndDate { get; set; }

        public bool IsDefault()
        {
            var defaultRanking = new Ranking();
            return Type == defaultRanking.Type &&
                IsLocal == defaultRanking.IsLocal &&
                ReplayRankLimit == defaultRanking.ReplayRankLimit &&
                DisplayRankLimit == defaultRanking.DisplayRankLimit &&
                BoardID == defaultRanking.BoardID &&
                Registration == defaultRanking.Registration &&
                RegistrationType == defaultRanking.RegistrationType &&
                BeginDate == defaultRanking.BeginDate &&
                EndDate == defaultRanking.EndDate;
        }

        public void ParseFromXml(XmlNode node)
        {
            foreach (XmlNode rNode in node.ChildNodes)
            {
                switch (rNode.Name)
                {
                    case "type":
                        Type = rNode.ReadValueEnum<RankingType>(); break;
                    case "is_local":
                        IsLocal = rNode.ReadValueBool(); break;
                    case "replay_rank_limit":
                        ReplayRankLimit = rNode.ReadValueShort(); break;
                    case "display_rank_limit":
                        DisplayRankLimit = rNode.ReadValueShort(); break;
                    case "board_id":
                        BoardID = rNode.ReadValueULong(); break;
                    case "registration":
                        Registration = rNode.ReadValueShort(); break;
                    case "registration_type":
                        RegistrationType = rNode.ReadValueEnum<RegistrationType>(); break;

                    case "begin_date":
                        string date = rNode.InnerText.Replace("/00", "/01");
                        DateTime.TryParseExact(date, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime time);
                        BeginDate = time;
                        break;
                    case "end_date":
                        string eDate = rNode.InnerText.Replace("/00", "/01");
                        DateTime.TryParseExact(eDate, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime eTime);
                        EndDate = eTime;
                        break;
                }
            }
        }

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteElementValue("type", Type.ToString());
            xml.WriteElementBool("is_local", IsLocal);
            xml.WriteElementInt("replay_rank_limit", ReplayRankLimit);
            xml.WriteElementInt("display_rank_limit", DisplayRankLimit);
            xml.WriteElementULong("board_id", BoardID);
            xml.WriteElementInt("registration", Registration);
            xml.WriteElementValue("registration_type", RegistrationType.ToString());

            xml.WriteStartElement("begin_date"); xml.WriteString(BeginDate.ToString("yyyy/MM/dd HH:mm:ss")); xml.WriteEndElement();
            xml.WriteStartElement("end_date"); xml.WriteString(EndDate.ToString("yyyy/MM/dd HH:mm:ss")); xml.WriteEndElement();
        }

        public void Serialize(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E6_6D_E9);
            bs.WriteUInt32(1_01); // Version

            bs.WriteInt16((short)Type);
            bs.WriteInt16(IsLocal ? (short)1 : (short)0);
            bs.WriteInt16(ReplayRankLimit);
            bs.WriteInt16(DisplayRankLimit);
            bs.WriteUInt64(BoardID);
            bs.WriteDouble(PDIDATETIME.DateTimeToJulian_64(BeginDate));
            bs.WriteDouble(PDIDATETIME.DateTimeToJulian_64(EndDate));
            bs.WriteInt16(Registration);
            bs.WriteSByte((sbyte)RegistrationType);
        }
    }
}
