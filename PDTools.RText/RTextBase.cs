using System.Collections.Generic;
using System.Linq;

using PDTools.Utils;

namespace PDTools.RText;

public abstract class RTextBase
{
    protected SortedDictionary<string, RTextPageBase> _pages { get; set; } 
        = new SortedDictionary<string, RTextPageBase>(AlphaNumStringComparer.Default);

    public SortedDictionary<string, RTextPageBase> GetPages()
    {
        return _pages;
    }

    public abstract void Read(byte[] buffer);

    /* Note & possible minor TODO - use optimized string table:
     * While we are saving strings ordered, we still aren't storing the strings optimally
     * As in - different entry offsets may point to the same string offsets to save on size
     * Most specifically noticeable in car description rt2's
     */
    public abstract void Save(string filePath);
}
