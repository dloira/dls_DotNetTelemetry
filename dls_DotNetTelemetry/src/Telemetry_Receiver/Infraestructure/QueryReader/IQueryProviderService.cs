namespace Telemetry_Receiver.Infraestructure.QueryReader
{
    public interface IQueryProviderService
    {
        string? GetQuery(string queryName);
    }
}
