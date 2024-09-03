using System;

namespace TransportManager.Models;

public class Route : BaseEntity {
  private string _startLocation = string.Empty;
  private string _endLocation = string.Empty;
  private double _distance;
  private TimeSpan _estimatedDuration;
  private Driver? _driver;
  private Vehicle? _vehicle;

  public string StartLocation {
    get => _startLocation;
    set => SetProperty(ref _startLocation, value);
  }

  public string EndLocation {
    get => _endLocation;
    set => SetProperty(ref _endLocation, value);
  }

  public double Distance {
    get => _distance;
    set => SetProperty(ref _distance, value);
  }

  public TimeSpan EstimatedDuration {
    get => _estimatedDuration;
    set => SetProperty(ref _estimatedDuration, value);
  }

  public Driver? Driver {
    get => _driver;
    set => SetProperty(ref _driver, value);
  }

  public Vehicle? Vehicle {
    get => _vehicle;
    set => SetProperty(ref _vehicle, value);
  }

  public bool IsValid() { return Driver != null && Vehicle != null; }

  public override void Update() {
    base.Update();
    // Adicione aqui qualquer lógica específica de atualização para rotas
  }

  public ICollection<ValidationResult> Validate() {
    var results = new List<ValidationResult>();

    if (string.IsNullOrWhiteSpace(StartLocation))
      results.Add(
          new ValidationResult("A localização de início é obrigatória."));

    if (string.IsNullOrWhiteSpace(EndLocation))
      results.Add(
          new ValidationResult("A localização de destino é obrigatória."));

    if (Distance <= 0)
      results.Add(new ValidationResult("A distância deve ser maior que zero."));

    if (EstimatedDuration <= TimeSpan.Zero)
      results.Add(
          new ValidationResult("A duração estimada deve ser maior que zero."));

    if (Driver == null)
      results.Add(new ValidationResult("Um motorista deve ser selecionado."));

    if (Vehicle == null)
      results.Add(new ValidationResult("Um veículo deve ser selecionado."));

    return results;
  }

  public static Route CreateRoute(Driver driver, Vehicle vehicle,
                                  string startLocation, string endLocation,
                                  double distance, TimeSpan estimatedDuration) {
    if (driver == null || vehicle == null) {
      throw new ArgumentException(
          "O motorista e o veículo devem ser selecionados antes de criar uma nova rota.");
    }

    return new Route { Driver = driver,
                       Vehicle = vehicle,
                       StartLocation = startLocation,
                       EndLocation = endLocation,
                       Distance = distance,
                       EstimatedDuration = estimatedDuration };
  }

  public void CalculateEstimatedDuration() {
    if (Vehicle != null && Distance > 0) {
      // Assumindo que a velocidade média é de 80 km/h
      double averageSpeed = 80;
      double hours = Distance / averageSpeed;
      EstimatedDuration = TimeSpan.FromHours(hours);
    }
  }
}