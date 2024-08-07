using System;

namespace TransportManager.Models;

public class Route : BaseEntity
{
    private string _startLocation = string.Empty;
    private string _endLocation = string.Empty;
    private double _distance;
    private TimeSpan _estimatedDuration;

    public string StartLocation
    {
        get => _startLocation;
        set => SetProperty(ref _startLocation, value);
    }

    public string EndLocation
    {
        get => _endLocation;
        set => SetProperty(ref _endLocation, value);
    }

    public double Distance
    {
        get => _distance;
        set => SetProperty(ref _distance, value);
    }

    public TimeSpan EstimatedDuration
    {
        get => _estimatedDuration;
        set => SetProperty(ref _estimatedDuration, value);
    }

    public override void Update()
    {
        base.Update();
        // Adicione aqui qualquer lógica específica de atualização para rotas
    }
}