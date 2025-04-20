// IDriverRepository.cs
namespace TransportManager.Repository;

public interface IDriverRepository
{
    Task<List<Driver>> GetAllDriversAsync();
    Task AddDriverAsync(Driver driver);
    Task UpdateDriverAsync(Driver driver);
    Task RemoveDriverAsync(Driver driver);
    Task<Driver?> GetDriverByIdAsync(string id);
}