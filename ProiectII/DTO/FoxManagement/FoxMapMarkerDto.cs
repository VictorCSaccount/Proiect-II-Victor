namespace ProiectII.DTO.FoxManagement
{
    public class FoxMapMarkerDto
    {
        public uint FoxId { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string StatusName { get; set; }
        //public string PinColor => StatusName == "Under Treatment" ? "red" : "green";
    }
}