namespace FarmSystemProject.DTOs.Lots;

public class LotDashboardResponse
{
    public int LotId { get; set; }
    
    public int CurrentAlive { get; set; }         
    
    public int EggsCollectedToday { get; set; }   
    
    public int HensNotLayingToday { get; set; }   
    
    public decimal LayingPercentage { get; set; } 
}