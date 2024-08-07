using System;

namespace TransportManager.Models;

public class Maintenance : BaseEntity
{
    private string _vehicleId = string.Empty;
    private DateTime _scheduledDate;
    private string _description = string.Empty;
    private MaintenanceStatus _status;
    private double _cost;

    public string VehicleId
    {
        get => _vehicleId;
        set => SetProperty(ref _vehicleId, value);
    }

    public DateTime ScheduledDate
    {
        get => _scheduledDate;
        set => SetProperty(ref _scheduledDate, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public MaintenanceStatus Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    public double Cost
    {
        get => _cost;
        set => SetProperty(ref _cost, value);
    }

    public override void Update()
    {
        base.Update();
        // Adicione aqui qualquer lógica específica de atualização para manutenções
    }
}

public enum MaintenanceStatus
{
    Scheduled,
    InProgress,
    Completed,
    Cancelled
}