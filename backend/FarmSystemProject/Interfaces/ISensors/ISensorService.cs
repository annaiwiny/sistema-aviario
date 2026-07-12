using FarmSystemProject.DTOs.Sensors;
using FarmSystemProject.Models.Sensors;

namespace FarmSystemProject.Interfaces.ISensors;

public interface ISensorService
{
    Task<List<SensorSummary>> GetSensorsSummary(int lotId, int ownerId);
    Task RegisterEsp32Readings(Esp32Payload payload);
    Task<Sensor> Create(CreateSensor request);
    Task<List<SensorReading>> GetReadingsHistory(int lotId, int ownerId, SensorType type, DateTime from);
}
