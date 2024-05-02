using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;

using PDTools.Utils;

namespace PDTools.SpecDB.Core;

public class DatabaseTableHuffmanTree
{
    private Dictionary<byte, int> _freqTable = new Dictionary<byte, int>();
    private Node _root;

    public Dictionary<byte, Node> Leaves { get; private set; } = new();
    public LookupEntry[] LookupTable { get; set; } = new LookupEntry[0x100];

    public DatabaseTableHuffmanTree()
    {

    }

    public void BuildTreeAndLookupTable()
    {
        var nodes = new PriorityQueue<Node, int>();
        foreach (var t in _freqTable)
        {
            var node = new Node();
            node.Data = t.Key;
            node.Weight = t.Value;
            nodes.Enqueue(node, node.Weight);
        }

        // Build the huffman tree
        BuildTree(nodes);

        // Flatten our tree, assign codes to each node
        if (_root.Left == null && _root.Right == null)
        {
            var n = nodes.Peek();
            Leaves.Add(n.Data.Value, n);

            n.Code = 0;
            n.BitSizeCode = 3;
        }
        else
        {
            FlattenTreeAssignCodes(_root, 0, 0, "");
        }

        // Order our leaves by code bit size, then code - for bsearch
        Leaves = Leaves.OrderBy(e => e.Value.BitSizeCode).ThenBy(e => e.Value.Code)
            .ToDictionary(e => e.Key, e => e.Value);

        // Create lookup table (no idea how this really works, just based on unused bits & patterns?)
        // This is related I think https://stackoverflow.com/a/2306125
        foreach (Node leaf in Leaves.Values)
        {
            if (leaf.BitSizeCode > 8)
                continue;

            int code = (int)leaf.Code;
            int bit = 1 << leaf.BitSizeCode;

            while (code < 0x100)
            {
                LookupTable[code] = new LookupEntry();
                LookupTable[code].Data = (byte)leaf.Data;
                LookupTable[code].BitSizeCode = leaf.BitSizeCode;
                code += bit;
            }
        }
    }

    private void BuildTree(PriorityQueue<Node, int> nodes)
    {
        var root = new Node();
        while (nodes.Count > 1)
        {
            var x = nodes.Dequeue();
            var y = nodes.Dequeue();

            var f = new Node();
            f.Weight = x.Weight + y.Weight;
            f.Left = x;
            f.Right = y;
            root = f;
            nodes.Enqueue(f, f.Weight);
        }

        _root = root;
    }

    public void IncrementFrequencyOfByte(byte value)
    {
        if (!_freqTable.ContainsKey(value))
        {
            _freqTable.Add(value, 1);
        }
        else
        {
            _freqTable[value]++;
        }
    }

    public void IncrementFrequencyOfBytes(byte[] values)
    {
        for (var i = 0; i < values.Length; i++)
        {
            var value = values[i];
            if (!_freqTable.ContainsKey(value))
            {
                _freqTable.Add(value, 1);
            }
            else
            {
                _freqTable[value]++;
            }
        }
    }

    public void ReorderFrequencyTable()
    {
        _freqTable = _freqTable.OrderBy(e => e.Value).ToDictionary(e => e.Key, e => e.Value);
    }

    private void FlattenTreeAssignCodes(Node node, uint i, uint depth, string s)
    {
        node.Code = i;
        node.BitSizeCode = (byte)Math.Max(3, depth); // Minimum 3 bits
        node.CodeStr = s;

        if (node.Left == null && node.Right == null && node.Data != null)
        {
            Leaves.Add(node.Data.Value, node);
            return;
        }

        FlattenTreeAssignCodes(node.Left, i, depth + 1, s + "0");
        FlattenTreeAssignCodes(node.Right, i | (uint)(1 << (int)depth), depth + 1, s + "1");
    }

    /// <summary>
    /// Gets the highest bit index for a value
    /// </summary>
    /// <param name="value">Value to get the highest bit for</param>
    /// <returns></returns>
    public static uint GetHighestBitIndex(uint value)
    {
        return (uint)(32u - BitOperations.LeadingZeroCount(value));
    }

    public void EncodeValueToStream(ref BitStream bs, byte value)
    {
        if (!Leaves.TryGetValue(value, out Node node))
        {
            throw new Exception("Not found leaf");
        }

        bs.WriteBits(node.Code, node.BitSizeCode);
    }

    public uint GetCodeCountForRow(byte[] row)
    {
        uint res = 0;
        for (var i = 0; i < row.Length; i++)
        {
            if (row[i] == 0)
                continue;

            res++;
        }

        return res;
    }

    public Node GetNodeForValue(byte value)
    {
        if (!Leaves.TryGetValue(value, out Node node))
        {
            throw new Exception("Not found leaf");
        }

        return node;
    }
}

public class Node
{
    public int Weight { get; set; }
    public byte? Data { get; set; }

    public byte BitSizeCode { get; set; }
    public uint Code { get; set; }

    public Node Left { get; set; }
    public Node Right { get; set; }

    public string CodeStr { get; set; }

    public override string ToString()
    {
        string str = Data.HasValue ? "Node" : "Leaf";
        if (Data != null)
            str += $" - 0x{Data:X2} (Code: {CodeStr}, Size: {BitSizeCode} bits)";

        return str;
    }

    public int Compare(Node x, Node y)
    {
        return x.Weight - y.Weight;
    }
}

class MyComparator : IComparer<Node>
{
    public int Compare(Node x, Node y)
    {
        return x.Weight - y.Weight;
    }
}

public class LookupEntry
{
    public byte Data { get; set; }
    public byte BitSizeCode { get; set; }

    public override string ToString()
    {
        return $"Value: 0x{Data:X2} - {BitSizeCode} bits";
    }
}
