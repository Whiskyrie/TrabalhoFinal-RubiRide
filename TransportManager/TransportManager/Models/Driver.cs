namespace TransportManager.Models;

public class Driver : BaseEntity
{
    private string _licenseNumber = string.Empty;
    private DateTime _licenseExpirationDate;
    private DriverStatus _status;

    public string LicenseNumber
    {
        get => _licenseNumber;
        set => SetProperty(ref _licenseNumber, value);
    }

    public DateTime LicenseExpirationDate
    {
        get => _licenseExpirationDate;
        set => SetProperty(ref _licenseExpirationDate, value);
    }

    public DriverStatus Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    public bool IsLicenseValid => LicenseExpirationDate > DateTime.Now;

    public override void Update()
    {
        base.Update();
        // Adicione aqui qualquer lógica específica de atualização para motoristas
    }
}

public enum DriverStatus
{
    Available,
    OnDuty,
    OnLeave,
    Inactive
}