﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.IO;

using Syroot.BinaryData;

using PDTools.Files;
using PDTools.Files.Models.ModelSet3;

namespace PDTools.Files.Courses.CourseData;

/// <summary>
/// Represents a course data file. (Disposable object)
/// </summary>
public class CourseDataFile : IDisposable
{
    public const uint Magic_BE = 0x50414342;
    public const uint Magic_LE = 0x5041434C;

    public record CourseDataFileEntry(uint Type, uint Alignment, uint DataStart, uint DataLength);
    public List<CourseDataFileEntry> Entries { get; set; } = new();

    public ModelSet3 MainModelSet { get; set; }
    public Stream Stream { get; set; }

    public static CourseDataFile Open(string file)
    {
        CourseDataFile courseData = new CourseDataFile();
        courseData.Stream = new FileStream(file, FileMode.Open);
        BinaryStream bs = new BinaryStream(courseData.Stream);
        bs.ByteConverter = ByteConverter.Big;

        long basePos = bs.Position;
        uint magic = bs.ReadUInt32();

        if (magic == Magic_BE)
            bs.ByteConverter = ByteConverter.Big;
        else if (magic == Magic_LE)
            bs.ByteConverter = ByteConverter.Little;
        else
            throw new InvalidDataException("Unsupported course data format.");

        bs.Position = 0x3C;
        uint tocEntryCount = bs.ReadUInt32();

        if (tocEntryCount == 0)
            throw new InvalidDataException("Contents is empty.");

        for (int i = 0; i < tocEntryCount; i++)
        {
            bs.Position = 0x40 + (i * 0x10);
            uint type = bs.ReadUInt32();
            uint alignment = bs.ReadUInt32();
            uint dataStart = bs.ReadUInt32();
            uint dataLength = bs.ReadUInt32();
            CourseDataFileEntry entry = new CourseDataFileEntry(type, alignment, dataStart, dataLength);
            courseData.Entries.Add(entry);
        }

        bs.Position = courseData.Entries[0].DataStart;
        long baseEntryPos = bs.Position;

        bs.ReadInt32();
        uint mainModelOffset = bs.ReadUInt32();
        uint unkModelOffset = bs.ReadUInt32();

        bs.Position = baseEntryPos + mainModelOffset;
        courseData.MainModelSet = ModelSet3.FromStream(bs);

        //bs.Position = baseEntryPos + unkModelOffset;
        //courseData.MainModelSet = ModelSet3.FromStream(bs);

        courseData.MainModelSet.ParentCourseData = courseData;

        return courseData;
    }

    public void Dispose()
    {
        Stream?.Dispose();
    }

    /*
    public void DeleteModel(ref SpanReader sr, ref SpanWriter sw, byte modelID)
    {
        int basePos = sr.Position;

        sr.Position = basePos + 0x48;
        int dataStart = sr.ReadInt32();

        sw.Position = dataStart + 0x4 + (modelID * 0x4);
        sw.WriteInt32(0);

        _ = Models.Remove(modelID);
    }*/
}

