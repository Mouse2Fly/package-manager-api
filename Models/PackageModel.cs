using System.ComponentModel.DataAnnotations;

namespace PackageModels
{ 
    public class Package
    {
        [Key]
        public int TrackingNumber { get; set; }

        public string SenderAdress { get; set; }
        public string SenderName { get; set; }
        public string SenderPhone { get; set; }

        public string RecipientAdress { get; set; }
        public string RecipientName { get; set; }
        public string RecipientPhone { get; set; }

        public string[] CurrentStatus { get; set; }

        public DateTime CreationDate { get; set; }

        public string[] StatusHistory { get; set; }
    }

    public class PackagePartial
    {
        public string SenderAdress { get; set; }
        public string SenderName { get; set; }
        public string SenderPhone { get; set; }

        public string RecipientAdress { get; set; }
        public string RecipientName { get; set; }
        public string RecipientPhone { get; set; }
    }

    public class PackageStatus
    {
        public string NewStatus { get; set; }
    }
}