using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.IO;

using PDTools.Files;

namespace PDTools.Files.Courses.Runway;

using Syroot.BinaryData;

public class RunwayFile
{
    /// <summary>
    /// Version major of the runway.
    /// </summary>
    public ushort VersionMajor { get; set; }

    /// <summary>
    /// Version minor of the runway.
    /// </summary>
    public ushort VersionMinor { get; set; }

    /// <summary>
    /// Unknown
    /// </summary>
    public uint Flags { get; set; }

    /// <summary>
    /// Min model bound of the runway.
    /// </summary>
    public Vector3 BoundsMin = default;

    /// <summary>
    /// Max model bound of the runway.
    /// </summary>
    public Vector3 BoundsMax = default;

    /// <summary>
    /// Increases depending on the track
    /// </summary>
    public uint UnkVal1 { get; set; }

    /// <summary>
    /// Unknown
    /// </summary>
    public uint UnkVal2 { get; set; }

    /// <summary>
    /// Track length in meters.
    /// </summary>
    public float TrackV { get; set; }

    public List<RunwaySector> SectorInfos { get; set; } = new();

    /// <summary>
    /// List of starts from the grid, as X Y Z angle.
    /// </summary>
    public List<Vec3R> StartingGrid { get; set; } = new();

    /// <summary>
    /// List of checkpoints.
    /// </summary>
    public List<RunwayCheckpoint> Checkpoints { get; set; } = new();

    public List<ushort> CheckpointList = new();

    /// <summary>
    /// List of defined lights for the runway.
    /// </summary>
    public List<RunwayLightSet> LightSets { get; set; } = new();

    /// <summary>
    /// Old gadgets for the runway, GT5 and 6 does not support it in the RWY file.
    /// </summary>
    public List<RunwayGadgetOld> Gadgets { get; set; } = new();

    /// <summary>
    /// Road vertices for this runway.
    /// </summary>
    public RunwayRoadVertMap RoadVerts = new();

    /// <summary>
    /// Road tris for tris for this runway.
    /// </summary>
    public List<RunwayRoadTri> RoadTris = new();

    public uint RayCastTreeMaxDepth { get; set; }
    public RunwayRayCastNode RayCastRootNode { get; set; }

    public List<RunwayBoundaryVert> BoundaryVerts { get; set; } = new();

    public List<ushort> BoundaryFaces = new();

    /// <summary>
    /// Positions of all pit stops in the runway.
    /// </summary>
    public List<Vec3R> PitStops = new();

    /// <summary>
    /// Positions of all adjacent pit stops in the runway.
    /// </summary>
    public List<Vec3R> PitStopAdjacents = new();

    public const int RNW4_BE = 0x524E5734;
    public const int RNW4_LE = 0x34574E52;
    public const int RNW5_BE = 0x524E5735;
    public const int RNW5_LE = 0x35574E52;

