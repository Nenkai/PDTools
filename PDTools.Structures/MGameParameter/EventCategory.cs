
namespace PDTools.Structures.MGameParameter
{
    public class EventCategory
    {
        public string name { get; set; }
        public int typeID { get; set; }

        public EventCategory()
        {
            this.name = "";
            this.typeID = -1;
        }

        public EventCategory(string name, int typeID)
        {
            this.name = name;
            this.typeID = typeID;
        }
    }
}
