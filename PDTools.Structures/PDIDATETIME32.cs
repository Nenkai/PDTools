using System;

namespace PDTools
{
	public struct PDIDATETIME32
	{
		private uint _timeData;

		public void SetDateTime(DateTime date)
		{
			_timeData = 0U;
			if (date.Year > 1970)
				_timeData |= (uint)(date.Year - 1970) << 26;

			_timeData |= (uint)date.Month << 22;
			_timeData |= (uint)date.Day << 17;
			_timeData |= (uint)date.Hour << 12;
			_timeData |= (uint)date.Minute << 6;
			_timeData |= (uint)date.Second;
		}

		public DateTime GetDateTime()
			=> new DateTime(
				(int)((_timeData >> 26 & 0x3F) + 1970), // Year
				(int)(_timeData >> 22 & 0xF), // Month
				(int)(_timeData >> 17 & 0x1F), // Day
				(int)(_timeData >> 12 & 0x1F), // Hour
				(int)(_timeData >> 6 & 0x3F), // Min
				(int)(_timeData & 0x3F)); // Sec

		public uint GetRawData()
			=> _timeData;

		public void SetRawData(uint data)
			=> _timeData = data;
	}
}