    public static RunwayFile FromStream(Stream stream)
    {
        BinaryStream bs = new BinaryStream(stream);
        long basePos = bs.Position;

        uint magic = bs.ReadUInt32();

        if (magic == RNW5_BE)
            bs.ByteConverter = ByteConverter.Big;
        else if (magic == RNW5_LE)
            bs.ByteConverter = ByteConverter.Little;
        else if (magic == RNW4_LE || magic == RNW4_BE)
            throw new InvalidDataException("RNW4 is not supported yet.");
        else
            throw new InvalidDataException("Unsupported runway format.");

        RunwayFile rwy = new RunwayFile();

        bs.ReadInt32(); // Reloc Pointer
        bs.ReadUInt32(); // Reloc Size
        rwy.VersionMajor = bs.ReadUInt16();
        rwy.VersionMinor = bs.ReadUInt16();
        rwy.Flags = bs.ReadUInt32();
        rwy.TrackV = bs.ReadSingle();
        bs.Position += 8;
        rwy.BoundsMin = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
        rwy.BoundsMax = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
        rwy.UnkVal1 = bs.ReadUInt32();
        rwy.UnkVal2 = bs.ReadUInt32();
        bs.Position += 4;

        uint sectorsMapCount = bs.ReadUInt32();
        uint sectorsMapOffset = bs.ReadUInt32();
        uint startingGridCount = bs.ReadUInt32();
        int startingGridOffset = bs.ReadInt32();
        uint checkpointCount = bs.ReadUInt32();
        int checkpointOffset = bs.ReadInt32();
        uint checkpointListCount = bs.ReadUInt32();
        int checkpointListOffset = bs.ReadInt32();
        uint lightSetCount = bs.ReadUInt32();
        uint lightSetOffset = bs.ReadUInt32();
        uint carLightSourceCount = bs.ReadUInt32();
        uint carLightSourcesOffset = bs.ReadUInt32();
        uint gadgetCount = bs.ReadUInt32();
        uint gadgetOffset = bs.ReadUInt32();
        uint roadVertCount = bs.ReadUInt32();
        int roadVertOffset = bs.ReadInt32();
        uint roadTriCount = bs.ReadUInt32();
        int roadTriOffset = bs.ReadInt32();

        bs.Position += 8;
        rwy.RayCastTreeMaxDepth = bs.ReadUInt32();
        uint rayCastTreeOffset = bs.ReadUInt32();
        uint boundaryVertCount = bs.ReadUInt32();
        int boundaryVertOffset = bs.ReadInt32();
        uint boundaryListCount = bs.ReadUInt32();
        int boundaryListOffset = bs.ReadInt32();
        uint pitStopCount = bs.ReadUInt32();
        int pitStopOffset = bs.ReadInt32();

        bs.Position = basePos + 0xBC;
        uint pitStopAdjacentCount = bs.ReadUInt32();
        int pitStopAdjacentOffset = bs.ReadInt32();

        bs.Position = basePos + sectorsMapOffset;
        for (int i = 0; i < sectorsMapCount; i++)
        {
            bs.Position = basePos + sectorsMapOffset + (i * 0x08);
            RunwaySector sector = RunwaySector.FromStream(bs, rwy.VersionMajor, rwy.VersionMinor);
            rwy.SectorInfos.Add(sector);
        }

        rwy.StartingGrid = new List<Vec3R>((int)startingGridCount);
        for (int i = 0; i < startingGridCount; i++)
        {
            bs.Position = basePos + startingGridOffset + (i * Vec3R.Size);
            Vec3R gridPos = Vec3R.FromStream(bs);
            rwy.StartingGrid.Add(gridPos);
        }

        for (int i = 0; i < checkpointCount; i++)
        {
            bs.Position = basePos + checkpointOffset + (i * RunwayCheckpoint.GetSize(rwy.VersionMajor, rwy.VersionMinor));
            RunwayCheckpoint chk = RunwayCheckpoint.FromStream(bs, rwy.VersionMajor, rwy.VersionMinor);
            rwy.Checkpoints.Add(chk);
        }

        bs.Position = basePos + checkpointListOffset;
        for (int i = 0; i < checkpointListCount; i++)
            rwy.CheckpointList.Add(bs.ReadUInt16());

        for (int i = 0; i < lightSetCount; i++)
        {
            bs.Position = basePos + lightSetOffset + (i * RunwayLightSet.GetSize(rwy.VersionMajor, rwy.VersionMinor));
            RunwayLightSet lightSet = RunwayLightSet.FromStream(bs, rwy.VersionMajor, rwy.VersionMinor);
            rwy.LightSets.Add(lightSet);
        }

        for (int i = 0; i < gadgetCount; i++)
        {
            bs.Position = basePos + gadgetOffset + (i * RunwayGadgetOld.GetSize(rwy.VersionMinor, rwy.VersionMinor));
            var gadget = RunwayGadgetOld.FromStream(bs, rwy.VersionMajor, rwy.VersionMinor);
            rwy.Gadgets.Add(gadget);
        }

        bs.Position = basePos + roadVertOffset;
        rwy.RoadVerts = RunwayRoadVertMap.FromStream(bs, roadVertCount, rwy.VersionMajor, rwy.VersionMinor);

        for (int i = 0; i < roadTriCount; i++)
        {
            bs.Position = basePos + roadTriOffset + (i * RunwayRoadTri.GetSize(rwy.VersionMajor, rwy.VersionMinor));
            RunwayRoadTri tri = RunwayRoadTri.FromStream(bs, rwy.VersionMajor, rwy.VersionMinor);
            rwy.RoadTris.Add(tri);
        } 
        
        if (rayCastTreeOffset != 0)
        {
            bs.Position = basePos + rayCastTreeOffset;
            rwy.RayCastRootNode = RunwayRayCastNode.TraverseRead(bs, rwy.VersionMajor, rwy.VersionMinor);
        }

        for (int i = 0; i < boundaryVertCount; i++)
        {
            bs.Position = basePos + boundaryVertOffset + (i * RunwayBoundaryVert.GetSize(rwy.VersionMajor, rwy.VersionMinor));
            RunwayBoundaryVert boundaryVert = RunwayBoundaryVert.FromStream(bs, rwy.VersionMajor, rwy.VersionMinor);
            rwy.BoundaryVerts.Add(boundaryVert);
        }

        bs.Position = basePos + boundaryListOffset;
        for (int i = 0; i < boundaryListCount; i++)
            rwy.BoundaryFaces.Add(bs.ReadUInt16());

        bs.Position = basePos + pitStopOffset;
        for (int i = 0; i < pitStopCount; i++)
        {
            bs.Position = basePos + pitStopOffset + (i * Vec3R.Size);
            Vec3R pitStopPos = Vec3R.FromStream(bs);
            rwy.PitStops.Add(pitStopPos);
        }

        bs.Position = basePos + pitStopAdjacentOffset;
        for (int i = 0; i < pitStopAdjacentCount; i++)
        {
            bs.Position = basePos + pitStopAdjacentOffset + (i * Vec3R.Size);
            Vec3R pitStopAdjacentPos = Vec3R.FromStream(bs);
            rwy.PitStopAdjacents.Add(pitStopAdjacentPos);
        }

        return rwy;
    }

