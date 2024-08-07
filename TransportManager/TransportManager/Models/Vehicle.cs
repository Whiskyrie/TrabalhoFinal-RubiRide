namespace TransportManager.Models;

public class Vehicle : BaseEntity {
  private string _model = string.Empty;
  private int _year;
  private string _licensePlate = string.Empty;
  private double _capacity;
  private VehicleType _type;
  private VehicleStatus _status;

  public string Model {
    get => _model;
    set => SetProperty(ref _model, value);
  }

  public int Year {
    get => _year;
    set => SetProperty(ref _year, value);
  }

  public string LicensePlate {
    get => _licensePlate;
    set => SetProperty(ref _licensePlate, value);
  }

  public double Capacity {
    get => _capacity;
    set => SetProperty(ref _capacity, value);
  }

  public VehicleType Type {
    get => _type;
    set => SetProperty(ref _type, value);
  }

  public VehicleStatus Status {
    get => _status;
    set => SetProperty(ref _status, value);
  }

  public string Details => $"{Year} {Model} - {LicensePlate}";

  public override void Update() {
    base.Update();
    // Adicione aqui qualquer lógica específica de atualização para veículos
  }
}

public enum VehicleType { Car, Truck, Van, Bus, Motorcycle }

public enum VehicleStatus { Available, InUse, UnderMaintenance, OutOfService }
