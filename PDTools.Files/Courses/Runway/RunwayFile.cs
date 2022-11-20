﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.IO;

using PDTools.Files;

namespace PDTools.Files.Courses.Runway;

using Syroot.BinaryData;

using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics;

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
    /// Unknown (GT6 Apricot Hill has it set)
    /// </summary>
    public uint UnkVal3 { get; set; }

    /// <summary>
    /// Track length in meters.
    /// </summary>
    public float TrackV { get; set; }

    public List<RunwaySector> SectorInfos { get; set; } = new();

    /// <summary>
    /// List of starts from the grid, as X Y Z angle.
    /// </summary>
    public List<RunwayStartingGridPosition> StartingGrid { get; set; } = new();

    /// <summary>
    /// List of checkpoints.
    /// </summary>
    public List<RunwayCheckpoint> Checkpoints { get; set; } = new();

    public List<ushort> CheckpointList = new();

    /// <summary>
    /// List of defined lights for the runway. (GT6)
    /// </summary>
    public List<RunwayLightDefinition> LightDefs { get; set; } = new();

    /// <summary>
    /// List of defined light sets for the runway. (GT6)
    /// </summary>
    public RunwayCarLightSetCollection LightSets { get; set; } = new();

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

    /// <summary>
    /// For GT7
    /// </summary>
    public byte BBoxAndRoadVertScaleX { get; set; }

    /// <summary>
    /// For GT7
    /// </summary>
    public byte BBoxAndRoadVertScaleY { get; set; }

    /// <summary>
    /// For GT7
    /// </summary>
    public byte BBoxAndRoadVertScaleZ { get; set; }

    public uint RayCastTreeMaxDepth { get; set; }
    public RunwayRayCastNode RayCastRootNode { get; set; }

    /// <summary>
    /// For GT7
    /// </summary>
    public byte BoundaryVertScaleX { get; set; }

    /// <summary>
    /// For GT7
    /// </summary>
    public byte BoundaryVertScaleY { get; set; }

    /// <summary>
    /// For GT7
    /// </summary>
    public byte BoundaryVertScaleZ { get; set; }

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

    /// <summary>
    /// GT6 Only
    /// </summary>
    public List<RunwayLightVFX> LightParticles = new();

    public const int RNW4_BE = 0x524E5734;
    public const int RNW4_LE = 0x34574E52;
    public const int RNW5_BE = 0x524E5735;
    public const int RNW5_LE = 0x35574E52;
    public const int RW64_LE = 0x34365752;

    public uint Magic { get; set; }
    public static RunwayFile FromStream(Stream stream)
    {
        BinaryStream bs = new BinaryStream(stream);
        long basePos = bs.Position;

        RunwayFile rwy = new RunwayFile();
        rwy.Magic = bs.ReadUInt32();

        if (rwy.Magic == RNW5_BE)
            bs.ByteConverter = ByteConverter.Big;
        else if (rwy.Magic == RNW5_LE)
            bs.ByteConverter = ByteConverter.Little;
        else if (rwy.Magic == RW64_LE)
            bs.ByteConverter = ByteConverter.Little;
        else if (rwy.Magic == RNW4_LE || rwy.Magic == RNW4_BE)
            throw new InvalidDataException("RNW4 is not supported yet.");
        else
            throw new InvalidDataException("Unsupported runway format.");

        bs.ReadInt32(); // Reloc Pointer
        bs.ReadUInt32(); // Reloc Size

        if (bs.ByteConverter == ByteConverter.Big)
        {
            rwy.VersionMajor = bs.ReadUInt16();
            rwy.VersionMinor = bs.ReadUInt16();
        }
        else
        {
            rwy.VersionMinor = bs.ReadUInt16();
            rwy.VersionMajor = bs.ReadUInt16();
        }

        rwy.Flags = bs.ReadUInt32();
        rwy.TrackV = bs.ReadSingle();
        bs.Position += 8;

        rwy.BoundsMin = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
        rwy.BoundsMax = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());

        rwy.UnkVal1 = bs.ReadUInt32();
        rwy.UnkVal2 = bs.ReadUInt32();
        rwy.UnkVal3 = bs.ReadUInt32();

        if (rwy.VersionMajor >= 6)
            bs.Position += 0x0C;

        long sectorsMapCount = Read32Or64(bs, rwy.VersionMajor);
        long sectorsMapOffset = Read32Or64(bs, rwy.VersionMajor);
        long startingGridCount = Read32Or64(bs, rwy.VersionMajor);
        long startingGridOffset = Read32Or64(bs, rwy.VersionMajor);
        long checkpointCount = Read32Or64(bs, rwy.VersionMajor);
        long checkpointOffset = Read32Or64(bs, rwy.VersionMajor);
        long checkpointListCount = Read32Or64(bs, rwy.VersionMajor);
        long checkpointListOffset = Read32Or64(bs, rwy.VersionMajor);
        long lightDefCount = Read32Or64(bs, rwy.VersionMajor);
        long lightDefsOffset = Read32Or64(bs, rwy.VersionMajor);
        long lightSetsCount = Read32Or64(bs, rwy.VersionMajor);
        long lightSetsOffset = Read32Or64(bs, rwy.VersionMajor);
        long gadgetCount = Read32Or64(bs, rwy.VersionMajor);
        long gadgetOffset = Read32Or64(bs, rwy.VersionMajor);
        long roadVertCount = bs.ReadInt32();

        if (rwy.VersionMajor >= 6)
        {
            rwy.BBoxAndRoadVertScaleX = bs.Read1Byte();
            rwy.BBoxAndRoadVertScaleY = bs.Read1Byte();
            rwy.BBoxAndRoadVertScaleZ = bs.Read1Byte();
            bs.Read1Byte(); // Unk, 1

            if (rwy.BBoxAndRoadVertScaleX > 0)
            {
                rwy.BoundsMin = rwy.BoundsMin with
                {
                    X = (float)DecodeAVXFloat(BitConverter.SingleToInt32Bits(rwy.BoundsMin.X), rwy.BBoxAndRoadVertScaleX)
                };

                rwy.BoundsMax = rwy.BoundsMax with
                {
                    X = (float)DecodeAVXFloat(BitConverter.SingleToInt32Bits(rwy.BoundsMax.X), rwy.BBoxAndRoadVertScaleX)
                };
            }

            if (rwy.BBoxAndRoadVertScaleY > 0)
            {
                rwy.BoundsMin = rwy.BoundsMin with
                {
                    Y = (float)DecodeAVXFloat(BitConverter.SingleToInt32Bits(rwy.BoundsMin.Y), rwy.BBoxAndRoadVertScaleY)
                };

                rwy.BoundsMax = rwy.BoundsMax with
                {
                    Y = (float)DecodeAVXFloat(BitConverter.SingleToInt32Bits(rwy.BoundsMax.Y), rwy.BBoxAndRoadVertScaleY)
                };
            }

            if (rwy.BBoxAndRoadVertScaleZ > 0)
            {
                rwy.BoundsMin = rwy.BoundsMin with
                {
                    Z = (float)DecodeAVXFloat(BitConverter.SingleToInt32Bits(rwy.BoundsMin.Z), rwy.BBoxAndRoadVertScaleZ)
                };

                rwy.BoundsMax = rwy.BoundsMax with
                {
                    Z = (float)DecodeAVXFloat(BitConverter.SingleToInt32Bits(rwy.BoundsMax.Z), rwy.BBoxAndRoadVertScaleZ)
                };
            }
        }

        long roadVertOffset = Read32Or64(bs, rwy.VersionMajor);
        long roadTriCount = Read32Or64(bs, rwy.VersionMajor);
        long roadTriOffset = Read32Or64(bs, rwy.VersionMajor);

        bs.Position += 0x10;
        rwy.RayCastTreeMaxDepth = (uint)Read32Or64(bs, rwy.VersionMajor);
        long rayCastTreeOffset = Read32Or64(bs, rwy.VersionMajor);
        long boundaryVertCount = bs.ReadUInt32();
        if (rwy.VersionMajor >= 6)
        {
            rwy.BoundaryVertScaleX = bs.Read1Byte();
            rwy.BoundaryVertScaleY = bs.Read1Byte();
            rwy.BoundaryVertScaleZ = bs.Read1Byte();
            bs.Read1Byte(); // Unk
        }

        long boundaryVertOffset = Read32Or64(bs, rwy.VersionMajor);
        long boundaryListCount = Read32Or64(bs, rwy.VersionMajor);
        long boundaryListOffset = Read32Or64(bs, rwy.VersionMajor);
        long pitStopCount = Read32Or64(bs, rwy.VersionMajor);
        long pitStopOffset = Read32Or64(bs, rwy.VersionMajor);
        long unkCount = Read32Or64(bs, rwy.VersionMajor);
        long unkOffset = Read32Or64(bs, rwy.VersionMajor);
        Read32Or64(bs, rwy.VersionMajor);
        long pitStopAdjacentOffset = Read32Or64(bs, rwy.VersionMajor);
        Read32Or64(bs, rwy.VersionMajor);
        long lightParticlesOffset = Read32Or64(bs, rwy.VersionMajor);

        bs.Position = basePos + sectorsMapOffset;
        for (int i = 0; i < sectorsMapCount; i++)
        {
            bs.Position = basePos + sectorsMapOffset + (i * 0x08);
            RunwaySector sector = RunwaySector.FromStream(bs, rwy.VersionMajor, rwy.VersionMinor);
            rwy.SectorInfos.Add(sector);
        }

        rwy.StartingGrid = new List<RunwayStartingGridPosition>((int)startingGridCount);
        for (int i = 0; i < startingGridCount; i++)
        {
            bs.Position = basePos + startingGridOffset + (i * Vec3R.Size);
            RunwayStartingGridPosition gridPos = RunwayStartingGridPosition.FromStream(bs, rwy.VersionMajor, rwy.VersionMinor);
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

        if (rwy.VersionMajor < 6)
        {
            for (int i = 0; i < lightDefCount; i++)
            {
                bs.Position = basePos + lightDefsOffset + (i * RunwayLightDefinition.GetSize(rwy.VersionMajor, rwy.VersionMinor));
                RunwayLightDefinition lightDef = RunwayLightDefinition.FromStream(bs, rwy.VersionMajor, rwy.VersionMinor);
                rwy.LightDefs.Add(lightDef);
            }

            for (int i = 0; i < lightSetsCount; i++)
            {
                bs.Position = basePos + lightSetsOffset;
                rwy.LightSets = RunwayCarLightSetCollection.FromStream(bs, lightSetsCount, rwy.VersionMajor, rwy.VersionMinor);
            }
        }

        for (int i = 0; i < gadgetCount; i++)
        {
            bs.Position = basePos + gadgetOffset + (i * RunwayGadgetOld.GetSize(rwy.VersionMinor, rwy.VersionMinor));
            var gadget = RunwayGadgetOld.FromStream(bs, rwy.VersionMajor, rwy.VersionMinor);
            rwy.Gadgets.Add(gadget);
        }

        bs.Position = basePos + roadVertOffset;
        rwy.RoadVerts = RunwayRoadVertMap.FromStream(bs, roadVertCount, rwy);

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
            bs.Position = basePos + boundaryVertOffset + (i * RunwayBoundaryVert.GetSize(rwy));
            RunwayBoundaryVert boundaryVert = RunwayBoundaryVert.FromStream(bs, rwy);
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

        if (pitStopAdjacentOffset != 0)
        {
            bs.Position = basePos + pitStopAdjacentOffset - 4;
            uint pitStopAdjacentCount = bs.ReadUInt32();
            for (int i = 0; i < pitStopAdjacentCount; i++)
            {
                bs.Position = basePos + pitStopAdjacentOffset + (i * Vec3R.Size);
                Vec3R pitStopAdjacentPos = Vec3R.FromStream(bs);
                rwy.PitStopAdjacents.Add(pitStopAdjacentPos);
            }
        }

        if (lightParticlesOffset != 0)
        {
            bs.Position = basePos + lightParticlesOffset - 4;
            uint lightParticlesCount = bs.ReadUInt32();
            for (int i = 0; i < lightParticlesCount; i++)
            {
                bs.Position = basePos + lightParticlesOffset + (i * RunwayLightVFX.GetSize(rwy.VersionMajor, rwy.VersionMinor));
                RunwayLightVFX lightVFX = RunwayLightVFX.FromStream(bs, rwy.VersionMajor, rwy.VersionMinor);
                rwy.LightParticles.Add(lightVFX);
            }
        }

        return rwy;
    }

    public static long Read32Or64(BinaryStream bs, ushort versionMajor)
    {
        if (versionMajor >= 6)
            return bs.ReadInt64();
        else
            return bs.ReadInt32();
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
            throw new NotImplementedException($"Export not implemented for Runway v{VersionMajor}.{VersionMinor}");

        bs.Position = 0x100;

        long sectorInfoMapOffset = SectorInfos.Count != 0 ? bs.Position : 0;
        WriteSectorInfos(bs, basePos);

        long startingGridOffset = StartingGrid.Count != 0 ? bs.Position : 0;
        WriteStartingGrid(bs);

        long checkpointsOffset = Checkpoints.Count != 0 ? bs.Position : 0;
        WriteCheckpoints(bs);

        long checkpointListOffset = CheckpointList.Count != 0 ? bs.Position : 0;
        WriteCheckpointList(bs);

        long lightDefsOffset = WriteLightDefs(bs);

        long lightSetsOffset = LightSets.Sets.Count != 0 ? bs.Position : 0;
        WriteLightSets(bs);

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

        long pitStopsAdjacentsOffset = WritePitStopsAdjacents(bs);

        long lightParticlesOffset = WriteLightParticles(bs);

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
        bs.WriteUInt32(UnkVal3);

        bs.WriteUInt32((uint)SectorInfos.Count);
        bs.WriteUInt32((uint)(sectorInfoMapOffset - basePos));
        bs.WriteUInt32((uint)StartingGrid.Count);
        bs.WriteUInt32((uint)(startingGridOffset - basePos));
        bs.WriteUInt32((uint)Checkpoints.Count);
        bs.WriteUInt32((uint)(checkpointsOffset - basePos));
        bs.WriteUInt32((uint)CheckpointList.Count);
        bs.WriteUInt32((uint)(checkpointListOffset - basePos));
        bs.WriteUInt32((uint)LightDefs.Count);
        bs.WriteUInt32((uint)(lightDefsOffset - basePos));
        bs.WriteUInt32((uint)LightSets?.Sets?.Count != 0 ? (uint)(LightSets.Sets.Count + 1u) : 0u);
        bs.WriteUInt32((uint)(lightSetsOffset - basePos));
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
        bs.WriteUInt32((uint)(boundaryFacesOffset - basePos));
        bs.WriteUInt32((uint)PitStops.Count);
        bs.WriteUInt32((uint)(pitStopsOffset - basePos));

        bs.Position += 0x0C;
        bs.WriteUInt32((uint)(pitStopsAdjacentsOffset - basePos));
        bs.Position += 4;
        bs.WriteUInt32((uint)(lightParticlesOffset - basePos));
    }

    private void WriteSectorInfos(BinaryStream bs, long baseRwyPos)
    {
        long offsetsPos = bs.Position;
        long lastDataPos = bs.Position + (SectorInfos.Count * 0x08);

        for (int i = 0; i < SectorInfos.Count; i++)
        {
            RunwaySector sectorInfo = SectorInfos[i];
            bs.Position = offsetsPos + (i * 0x08);
            bs.WriteUInt32((uint)sectorInfo.SectorVCoords.Count);
            bs.WriteUInt32((uint)(lastDataPos - baseRwyPos));

            sectorInfo.ToStream(bs, VersionMajor, VersionMinor);
            lastDataPos = bs.Position;
        }

        bs.Align(0x10, grow: true);
    }

    private void WriteStartingGrid(BinaryStream bs)
    {
        for (int i = 0; i < StartingGrid.Count; i++)
        {
            RunwayStartingGridPosition startingGridPos = StartingGrid[i];
            startingGridPos.ToStream(bs, VersionMajor, VersionMinor);
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

    private long WriteLightDefs(BinaryStream bs)
    {
        // Write count before data (its read that way)
        if (LightDefs.Count > 0)
        {
            if (bs.Position % 0x10 >= 4) // Try to write using padding if possible
            {
                bs.Align(0x10, grow: true);
                bs.Position -= 4;
                bs.WriteInt32(LightDefs.Count);
            }
            else // Start new padding + write count
            {
                bs.WriteInt32(0);
                bs.WriteInt32(0);
                bs.WriteInt32(0);
                bs.WriteInt32(LightDefs.Count);
            }
        }

        long offset = LightDefs.Count != 0 ? bs.Position : 0;
        for (int i = 0; i < LightDefs.Count; i++)
        {
            RunwayLightDefinition lightDef = LightDefs[i];
            lightDef.ToStream(bs, VersionMajor, VersionMinor);
        }

        bs.Align(0x10, grow: true);
        return offset;
    }

    private void WriteLightSets(BinaryStream bs)
    {
        LightSets.ToStream(bs, VersionMajor, VersionMinor);
        bs.Align(0x08, grow: true);
    }

    private void WriteCheckpointList(BinaryStream bs)
    {
        for (int i = 0; i < CheckpointList.Count; i++)
        {
            ushort chkIdx = CheckpointList[i];
            bs.WriteUInt16(chkIdx);
        }
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
            vert.ToStream(bs, this);
        }

        bs.Align(0x10, grow: true);
    }

    private void WriteBoundaryFaces(BinaryStream bs)
    {
        for (int i = 0; i < BoundaryFaces.Count; i++)
        {
            bs.WriteUInt16(BoundaryFaces[i]);
        }

        bs.Align(0x10, grow: true);
    }

    private void WritePitStops(BinaryStream bs)
    {
        for (int i = 0; i < PitStops.Count; i++)
        {
            Vec3R pitStopPos = PitStops[i];
            pitStopPos.ToStream(bs);
        }

         // No align
    }

    private long WritePitStopsAdjacents(BinaryStream bs)
    {
        if (PitStopAdjacents.Count > 0)
        {
            // Write count before data (its read that way)
            if (bs.Position % 0x10 >= 4) // Try to write using padding if possible
            {
                bs.Align(0x10, grow: true);
                bs.Position -= 4;
                bs.WriteInt32(PitStopAdjacents.Count);
            }
            else // Start new padding + write count
            {
                bs.WriteInt32(0);
                bs.WriteInt32(0);
                bs.WriteInt32(0);
                bs.WriteInt32(PitStopAdjacents.Count);
            }
        }

        long offset = PitStopAdjacents.Count != 0 ? bs.Position : 0;
        for (int i = 0; i < PitStopAdjacents.Count; i++)
        {
            Vec3R pitStopAdjPos = PitStopAdjacents[i];
            pitStopAdjPos.ToStream(bs);
        }

        bs.Align(0x08, grow: true);

        return offset;
    }

    private long WriteLightParticles(BinaryStream bs)
    {
        if (LightParticles.Count > 0)
        {
            // Write count before data (its read that way)
            if (bs.Position % 0x10 >= 4) // Try to write using padding if possible
            {
                bs.Align(0x10, grow: true);
                bs.Position -= 4;
                bs.WriteInt32(LightParticles.Count);
            }
            else // Start new padding + write count
            {
                bs.WriteInt32(0);
                bs.WriteInt32(0);
                bs.WriteInt32(0);
                bs.WriteInt32(LightParticles.Count);
            }
        }
        

        long offset = LightParticles.Count != 0 ? bs.Position : 0;
        for (int i = 0; i < LightParticles.Count; i++)
        {
            RunwayLightVFX lightVFX = LightParticles[i];
            lightVFX.ToStream(bs, VersionMajor, VersionMinor);
        }

        bs.Align(0x10, grow: true);

        return offset;
    }

    public static double DecodeAVXFloat(int value, int axisPow)
    {
        var emptyVec = Vector128.Create(0d, 0d);
        var valVec = Avx.ConvertScalarToVector128Double(emptyVec, value);
        var powVec = Avx.ConvertScalarToVector128Double(emptyVec, 1 << axisPow);
        var div = Avx.DivideScalar(valVec, powVec);
        return div.GetElement(0);
    }
}