    public void Merge(RunwayFile other)
    {
        BoundsMin = other.BoundsMin;
        BoundsMax = other.BoundsMax;
        TrackV = other.TrackV;
        StartingGrid = other.StartingGrid;
        Checkpoints = other.Checkpoints;
        CheckpointList = other.CheckpointList;
        BoundaryVerts = other.BoundaryVerts;
        BoundaryFaces = other.BoundaryFaces;
        PitStops = other.PitStops;
        PitStopAdjacents = other.PitStopAdjacents;
    }

    public void ToStream(Stream stream, bool bigEndian = true)
    {
        BinaryStream bs = new BinaryStream(stream);
        long basePos = bs.Position;

        if (bigEndian)
        {
            bs.WriteUInt32(RNW5_BE);
            bs.ByteConverter = ByteConverter.Big;
        }
        else
        {
            bs.WriteUInt32(RNW5_LE);
            bs.ByteConverter = ByteConverter.Little;
        }

        if (VersionMajor < 4 || VersionMajor >= 5)
            throw new NotImplementedException($"Export not implemented for RNW5 v{VersionMajor}.{VersionMinor}");

        bs.Position = 0x100;

        long sectorInfoMapOffset = SectorInfos.Count != 0 ? bs.Position : 0;
        WriteSectorInfos(bs); // To check

        long startingGridOffset = StartingGrid.Count != 0 ? bs.Position : 0;
        WriteStartingGrid(bs);

        long checkpointsOffset = Checkpoints.Count != 0 ? bs.Position : 0;
        WriteCheckpoints(bs);

        long checkpointListOffset = CheckpointList.Count != 0 ? bs.Position : 0;
        WriteCheckpointList(bs);

        long lightSetOffset = LightSets.Count != 0 ? bs.Position : 0;
        WriteLightSet(bs);

        long gadgetsOffset = Gadgets.Count != 0 ? bs.Position : 0;
        WriteOldGadgets(bs);

        long roadVertsOffset = RoadVerts.Vertices.Count != 0 ? bs.Position : 0;
        WriteRoadVerts(bs);

        long roadTrisOffset = RoadTris.Count != 0 ? bs.Position : 0;
        WriteRoadTris(bs);

        long rayCastTreeOffset = RayCastRootNode is not null ? bs.Position : 0;
        WriteRayCastTree(bs);

        long boundaryVertsOffset = BoundaryVerts.Count != 0 ? bs.Position : 0;
        WriteBoundaryVerts(bs);

        long boundaryFacesOffset = BoundaryFaces.Count != 0 ? bs.Position : 0;
        WriteBoundaryFaces(bs);

        long pitStopsOffset = PitStops.Count != 0 ? bs.Position : 0;
        WritePitStops(bs);

        long fileSize = bs.Position - basePos;

        /* Writing Header */
        bs.Position = basePos + 0x8;
        bs.WriteUInt32((uint)fileSize);
        bs.WriteUInt16(VersionMajor);
        bs.WriteUInt16(VersionMinor);
        bs.WriteUInt32(Flags);
        bs.WriteSingle(TrackV);
        bs.Position += 0x08;
        bs.WriteSingle(BoundsMin.X);
        bs.WriteSingle(BoundsMin.Y);
        bs.WriteSingle(BoundsMin.Z);
        bs.WriteSingle(BoundsMax.X);
        bs.WriteSingle(BoundsMax.Y);
        bs.WriteSingle(BoundsMax.Z);
        bs.WriteUInt32(UnkVal1);
        bs.WriteUInt32(UnkVal2);
        bs.Position += 4;

        bs.WriteUInt32((uint)SectorInfos.Count);
        bs.WriteUInt32((uint)(sectorInfoMapOffset - basePos));
        bs.WriteUInt32((uint)StartingGrid.Count);
        bs.WriteUInt32((uint)(startingGridOffset - basePos));
        bs.WriteUInt32((uint)Checkpoints.Count);
        bs.WriteUInt32((uint)(checkpointsOffset - basePos));
        bs.WriteUInt32((uint)CheckpointList.Count);
        bs.WriteUInt32((uint)(checkpointListOffset - basePos));
        bs.WriteUInt32((uint)LightSets.Count);
        bs.WriteUInt32((uint)(lightSetOffset - basePos));
        
        bs.Position += 0x08;
        bs.WriteUInt32((uint)Gadgets.Count);
        bs.WriteUInt32((uint)(gadgetsOffset - basePos));
        bs.WriteUInt32((uint)RoadVerts.Vertices.Count);
        bs.WriteUInt32((uint)(roadVertsOffset - basePos));
        bs.WriteUInt32((uint)RoadTris.Count);
        bs.WriteUInt32((uint)(roadTrisOffset - basePos));

        bs.Position += 8;
        bs.WriteUInt32(RayCastTreeMaxDepth);
        bs.WriteUInt32((uint)(rayCastTreeOffset - basePos));
        bs.WriteUInt32((uint)BoundaryVerts.Count);
        bs.WriteUInt32((uint)boundaryVertsOffset);
        bs.WriteUInt32((uint)BoundaryFaces.Count);
        bs.WriteUInt32((uint)boundaryFacesOffset);
        bs.WriteUInt32((uint)PitStops.Count);
        bs.WriteUInt32((uint)pitStopsOffset);
    }

