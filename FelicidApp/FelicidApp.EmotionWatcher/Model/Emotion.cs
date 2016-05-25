using Microsoft.WindowsAzure.Storage.Table;

public class Emotion: TableEntity
{
    public string deviceid { get; set; }
    public string mood { get; set; }
    public double heartrate { get; set; }
    public string done { get; set; }
    public string id { get; set; }
}