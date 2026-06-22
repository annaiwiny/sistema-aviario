using FarmSystemProject.DTOs.Sensors;

namespace FarmSystemProject.Interfaces.ISensors;

public interface ISensorService
{
    Task<List<SensorSummary>> GetSensorsSummary(int lotId, int ownerId);
    Task RegisterEsp32Readings(Esp32Payload payload);
}
