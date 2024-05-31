namespace DiplomaWork.Models
{
    public class Place
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ProductType { get; set; }        
        public string Address { get; set; }
        public string WorkingHours { get; set; }
        public string OwnershipForm { get; set; }
        public int HostId { get; set; }
    }    
}
