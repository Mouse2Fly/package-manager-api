public class Package
{
    public int TrackingNumber { get; set; }
    
    public string SenderAdress { get; set; }
    public string SenderName { get; set; }
    public string SenderPhone { get; set; }

    public string RecipientAdress { get; set; }
    public string RecipientName { get; set; }
    public string RecipientPhone { get; set; }

    public string[] CurrentStatus { get; set; }
}

public class PackageTest
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string[] CurrentStatus { get; set; }
}

public class PackageStatusHistory
{
    public int Id { get; set; }
    public string[] Status { get; set; }
    public string[] StatusDate { get; set; }
}