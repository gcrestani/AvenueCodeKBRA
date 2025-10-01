namespace AvenueCodeKBRA.Models;

public class ApplicationSettings
{
    public DateTime CutoffDate { get; set; }
    
    /// <summary>
    /// Configuration for XML output formatting
    /// </summary>
    public XmlOutputSettings XmlOutput { get; set; } = new();
}

public class XmlOutputSettings
{
    /// <summary>
    /// The sequence number for person groups in XML output
    /// </summary>
    public int PersonGroupSequence { get; set; } = 1;
    
    /// <summary>
    /// The name for person groups in XML output
    /// </summary>
    public string PersonGroupName { get; set; } = "Analytical Contacts";
}
