namespace PDTools.Structures.PS3.MGameParameter
{
    public class EventCategory
    {
        public string name { get; set; }
        public int typeID { get; set; }

        public EventCategory()
        {
            name = "";
            typeID = -1;
        }

        public EventCategory(string name, int typeID)
        {
            this.name = name;
            this.typeID = typeID;
        }
    }
}
