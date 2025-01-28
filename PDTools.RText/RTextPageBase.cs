using System.Collections.Generic;
using System.Linq;

using PDTools.Utils;

using Syroot.BinaryData;

namespace PDTools.RText;

public abstract class RTextPageBase
{
    public string Name { get; set; }

    // We need to have them ordered, game uses binary searching
    public SortedDictionary<string, RTextPairUnit> PairUnits { get; set; }
        = new SortedDictionary<string, RTextPairUnit>(AlphaNumStringComparer.Default);

    public void EditRow(int id, string label, string data)
    {
        if (!PairExists(label))
            return;

        PairUnits[label] = new RTextPairUnit(id, label, data);
    }

    public int AddRow(int id, string label, string data)
    {
        var index = PairUnits.Count;
        PairUnits.Add(label, new RTextPairUnit(id, label, data));
        return index;
    }

    public void DeleteRow(string label)
        => PairUnits.Remove(label);

    public int GetLastId()
        => PairUnits.Max(p => p.Value.ID);

    public bool PairExists(string label)
        => PairUnits.ContainsKey(label);

    public void AddPairs(Dictionary<string, string> pairs)
    {
        int lastId = GetLastId();
        foreach (var elem in pairs)
        {
            if (PairUnits.TryGetValue(elem.Key, out RTextPairUnit pair))
                pair.Value = elem.Value;
            else
                AddRow(++lastId, elem.Key, elem.Value);
        }
    }

    public abstract void Read(BinaryStream reader);
    public abstract void Write(BinaryStream writer, int baseOffset, int baseDataOffset);
}
