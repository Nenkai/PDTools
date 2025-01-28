using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

using PDTools.Files.Sound.Se.Meta;

namespace PDTools.Files.Sound.Se;

// SeSequencer is the brother of SqSequencer intended for sound effects
// Very very few events supported here, 4 bit channel number not used either
public class SeSeq
{
    public List<SeMessage> Messages { get; set; } = [];

    public void Read(BinaryStream bs)
    {
        // Starting from here, closely matches midi format specification
        byte lastStatus = 0;
        while (true)
        {
            var message = new SeMessage();
            message.Read(bs, lastStatus);
            if (message.Status == 0xFF)
            {
                var meta = message.Event as SeMetaEvent;
                if (meta.Type == 0x2F)
                    break;
            }

            Messages.Add(message);
            lastStatus = message.Status;
        }
    }

    // BIG NOTE: 7bit int implementation is different!
    public static long readVariable(BinaryStream bs)
    {
        long result = 0;
        byte @byte;

        do
        {
            @byte = bs.Read1Byte();
            result = (result << 7) + (@byte & 0x7F);
        }
        while ((@byte & 0x80) != 0);
        return result;
    }
}

public class SeMessage
{
    public uint Delta { get; set; }
    public byte Status { get; set; }
    public ISeEvent Event { get; set; }

    public void Read(BinaryStream bs, byte lastStatus)
    {
        byte status = bs.Read1Byte();
        if ((status & 0x80) != 0)
            Status = status;
        else
        {
            Status = lastStatus;
            bs.Position -= 1;
        }

        // This is all that's supported by the SqSequencer
        // Note that SeSequencer may support more events/meta (not implemented for now, it's used by sfx - midi is bundled inside the ins header in that case)
        // NOTE: Channel (lower 4 bits) is never used!
        if ((Status & 0xF0) == 0xA0)
        {
            Event = new SeNotePressureEvent();
            Event.Read(bs);
        }
        else if ((Status & 0xF0) == 0xB0)
        {
            Event = new SeControllerEvent();
            Event.Read(bs);
        }
        else if (status == 0xFF)
        {
            Event = new SeMetaEvent();
            Event.Read(bs);

            if (Event is SeMetaEvent meta && meta.Type == 0x2F)
                return;
        }

        Delta = (uint)SeSeq.readVariable(bs);
    }
}

public class SeNotePressureEvent : ISeEvent
{
    public byte Note { get; set; }
    public byte Pressure { get; set; }
    public byte Channel { get; set; }

    public void Read(BinaryStream bs)
    {
        // This one has extra values

        // supported by SeSequencer:
        // - 0x01 (modulation wheel) aka pitch modulate depth
        //   * val1 = value, val2 = channel index, val3 = note
        //
        // - 0x02 (breath) aka pitch modulate speed
        //   * val1 = value, val2 = channel index, val3 = note
        //
        // - 0x07 (volume)
        //   * use val 1 & 2, val 3 = channel index, use extra note
        //
        // - 0x0a (pan)
        //   * use val 1 & 2, val 3 = channel index, use extra note
        //
        // - something above 0x0a and below 0x60 - set auto pitch
        //   * use val 1 & 2, val 3 = channel index, use extra note
        //
        // - 0x60 (data increment)
        //   * concatenate value 1 and 2 into a short as offset to jump, value 3 unk
        //   * read extra var int value as delta possibly

        // There are 3 values here (channel is added)
        Note = bs.Read1Byte();
        Pressure = bs.Read1Byte();
        Channel = bs.Read1Byte();
    }
}

public class SeControllerEvent : ISeEvent
{
    public byte Type { get; set; }
    public byte Value { get; set; }
    public byte Value2 { get; set; }
    public byte Value3 { get; set; }

    public byte Note { get; set; }

    public void Read(BinaryStream bs)
    {
        // This one has extra values

        // supported by SeSequencer:
        // - 0x01 (modulation wheel) aka pitch modulate depth
        //   * val1 = value, val2 = channel index, val3 = note
        //
        // - 0x02 (breath) aka pitch modulate speed
        //   * val1 = value, val2 = channel index, val3 = note
        //
        // - 0x07 (volume)
        //   * use val 1 & 2, val 3 = channel index, use extra note
        //
        // - 0x0a (pan)
        //   * use val 1 & 2, val 3 = channel index, use extra note
        //
        // - something above 0x0a and below 0x60 - set auto pitch
        //   * use val 1 & 2, val 3 = channel index, use extra note
        //
        // - 0x60 (data increment)
        //   * concatenate value 1 and 2 into a short as offset to jump, value 3 unk
        //   * read extra var int value as delta possibly

        // There are 3 values here
        Type = bs.Read1Byte();
        Value = bs.Read1Byte();
        Value2 = bs.Read1Byte();
        Value3 = bs.Read1Byte();

        if (Type >= 3 && Type < 60)
        {
            Note = bs.Read1Byte();
        }
            
    }
}

public class SeMetaEvent : ISeEvent
{
    public byte Type { get; set; }
    public uint Length { get; set; }
    public ISeMeta Meta { get; set; }

    public void Read(BinaryStream bs)
    {
        Type = bs.Read1Byte();
        Length = (uint)bs.Read7BitInt32();

        // Only end supported
        if (Type == 0x2F)
        {
            // TODO
        }

        bs.ReadBytes((int)Length);
    }
}

public interface ISeEvent
{
    public void Read(BinaryStream bs);
}
