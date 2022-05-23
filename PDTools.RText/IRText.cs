using System.Collections.Generic;

namespace PDTools.RText
{
    public interface IRText
    {
        void Save(string filePath);

        List<IRTextCategory> GetCategories();
    }
}
