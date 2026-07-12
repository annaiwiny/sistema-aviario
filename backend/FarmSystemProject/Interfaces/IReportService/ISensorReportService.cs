using FarmSystemProject.Models.Sensors;

namespace FarmSystemProject.Interfaces.IReportService;

public interface ISensorReportService
{
    Task<byte[]> GenerateSensorMonitoringReport(int lotId, int ownerId, SensorType type);
    byte[] BuildReportFromReadings(
        SensorType type,
        List<SensorReading> lastHourReadings,
        List<SensorReading> last24hReadings,
        List<SensorReading> last7dReadings);
}
