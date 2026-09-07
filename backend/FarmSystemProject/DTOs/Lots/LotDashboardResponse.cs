namespace FarmSystemProject.DTOs.Lots;

public class LotDashboardResponse
{
    public int LotId { get; set; }
    
    public int CurrentAlive { get; set; }         
    
    public int EggsCollectedToday { get; set; }   
    
    public int HensNotLayingToday { get; set; }   
    
    public decimal LayingPercentage { get; set; }

    // Dia a que os números se referem. Null quando o lote ainda não tem
    // nenhuma coleta registrada.
    public DateTime? ReferenceDate { get; set; }

    // False quando os números vêm da última coleta registrada, não de hoje.
    public bool IsToday { get; set; }
}