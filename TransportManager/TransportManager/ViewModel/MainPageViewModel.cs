namespace TransportManager.ViewModel;

[Bindable]
public class MainPageViewModel : INotifyPropertyChanged {
  private readonly VehicleRepository _vehicleRepository;
  private readonly DriverRepository _driverRepository;
  private readonly RouteRepository _routeRepository;
  private ObservableCollection<Vehicle> _vehicles;
  private ObservableCollection<Driver> _drivers;
  private ObservableCollection<Route> _routes;
  private Vehicle? _selectedVehicle;
  private Driver? _selectedDriver;
  private Route? _selectedRoute;

  public ObservableCollection<Vehicle> Vehicles {
    get => _vehicles;
    set {
      _vehicles = value;
      OnPropertyChanged(nameof(Vehicles));
    }
  }

  public ObservableCollection<Driver> Drivers {
    get => _drivers;
    set {
      _drivers = value;
      OnPropertyChanged(nameof(Drivers));
    }
  }

  public ObservableCollection<Route> Routes {
    get => _routes;
    set {
      _routes = value;
      OnPropertyChanged(nameof(Routes));
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

  public Driver? SelectedDriver {
    get => _selectedDriver;
    set {
      if (_selectedDriver != value) {
        _selectedDriver = value;
        OnPropertyChanged(nameof(SelectedDriver));
        OnPropertyChanged(nameof(IsDriverSelected));
        (EditDriverCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
        (RemoveDriverCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
      }
    }
  }

  public Route? SelectedRoute {
    get => _selectedRoute;
    set {
      if (_selectedRoute != value) {
        _selectedRoute = value;
        OnPropertyChanged(nameof(SelectedRoute));
        OnPropertyChanged(nameof(IsRouteSelected));
        (EditRouteCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
        (RemoveRouteCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
      }
    }
  }

  public bool IsVehicleSelected => SelectedVehicle != null;
  public bool IsDriverSelected => SelectedDriver != null;
  public bool IsRouteSelected => SelectedRoute != null;

  public ICommand AddVehicleCommand { get; }
  public ICommand EditVehicleCommand { get; }
  public ICommand RemoveVehicleCommand { get; }
  public ICommand LoadVehiclesCommand { get; }

  public ICommand AddDriverCommand { get; }
  public ICommand EditDriverCommand { get; }
  public ICommand RemoveDriverCommand { get; }
  public ICommand LoadDriversCommand { get; }

  public ICommand AddRouteCommand { get; }
  public ICommand EditRouteCommand { get; }
  public ICommand RemoveRouteCommand { get; }
  public ICommand LoadRoutesCommand { get; }

  public Func<Task<Vehicle?>> AddVehicleRequested { get; set; }
  public Func<Vehicle, Task<Vehicle?>> EditVehicleRequested { get; set; }
  public Func<Vehicle, Task<bool>> RemoveVehicleRequested { get; set; }

  public Func<Task<Driver?>> AddDriverRequested { get; set; }
  public Func<Driver, Task<Driver?>> EditDriverRequested { get; set; }
  public Func<Driver, Task<bool>> RemoveDriverRequested { get; set; }

  public Func<Task<Route?>> AddRouteRequested { get; set; }
  public Func<Route, Task<Route?>> EditRouteRequested { get; set; }
  public Func<Route, Task<bool>> RemoveRouteRequested { get; set; }

  public Action LoadingStarted { get; set; }
  public Action LoadingFinished { get; set; }
  public Action<string> ShowErrorMessage { get; set; }

  public MainPageViewModel(VehicleRepository vehicleRepository,
                           DriverRepository driverRepository,
                           RouteRepository routeRepository) {
    _vehicleRepository = vehicleRepository
        ?? throw new ArgumentNullException(nameof(vehicleRepository));
    _driverRepository = driverRepository
        ?? throw new ArgumentNullException(nameof(driverRepository));
    _routeRepository = routeRepository
        ?? throw new ArgumentNullException(nameof(routeRepository));
    _vehicles = [];
    _drivers = [];
    _routes = [];

    AddVehicleCommand = new AsyncRelayCommand(AddVehicle);
    EditVehicleCommand =
        new AsyncRelayCommand(EditVehicle, () => IsVehicleSelected);
    RemoveVehicleCommand =
        new AsyncRelayCommand(RemoveVehicle, () => IsVehicleSelected);
    LoadVehiclesCommand = new AsyncRelayCommand(LoadVehicles);

    AddDriverCommand = new AsyncRelayCommand(AddDriver);
    EditDriverCommand =
        new AsyncRelayCommand(EditDriver, () => IsDriverSelected);
    RemoveDriverCommand =
        new AsyncRelayCommand(RemoveDriver, () => IsDriverSelected);
    LoadDriversCommand = new AsyncRelayCommand(LoadDrivers);

    AddRouteCommand = new AsyncRelayCommand(AddRoute);
    EditRouteCommand = new AsyncRelayCommand(EditRoute, () => IsRouteSelected);
    RemoveRouteCommand =
        new AsyncRelayCommand(RemoveRoute, () => IsRouteSelected);
    LoadRoutesCommand = new AsyncRelayCommand(LoadRoutes);

    // Initialize with empty delegates to avoid null reference exceptions
    AddVehicleRequested = () => Task.FromResult<Vehicle?>(null);
    EditVehicleRequested = _ => Task.FromResult<Vehicle?>(null);
    RemoveVehicleRequested = _ => Task.FromResult(false);
    AddDriverRequested = () => Task.FromResult<Driver?>(null);
    EditDriverRequested = _ => Task.FromResult<Driver?>(null);
    RemoveDriverRequested = _ => Task.FromResult(false);
    LoadingStarted = () => {};
    LoadingFinished = () => {};
    ShowErrorMessage = _ => {};
    AddRouteRequested = () => Task.FromResult<Route?>(null);
    EditRouteRequested = _ => Task.FromResult<Route?>(null);
    RemoveRouteRequested = _ => Task.FromResult(false);
  }

  private async Task LoadVehicles() {
    try {
      LoadingStarted?.Invoke();
      var vehicles = await _vehicleRepository.GetAllVehiclesAsync();
      Vehicles = new ObservableCollection<Vehicle>(vehicles);
    } catch (Exception ex) {
      // Handle or log the exception
      System.Diagnostics.Debug.WriteLine(
          $"Erro ao carregar veículos: {ex.Message}");
      ShowErrorMessage?.Invoke($"Erro ao carregar veículos: {ex.Message}");
    } finally {
      LoadingFinished?.Invoke();
    }
  }

  private async Task LoadDrivers() {
    try {
      LoadingStarted?.Invoke();
      var drivers = await _driverRepository.GetAllDriversAsync();
      Drivers = new ObservableCollection<Driver>(drivers);
    } catch (Exception ex) {
      System.Diagnostics.Debug.WriteLine(
          $"Erro ao carregar motoristas: {ex.Message}");
      ShowErrorMessage?.Invoke($"Erro ao carregar motoristas: {ex.Message}");
    } finally {
      LoadingFinished?.Invoke();
    }
  }
  private async Task LoadRoutes() {
    try {
      LoadingStarted?.Invoke();
      var routes = await _routeRepository.GetAllRoutesAsync();
      Routes = new ObservableCollection<Route>(routes);
    } catch (Exception ex) {
      System.Diagnostics.Debug.WriteLine(
          $"Erro ao carregar rotas: {ex.Message}");
      ShowErrorMessage?.Invoke($"Erro ao carregar rotas: {ex.Message}");
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

  private async Task AddDriver() {
    if (AddDriverRequested == null)
      return;

    var newDriver = await AddDriverRequested();
    if (newDriver != null) {
      await _driverRepository.AddDriverAsync(newDriver);
      Drivers.Add(newDriver);
    }
  }
  private async Task AddRoute() {
    if (AddRouteRequested == null)
      return;

    var newRoute = await AddRouteRequested();
    if (newRoute != null) {
      await _routeRepository.AddRouteAsync(newRoute);
      Routes.Add(newRoute);
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

  private async Task EditDriver() {
    if (SelectedDriver == null || EditDriverRequested == null)
      return;

    try {
      var updatedDriver = await EditDriverRequested(SelectedDriver);
      if (updatedDriver != null) {
        System.Diagnostics.Debug.WriteLine(
            $"Editando motorista com ID: {updatedDriver.Id}");
        await _driverRepository.UpdateDriverAsync(updatedDriver);

        var index = Drivers.IndexOf(SelectedDriver);
        if (index != -1) {
          Drivers[index] = updatedDriver;
        }

        SelectedDriver = updatedDriver;
        OnPropertyChanged(nameof(Drivers));
      }
    } catch (Exception ex) {
      System.Diagnostics.Debug.WriteLine(
          $"Erro ao editar motorista: {ex.Message}");
      ShowErrorMessage?.Invoke($"Erro ao editar motorista: {ex.Message}");
    }
  }
  private async Task EditRoute() {
    if (SelectedRoute == null || EditRouteRequested == null)
      return;

    try {
      var updatedRoute = await EditRouteRequested(SelectedRoute);
      if (updatedRoute != null) {
        System.Diagnostics.Debug.WriteLine(
            $"Editando rota com ID: {updatedRoute.Id}");
        await _routeRepository.UpdateRouteAsync(updatedRoute);

        var index = Routes.IndexOf(SelectedRoute);
        if (index != -1) {
          Routes[index] = updatedRoute;
        }

        SelectedRoute = updatedRoute;
        OnPropertyChanged(nameof(Routes));
      }
    } catch (Exception ex) {
      System.Diagnostics.Debug.WriteLine($"Erro ao editar rota: {ex.Message}");
      ShowErrorMessage?.Invoke($"Erro ao editar rota: {ex.Message}");
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

  private async Task RemoveDriver() {
    if (SelectedDriver == null || RemoveDriverRequested == null)
      return;

    try {
      bool confirmRemove = await RemoveDriverRequested(SelectedDriver);
      if (confirmRemove) {
        System.Diagnostics.Debug.WriteLine(
            $"Removendo motorista com ID: {SelectedDriver.Id}");
        await _driverRepository.RemoveDriverAsync(SelectedDriver);
        Drivers.Remove(SelectedDriver);
        SelectedDriver = null;
      }
    } catch (Exception ex) {
      System.Diagnostics.Debug.WriteLine(
          $"Erro ao remover motorista: {ex.Message}");
      ShowErrorMessage?.Invoke($"Erro ao remover motorista: {ex.Message}");
    }
  }

  private async Task RemoveRoute() {
    if (SelectedRoute == null || RemoveRouteRequested == null)
      return;

    try {
      bool confirmRemove = await RemoveRouteRequested(SelectedRoute);
      if (confirmRemove) {
        System.Diagnostics.Debug.WriteLine(
            $"Removendo rota com ID: {SelectedRoute.Id}");
        await _routeRepository.RemoveRouteAsync(SelectedRoute);
        Routes.Remove(SelectedRoute);
        SelectedRoute = null;
      }
    } catch (Exception ex) {
      System.Diagnostics.Debug.WriteLine($"Erro ao remover rota: {ex.Message}");
      ShowErrorMessage?.Invoke($"Erro ao remover rota: {ex.Message}");
    }
  }

  public event PropertyChangedEventHandler? PropertyChanged;

  protected virtual void OnPropertyChanged([CallerMemberName] string
                                           ? propertyName = null) {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    System.Diagnostics.Debug.WriteLine($"Propriedade alterada: {propertyName}");
  }
}