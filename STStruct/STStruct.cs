using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Syroot.BinaryData;
using Syroot.BinaryData.Memory;

using PDTools.STStruct.Nodes;

namespace PDTools.STStruct
{
    public class STStruct
    {
        public string[] NodeNames;

        public NodeBase RootNode { get; set; }

        public static STStruct Read(byte[] data)
        {
            var sr = new SpanReader(data, Syroot.BinaryData.Core.Endian.Big);
            int basePos = sr.Position;

            byte version = sr.ReadByte(); // 0E
            int startPos = sr.ReadInt32();

            int baseTreeOffset = sr.Position;
            sr.Position = basePos + startPos;

            var tree = new STStruct();
            tree.NodeNames = new string[sr.Read7BitUInt32()];
            for (int i = 0; i < tree.NodeNames.Length; i++)
                tree.NodeNames[i] = sr.ReadString1();

            sr.Position = baseTreeOffset;
            tree.RootNode = tree.ReadNode(ref sr, null);

            return tree;
        }

        public NodeBase GetField(string path)
        {
            if (!(RootNode is STObject obj))
                return null;

            string[] attributes = path.Split('.');

            int currentIndex = 0;
            return Find(obj.Child, attributes, ref currentIndex);
        }

        private NodeBase Find(NodeBase currentNode, string[] attrs, ref int currentIndex)
        {
            var map = currentNode as STMap;
            if (map.Elements.TryGetValue(attrs[currentIndex], out NodeBase val))
            {
                if (currentIndex == attrs.Length - 1)
                    return val; // Found it

                currentIndex++;
                return Find(val, attrs, ref currentIndex);
            }

            return null;
        }

        private NodeBase ReadNode(ref SpanReader sr, NodeBase parent)
        {
            NodeBase currentNode = null;

            var nodeType = (NodeType)sr.ReadByte();
            switch (nodeType)
            {
                case NodeType.Null:
                    currentNode = new STObjectNull();
                    break;
                case NodeType.Short:
                    currentNode = new STShort(sr.ReadInt16());
                    break;
                case NodeType.SByte:
                    currentNode = new STSByte(sr.ReadSByte());
                    break;
                case NodeType.Int:
                    currentNode = new STInt(sr.ReadInt32());
                    break;
                case NodeType.UInt:
                    currentNode = new STUInt(sr.ReadUInt32());
                    break;
                case NodeType.Float:
                    currentNode = new STFloat(sr.ReadSingle());
                    break;
                case NodeType.Long:
                    currentNode = new STLong(sr.ReadInt64());
                    break;
                case NodeType.Double:
                    currentNode = new STULong(sr.ReadUInt64());
                    break;
                case NodeType.MBlob:
                    currentNode = new MBlob(sr.ReadBytes(sr.ReadInt32()));
                    break;
                case NodeType.Map:
                    currentNode = new STMap();
                    currentNode.Type = nodeType;
                    var map = currentNode as STMap;
                    int childCount = sr.ReadInt32();

                    for (int i = 0; i < childCount; i++)
                    {
                        // Read String then Child = <string, elem>
                        var key = ReadNode(ref sr, currentNode) as STString;
                        var value = ReadNode(ref sr, currentNode);
                        map.Elements.Add(key.Name, value);
                    }
                    break;
                case NodeType.Array:
                    currentNode = new STArray();
                    currentNode.Type = nodeType;
                    var arrayNode = currentNode as STArray;
                    int elemCount = sr.ReadInt32();

                    arrayNode.Elements = new List<NodeBase>(elemCount);
                    for (int i = 0; i < elemCount; i++)
                        arrayNode.Elements.Add(ReadNode(ref sr, currentNode));
                    break;
                case NodeType.String:
                    currentNode = new STString();
                    currentNode.Type = nodeType;
                    var nodeKey = currentNode as STString;

                    uint symbolIndex = sr.Read7BitUInt32();
                    nodeKey.Name = NodeNames[symbolIndex];

                    break;
                case NodeType.Object:
                    currentNode = new STObject();
                    currentNode.Type = nodeType;
                    (currentNode as STObject).Child = ReadNode(ref sr, currentNode);
                    break;
                default:
                    throw new Exception($"Unexpected structure type {nodeType}");
            }

            currentNode.Type = nodeType;
            return currentNode;
        }

        public void Save(string path)
        {
            using (var bs = new BinaryStream(new FileStream(path, FileMode.Create)))
            {
                bs.ByteConverter = ByteConverter.Big;
                bs.WriteInt32(0x18);
                bs.WriteInt64(80009560400);
                bs.Position += 4;

                bs.WriteByte(0x0E);
                bs.Position += 4; // Length
                List<string> keys = new List<string>();
                WriteNode(bs, RootNode, ref keys);
                int keyTableOffset = (int)bs.Length;
                bs.EncodeAndAdvance((uint)keys.Count);
                foreach (var key in keys)
                    bs.WriteString(key, StringCoding.ByteCharCount);
                int totalLen = (int)bs.Position;

                bs.Position = 0x10 + 1;
                bs.WriteInt32(keyTableOffset - 0x10);

                bs.Position = 0x0C;
                bs.WriteInt32(totalLen - 0x10);
            }
        }

        private void WriteNode(BinaryStream bs, NodeBase node, ref List<string> keys)
        {
            bs.WriteByte((byte)node.Type);
            switch (node)
            {
                case STObjectNull n:
                    break;

                case STSByte @sbyte:
                    bs.WriteSByte(@sbyte.Value); break;
                case STByte @byte:
                    bs.WriteByte(@byte.Value); break;
                case STShort @short:
                    bs.WriteInt16(@short.Value); break;
                case STInt @int:
                    bs.WriteInt32(@int.Value);
                    if (@int.KeyConfigNode != null)
                        WriteNode(bs, @int.KeyConfigNode, ref keys);
                    break;
                case STUInt @uint:
                    bs.WriteUInt32(@uint.Value); break;
                case STLong @long:
                    bs.WriteInt64(@long.Value); break;
                case STULong @ulong:
                    bs.WriteUInt64(@ulong.Value); break;
                case STFloat @float:
                    bs.WriteSingle(@float.Value); break;

                case MBlob @blob:
                    bs.WriteInt32(blob.Data.Length);
                    bs.Write(blob.Data.ToArray());
                    break;

                case STString str:
                    if (!keys.Contains(str.Name))
                        keys.Add(str.Name);
                    int index = keys.IndexOf(str.Name);
                    bs.EncodeAndAdvance((uint)index);
                    break;
                case STMap map:
                    bs.WriteInt32(map.Elements.Count);
                    foreach (var child in map.Elements)
                    {
                        WriteNode(bs, new STString() { Name = child.Key }, ref keys);
                        WriteNode(bs, child.Value, ref keys);
                    }
                    break;
                case STObject @object:
                    WriteNode(bs, @object.Child, ref keys);
                    break;
                case STArray @arr:
                    bs.WriteInt32(@arr.Elements.Count);
                    foreach (var child in @arr.Elements)
                        WriteNode(bs, child, ref keys);
                    break;
                default:
                    throw new Exception("Missing");
            }
        }

    }
}