    private void WriteSectorInfos(BinaryStream bs)
    {
        for (int i = 0; i < SectorInfos.Count; i++)
        {
            RunwaySector sectorInfo = SectorInfos[i];
            sectorInfo.ToStream(bs, VersionMajor, VersionMinor);
        }

        bs.Align(0x10, grow: true);
    }

    private void WriteStartingGrid(BinaryStream bs)
    {
        for (int i = 0; i < StartingGrid.Count; i++)
        {
            Vec3R startingGridPos = StartingGrid[i];
            startingGridPos.ToStream(bs);
        }

        bs.Align(0x10, grow: true);
    }

    private void WriteCheckpoints(BinaryStream bs)
    {
        for (int i = 0; i < Checkpoints.Count; i++)
        {
            RunwayCheckpoint checkpoint = Checkpoints[i];
            checkpoint.ToStream(bs, VersionMajor, VersionMinor);
        }

        bs.Align(0x08, grow: true);
    }

    private void WriteLightSet(BinaryStream bs)
    {
        for (int i = 0; i < LightSets.Count; i++)
        {
            RunwayLightSet lightSet = LightSets[i];
            lightSet.ToStream(bs, VersionMajor, VersionMinor);
        }

        bs.Align(0x08, grow: true);
    }

    private void WriteCheckpointList(BinaryStream bs)
    {
        for (int i = 0; i < CheckpointList.Count; i++)
        {
            ushort chkIdx = CheckpointList[i];
            bs.WriteUInt16(chkIdx);
        }

        bs.Align(0x10, grow: true);
    }

    private void WriteOldGadgets(BinaryStream bs)
    {
        for (int i = 0; i < Gadgets.Count; i++)
        {
            RunwayGadgetOld gadget = Gadgets[i];
            gadget.ToStream(bs, VersionMajor, VersionMinor);
        }
    }

    private void WriteRoadVerts(BinaryStream bs)
    {
        RoadVerts.ToStream(bs, VersionMajor, VersionMinor);
        bs.Align(0x10, grow: true);
    }

    private void WriteRoadTris(BinaryStream bs)
    {
        for (int i = 0; i < RoadTris.Count; i++)
        {
            RunwayRoadTri tri = RoadTris[i];
            tri.ToStream(bs, VersionMajor, VersionMinor);
        }


        bs.Align(0x10, grow: true);
    }

    private void WriteRayCastTree(BinaryStream bs)
    {
        if (RayCastRootNode != null)
        {
            RayCastRootNode.ToStream(bs, VersionMajor, VersionMinor);
            bs.Align(0x10, grow: true);
        }
    }

    private void WriteBoundaryVerts(BinaryStream bs)
    {
        for (int i = 0; i < BoundaryVerts.Count; i++)
        {
            RunwayBoundaryVert vert = BoundaryVerts[i];
            vert.ToStream(bs, VersionMajor, VersionMinor);
        }

        bs.Align(0x10, grow: true);
    }

    private void WriteBoundaryFaces(BinaryStream bs)
    {
        for (int i = 0; i < BoundaryFaces.Count; i++)
        {
            bs.WriteUInt16(BoundaryFaces[i]);
        }
    }

    private void WritePitStops(BinaryStream bs)
    {
        for (int i = 0; i < PitStops.Count; i++)
        {
            Vec3R pitStopPos = PitStops[i];
            pitStopPos.ToStream(bs);
        }

        bs.Align(0x10, grow: true);
    }
}

