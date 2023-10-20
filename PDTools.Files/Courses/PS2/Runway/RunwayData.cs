using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.IO;

using PDTools.Files;
using PDTools.Utils;

using Syroot.BinaryData;

namespace PDTools.Files.Courses.PS2.Runway;

public class RunwayData
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
    public Vector3[] Bounds = default;

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

    public float StartVCoord { get; set; }
    public float GoalVCoord { get; set; }

    public List<RunwayCheckpoint> Checkpoints { get; set; } = new();
    public List<short> CheckpointLookupIndices { get; set; } = new();
    public List<RunwayRoadVert> Vertices { get; set; } = new();
    public List<RunwayRoadTri> Tris { get; set; } = new();
    public List<RunwayCluster> Clusters { get; set; } = new();

    public byte TreeMaxDepth { get; set; }
    public Node Root { get; set; }

    public const int RNW4_BE = 0x524E5734;
    public const int RNW4_LE = 0x34574E52;

    public uint Magic { get; set; }

    public static RunwayData FromStream(Stream stream)
    {
        BinaryStream bs = new BinaryStream(stream);
        long basePos = bs.Position;

        RunwayData rwy = new RunwayData();
        rwy.Magic = bs.ReadUInt32();

        if (rwy.Magic == RNW4_BE)
            bs.ByteConverter = ByteConverter.Big;
        else if (rwy.Magic == RNW4_LE)
            bs.ByteConverter = ByteConverter.Little;
        else
            throw new InvalidDataException("Unsupported runway format.");

        bs.ReadInt32(); // Reloc Pointer
        bs.ReadUInt32(); // Reloc Size
        rwy.Flags = bs.ReadUInt32();
        uint unk = bs.ReadUInt32();
        rwy.TrackV = bs.ReadSingle();
        rwy.StartVCoord = bs.ReadSingle();
        rwy.GoalVCoord = bs.ReadSingle();

        rwy.Bounds = new Vector3[]
        {
            new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle()),
            new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle())
        };

        short checkpointListCount = bs.ReadInt16();
        short unkCount = bs.ReadInt16();
        short checkpointCount = bs.ReadInt16();
        short checkpointLookupIndicesCount = bs.ReadInt16();
        bs.ReadInt32();
        short gadgetsCount = bs.ReadInt16();
        short roadVerticesCount = bs.ReadInt16();
        short roadTriCount = bs.ReadInt16();
        short clusterCount = bs.ReadInt16();
        rwy.TreeMaxDepth = bs.Read1Byte();

        bs.Position = 0x60;
        int checkpointsOffset = bs.ReadInt32();
        int checkpointLookupIndicesOffset = bs.ReadInt32();
        int lightSetsOffset = bs.ReadInt32();
        int unkOffset3 = bs.ReadInt32();
        int gadgetsOffset = bs.ReadInt32();
        int roadVertsOffset = bs.ReadInt32();
        int roadTrisOffset = bs.ReadInt32();
        int clustersOffset = bs.ReadInt32();
        int traversalDataOffset = bs.ReadInt32();

        for (int i = 0; i < checkpointCount; i++)
        {
            bs.Position = basePos + checkpointsOffset + (i * RunwayCheckpoint.GetSize());
            var vert = RunwayCheckpoint.FromStream(bs);
            rwy.Checkpoints.Add(vert);
        }

        for (int i = 0; i < checkpointLookupIndicesCount; i++)
        {
            bs.Position = basePos + checkpointLookupIndicesOffset + (i * sizeof(short));
            rwy.CheckpointLookupIndices.Add(bs.ReadInt16());
        }

        for (int i = 0; i < roadVerticesCount; i++)
        {
            bs.Position = basePos + roadVertsOffset + (i * RunwayRoadVert.GetSize());
            var vert = RunwayRoadVert.FromStream(bs);
            rwy.Vertices.Add(vert);
        }

        for (int i = 0; i < roadTriCount; i++)
        {
            bs.Position = basePos + roadTrisOffset + (i * RunwayRoadTri.GetSize());
            var tri = RunwayRoadTri.FromStream(bs);
            rwy.Tris.Add(tri);
        }

        for (int i = 0; i < clusterCount; i++)
        {
            bs.Position = basePos + clustersOffset + (i * RunwayCluster.GetSize());
            var cluster = RunwayCluster.FromStream(bs);
            rwy.Clusters.Add(cluster);
        }

        bs.Position = traversalDataOffset;
        rwy.Root = TraverseRead(bs, rwy.TreeMaxDepth - 1);

        return rwy;
    }

    private static Node TraverseRead(BinaryStream stream, int depthLeft)
    {
        if (depthLeft < 0)
            return null;

        var node = new Node();
        byte data = stream.Read1Byte();
        node.Axis = (byte)(data >> 6);
        node.Value = ((float)(data & 0b111111) + 0.5f) * 0.015625f;

        node.Left = TraverseRead(stream, depthLeft - 1);
        node.Right = TraverseRead(stream, depthLeft - 1);

        return node;
    }

    /// <summary>
    /// Searches the runway for a possible collision at the provided coordinates (Y will be within bounds)
    /// </summary>
    /// <param name="result"></param>
    /// <param name="pos"></param>
    // GT4O US 0x293F90
    public bool search(out RunwayResult result, Vector3 pos)
    {
        if (pos.Y >= Bounds[0].Y)
        {
            Vector3 startPoint;
            startPoint.X = pos.X;
            startPoint.Y = Bounds[1].Y >= pos.Y ? pos.Y : Bounds[1].Y;
            startPoint.Z = pos.Z;

            Vector3 endPoint;
            endPoint.X = pos.X;
            endPoint.Y = Bounds[0].Y;
            endPoint.Z = pos.Z;

            int depth = TreeMaxDepth - 1;
            return traverse(out result, Bounds, startPoint, endPoint, Root, depth, 0);
        }
        else
            result = null;

        return false;
    }

    // GT4O US 0x294128
    // Finds the cluster linked to provided positions?
    public bool traverse(out RunwayResult result, Span<Vector3> bounds, Vector3 startPoint, Vector3 endPoint, Node node, int depth, short clusterIndex)
    {
        if (depth < 0)
            return checkHit(out result, startPoint, endPoint, clusterIndex);

        int axis = node.Axis;
        float axisBoundsMin = bounds[0].GetAxis(axis);
        float axisBoundsMax = bounds[1].GetAxis(axis);

        float axisV1 = startPoint.GetAxis(axis);
        float axisV2 = endPoint.GetAxis(axis);

        float pos = MathUtils.Lerp(axisBoundsMin, axisBoundsMax, node.Value); // (axisBoundsMin * (1.0f - node.Value)) + (axisBoundsMax * node.Value);
        float v20 = axisV1 - pos;

        Node nextNode;

        clusterIndex *= 2;

        Span<Vector3> nextBounds = stackalloc Vector3[2];
        if ((axisV1 - pos) * (axisV2 - pos) >= 0.0f)
        {
            nextBounds[0] = bounds[0];
            nextBounds[1] = bounds[1];

            if (v20 < 0.0f)
            {
                nextNode = node.Left;
                depth--;

                nextBounds[1].SetAxis(axis, pos);
            }
            else
            {
                nextNode = node.Right;
                depth--;

                nextBounds[0].SetAxis(axis, pos);
                clusterIndex++;
            }
        }
        else
        {
            throw new NotImplementedException("Reverse/Implement this part");
        }

        return traverse(out result, nextBounds, startPoint, endPoint, nextNode, depth, clusterIndex);
    }

    /// <summary>
    /// Returns whether a vector hits collision within a specific cluster, and returns additional information about the point that was hit
    /// </summary>
    /// <param name="runway"></param>
    /// <param name="result"></param>
    /// <param name="startPoint"></param>
    /// <param name="endPoint"></param>
    /// <param name="clusterIndex"></param>
    /// <returns></returns>
    // GT4O US 0x294508
    public bool checkHit(out RunwayResult result, Vector3 startPoint, Vector3 endPoint, short clusterIndex)
    {
        RunwayCluster cluster = Clusters[clusterIndex];

        Vector3 vecDiff = endPoint - startPoint;

        // Results
        Vector3 closest = Vector3.Zero;
        short resTri = -1;
        float v20 = float.NaN;

        float val1 = 0.0f, val2 = 0.0f, val3 = 0.0f;

        // Iterate through all tris/faces for this cluster, check if we are colliding against any of them
        for (int i = 0; i < cluster.TriIndices.Length; i++)
        {
            short currentTriIndex = cluster.TriIndices[i];
            RunwayRoadTri tri = Tris[currentTriIndex];

            RunwayRoadVert p1 = Vertices[tri.Vert1];
            RunwayRoadVert p2 = Vertices[tri.Vert2];
            RunwayRoadVert p3 = Vertices[tri.Vert3];

            // Check 1 to 2
            Vector3 a = p1.Vertex - startPoint;
            Vector3 b = p2.Vertex - startPoint;
            Vector3 c = p3.Vertex - startPoint;

            Vector3 crossed = Vector3.Cross(vecDiff, a); // GT4Course::'anonymous_namespace'::outerProduct
            float dot1 = Vector3.Dot(b, crossed); // GT4Course::'anonymous_namespace'::innerProduct

            if (dot1 <= 0.0f)
            {
                // Check 2 to 3
                float dot2 = Vector3.Dot(c, crossed);

                if (0.0f <= dot2)
                {
                    crossed = Vector3.Cross(b, c);
                    float dot3 = Vector3.Dot(vecDiff, crossed);
                    if (dot3 <= 0.0f)
                    {
                        b = p2.Vertex - p1.Vertex;
                        c = p3.Vertex - p1.Vertex;

                        crossed = Vector3.Cross(b, c);
                        float dot4 = Vector3.Dot(crossed, vecDiff);

                        float val = 0.0f;
                        if (dot4 != 0.0f)
                        {
                            var last = Vector3.Dot(crossed, a);
                            if (0.0f < last || last < dot4)
                                continue;

                            val = last / dot4;
                        }

                        if (float.IsNaN(v20) || val <= v20)
                        {
                            closest = crossed;
                            resTri = currentTriIndex;
                            v20 = val;

                            val1 = dot1;
                            val2 = -dot2;
                            val3 = dot3;
                        }
                    }
                }
            }
        }

        if (resTri >= 0)
        {
            result = new RunwayResult();

            result.HitPoint = new Vector3(
                ((endPoint.X - startPoint.X) * v20) + startPoint.X,
                ((endPoint.Y - startPoint.Y) * v20) + startPoint.Y,
                ((endPoint.Z - startPoint.Z) * v20) + startPoint.Z
            );
            result.Cluster = clusterIndex;
            result.TriIndex = resTri;

            var tri = Tris[resTri];
            RunwayRoadVert p1 = Vertices[tri.Vert1];
            RunwayRoadVert p2 = Vertices[tri.Vert2];
            RunwayRoadVert p3 = Vertices[tri.Vert3];

            Vector3 adjusted = result.HitPoint;
            for (int axis = 0; axis < 3; axis++)
            {
                float f1 = p1.Vertex.GetAxis(axis);
                float f2 = p2.Vertex.GetAxis(axis);
                float f3 = p3.Vertex.GetAxis(axis);
                float res = result.HitPoint.GetAxis(axis);

                float min, max;
                if (f1 >= f2)
                {
                    max = MathF.Max(f1, f3);
                    min = MathF.Min(f2, f3);
                }
                else
                {
                    max = MathF.Max(f2, f3);
                    min = MathF.Min(f1, f3);
                }

                float axisVal = MathF.Max(min, Math.Min(max, res));
                adjusted.SetAxis(axis, axisVal);
            }
            result.HitPoint = adjusted;

            float combined = val1 + val2 + val3;
            float rsqrt = 1.0f / closest.Length(); // SQRT(closest.X * closest.X + closest.Y * closest.Y + closest.Z * closest.Z);

            float val = (float)p1.Unk2 / 255.0f;
            val = val + (((float)p2.Unk2 / 255.0f - val) * val2 + ((float)p3.Unk2 / 255.0f - val) * val1) / combined;
            result.Unk = Math.Clamp(val, 0.0f, 1.0f); // min.s + max.s

            result.UnkVec = closest * rsqrt;

            val = (float)(p1.Unk3 & 0x7F) / 127.0f;
            val = val + (((float)(p2.Unk3 & 0x7F) / 127.0f - val) * val2 + ((float)(p3.Unk3 & 0x7F) / 127.0f - val) * val1) / combined;
            result.Unk2 = Math.Clamp(val, 0.0f, 1.0f); // min.s + max.s
            result.Unk3 = v20;
            result.Unk4 = -Vector3.Dot(result.UnkVec, p1.Vertex);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Gets the vcoord (meter) given a position.
    /// </summary>
    /// <param name="position">Position on the track.</param>
    /// <param name="hint">"Hint" for searching with the cluster and tri index. If default, will search for that first.</param>
    /// <returns></returns>
    public float getVCoord(Vector3 position, RunwayHint hint)
    {
        if ((hint.Cluster & 0x8000) == -1 || hint.TriIndex == -1)
        {
            search(out RunwayResult result, position);
            if (result.Cluster == -1 || result.TriIndex == -1)
                return 0.0f;

            hint.TriIndex = result.TriIndex;
            hint.Cluster = result.Cluster;
        }

        float lowest = float.NaN;
        float vcoord = 0.0f;

        // We have a cluster, loop through all the checkpoints linked to it
        // We'll use them to calculate the vcoord
        RunwayCluster cluster = Clusters[hint.Cluster];
        for (int i = cluster.CheckpointLookupIndexStart; i < cluster.CheckpointLookupIndexStart + cluster.CheckpointLookupLength; i++)
        {
            var index = CheckpointLookupIndices[i];
            RunwayCheckpoint cp = Checkpoints[index];
            RunwayCheckpoint nextcp = index + 1 != Checkpoints.Count ? Checkpoints[index + 1] : Checkpoints[0];

            // Check each side (aka each rectangle) for a possible hit
            if (QuadSTCompute(out Vector3 result, position, cp.Left, nextcp.Left, cp.Middle, nextcp.Middle) ||
                QuadSTCompute(out result, position, cp.Middle, nextcp.Middle, cp.Right, nextcp.Right))
            {
                float current = MathF.Abs(result.Z);
                if (float.IsNaN(lowest) || current < lowest)
                {
                    vcoord = cp.TrackV;
                    float nextV = nextcp.TrackV;

                    if (nextcp.TrackV < cp.TrackV)
                        nextV = this.TrackV;

                    vcoord += (nextV - vcoord) * result.X;
                    lowest = current;

                    // Loop back
                    if (vcoord > this.TrackV)
                        vcoord -= this.TrackV;
                }
            }
        }

        return vcoord;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    /// <param name="pos">Position</param>
    /// <param name="p1">Rect P1</param>
    /// <param name="p2">Rect P2</param>
    /// <param name="p3">Rect P3</param>
    /// <param name="p4">Rect P4</param>
    /// <returns></returns>
    public static bool QuadSTCompute(out Vector3 result, Vector3 pos, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        float p3p1XDiff = p3.X - p1.X;
        float p3p1ZDiff = p3.Z - p1.Z;
        float p2p4XDiff = p2.X - p4.X;
        float p2p4ZDiff = p2.Z - p4.Z;

        result = default;

        if (p3p1XDiff == 0.0 && p3p1ZDiff == 0.0 && p2p4XDiff == 0.0 && p2p4ZDiff == 0.0f)
            return false;

        float posP1XDiff = pos.X - p1.X;
        float posP1ZDiff = pos.Z - p1.Z;

        float unk = posP1ZDiff * p3p1XDiff - posP1XDiff * p3p1ZDiff;
        if (unk <= 0.0f)
        {
            float p2p1XDiff = p2.X - p1.X;
            float p2p1ZDiff = p2.Z - p1.Z;

            float unk2 = posP1ZDiff * p2p1XDiff - posP1XDiff * p2p1ZDiff;
            if (0.0f <= unk2)
            {
                float posP4XDiff = pos.X - p4.X;
                float posP4ZDiff = pos.Z - p4.Z;

                float unk3 = posP4ZDiff * p2p4XDiff - posP4XDiff * p2p4ZDiff;
                if (unk3 <= 0.0f)
                {
                    float p3p4XDiff = p3.X - p4.X;
                    float p3p4ZDiff = p3.Z - p4.Z;

                    float unk4 = posP4ZDiff * p3p4XDiff - posP4XDiff * p3p4ZDiff;
                    if (0.0f <= unk4)
                    {
                        float v1 = -p3p4XDiff - p2p1XDiff;
                        float v2 = -p3p4ZDiff - p2p1ZDiff;

                        float aa = posP1ZDiff * v1 - posP1XDiff * v2;

                        if (p3p1XDiff == 0.0f && p3p1ZDiff == 0.0f)
                        {
                            result.X = -unk2 / aa;
                            result.Y = aa / (p2p1ZDiff * v1 - p2p1XDiff - v2);
                        }
                        else
                        {
                            float unk5 = p2p1ZDiff * p3p1XDiff - p2p1XDiff * p3p1ZDiff;
                            float unk6 = aa + unk5;
                            float unk7 = MathF.Sqrt(unk6 * unk6 + unk2 * 4.0f * (p3p1ZDiff * v1 - p3p1XDiff - v2));
                            result.X = (unk + unk) / ((unk5 - aa) - unk7); // Progress between previous and next cp (0.0 to 1.0)
                            result.Y = (unk2 * -2.0f) / (unk6 - unk7);
                        }

                        result.Z = pos.Y - ((1.0f - result.X) * (1.0f - result.Y) * p1.Y 
                                                + result.X * (1.0f - result.Y) * p2.Y 
                                                + (1.0f - result.X) * result.Y * p3.Y 
                                                + result.X * result.Y * p4.Y);

                        return true;
                    }
                }
            }
        }

        return false;
    }

    public class Node
    {
        public byte Axis;
        public float Value;
        public Node Left;
        public Node Right;
    }

    public class RunwayResult
    {
        public Vector3 HitPoint { get; set; } // 0x00
        public byte TriUnk { get; set; } // 0x0C
        public byte TriUnk2 { get; set; } // 0x10
        public float Unk { get; set; } // 0x14
        public float Unk2 { get; set; } // 0x18
        public Vector3 UnkVec { get; set; } // 0x1C
        public float Unk4 { get; set; } // 0x28
        public float Unk3 { get; set; } // 0x2C

        public short TriIndex { get; set; } = -1; // 0x30
        public short Cluster { get; set; } = -1; // 0x32
    }

    public struct RunwayHint
    {
        public short TriIndex;
        public short Cluster;

        public RunwayHint(short triIndex, short cluster)
        {
            TriIndex = triIndex;
            Cluster = cluster;
        }

        public RunwayHint()
        {
            TriIndex = -1;
            Cluster = -1;
        }
    }
}

