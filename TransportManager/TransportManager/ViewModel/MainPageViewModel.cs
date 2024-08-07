namespace TransportManager.ViewModel;
[Bindable]
public class MainPageViewModel : INotifyPropertyChanged {
  private readonly VehicleRepository _vehicleRepository;
  private ObservableCollection<Vehicle> _vehicles;
  private Vehicle? _selectedVehicle;

  public ObservableCollection<Vehicle> Vehicles {
    get => _vehicles;
    set {
      _vehicles = value;
      OnPropertyChanged(nameof(Vehicles));
    }
  }

  public Vehicle? SelectedVehicle {
    get => _selectedVehicle;
    set {
      if (_selectedVehicle != value) {
        _selectedVehicle = value;
        OnPropertyChanged(nameof(SelectedVehicle));
        OnPropertyChanged(nameof(IsVehicleSelected));
        (EditVehicleCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
        (RemoveVehicleCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
      }
    }
  }

  public bool IsVehicleSelected => SelectedVehicle != null;

  public ICommand AddVehicleCommand { get; }
  public ICommand EditVehicleCommand { get; }
  public ICommand RemoveVehicleCommand { get; }
  public ICommand LoadVehiclesCommand { get; }

  public Func<Task<Vehicle?>> AddVehicleRequested { get; set; }
  public Func<Vehicle, Task<Vehicle?>> EditVehicleRequested { get; set; }
  public Func<Vehicle, Task<bool>> RemoveVehicleRequested { get; set; }
  public Action LoadingStarted { get; set; }
  public Action LoadingFinished { get; set; }
  public Action<string> ShowErrorMessage { get; set; }

  public MainPageViewModel(VehicleRepository vehicleRepository) {
    _vehicleRepository = vehicleRepository
        ?? throw new ArgumentNullException(nameof(vehicleRepository));
    _vehicles = [];

    AddVehicleCommand = new AsyncRelayCommand(AddVehicle);
    EditVehicleCommand =
        new AsyncRelayCommand(EditVehicle, () => IsVehicleSelected);
    RemoveVehicleCommand =
        new AsyncRelayCommand(RemoveVehicle, () => IsVehicleSelected);
    LoadVehiclesCommand = new AsyncRelayCommand(LoadVehicles);

    // Initialize with empty delegates to avoid null reference exceptions
    AddVehicleRequested = () => Task.FromResult<Vehicle?>(null);
    EditVehicleRequested = _ => Task.FromResult<Vehicle?>(null);
    RemoveVehicleRequested = _ => Task.FromResult(false);
    LoadingStarted = () => {};
    LoadingFinished = () => {};
    ShowErrorMessage = _ => {};
  }

  private async Task LoadVehicles() {
    try {
      LoadingStarted?.Invoke();
      var vehicles = await _vehicleRepository.GetAllVehiclesAsync();
      Vehicles = new ObservableCollection<Vehicle>(vehicles);
    } catch (Exception ex) {
      // Handle or log the exception
      System.Diagnostics.Debug.WriteLine(
          $"Error loading vehicles: {ex.Message}");
    } finally {
      LoadingFinished?.Invoke();
    }
  }

  private async Task AddVehicle() {
    if (AddVehicleRequested == null)
      return;

    var newVehicle = await AddVehicleRequested();
    if (newVehicle != null) {
      await _vehicleRepository.AddVehicleAsync(newVehicle);
      Vehicles.Add(newVehicle);
    }
  }

  private async Task EditVehicle() {
    if (SelectedVehicle == null || EditVehicleRequested == null)
      return;

    try {
      var updatedVehicle = await EditVehicleRequested(SelectedVehicle);
      if (updatedVehicle != null) {
        System.Diagnostics.Debug.WriteLine(
            $"Editando veículo com ID: {updatedVehicle.Id}");
        await _vehicleRepository.UpdateVehicleAsync(updatedVehicle);

        var index = Vehicles.IndexOf(SelectedVehicle);
        if (index != -1) {
          Vehicles[index] = updatedVehicle;
        }

        SelectedVehicle = updatedVehicle;
        OnPropertyChanged(nameof(Vehicles));
      }
    } catch (Exception ex) {
      System.Diagnostics.Debug.WriteLine(
          $"Erro ao editar veículo: {ex.Message}");
      ShowErrorMessage?.Invoke($"Erro ao editar veículo: {ex.Message}");
    }
  }

  private async Task RemoveVehicle() {
    if (SelectedVehicle == null || RemoveVehicleRequested == null)
      return;

    try {
      bool confirmRemove = await RemoveVehicleRequested(SelectedVehicle);
      if (confirmRemove) {
        System.Diagnostics.Debug.WriteLine(
            $"Removendo veículo com ID: {SelectedVehicle.Id}");
        await _vehicleRepository.RemoveVehicleAsync(SelectedVehicle);
        Vehicles.Remove(SelectedVehicle);
        SelectedVehicle = null;
      }
    } catch (Exception ex) {
      System.Diagnostics.Debug.WriteLine(
          $"Erro ao remover veículo: {ex.Message}");
      ShowErrorMessage?.Invoke($"Erro ao remover veículo: {ex.Message}");
    }
  }

  public event PropertyChangedEventHandler? PropertyChanged;

  protected virtual void OnPropertyChanged([CallerMemberName] string
                                           ? propertyName = null) {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    System.Diagnostics.Debug.WriteLine($"Propriedade alterada: {propertyName}");
  }
}