namespace TransportManager;

[Bindable]
public sealed partial class MainPage : Page {
  public MainPageViewModel ViewModel { get; }

  public MainPage() {
    this.InitializeComponent();

    // Ensure that Services is initialized
    if (App.Services == null) {
      throw new InvalidOperationException(
          "Service provider is not initialized.");
    }

    var dbContext = App.Services.GetRequiredService<TransportDbContext>();
    var vehicleRepository = new VehicleRepository(dbContext);
    var driverRepository = new DriverRepository(dbContext);
    var routeRepository = new RouteRepository(dbContext);
    ViewModel = new MainPageViewModel(vehicleRepository, driverRepository,
                                      routeRepository);
    DataContext = ViewModel;
    ViewModel.AddVehicleRequested += ShowAddVehicleDialog;
    ViewModel.EditVehicleRequested += ShowEditVehicleDialog;
    ViewModel.RemoveVehicleRequested += ShowRemoveVehicleDialog;
    ViewModel.AddDriverRequested += ShowAddDriverDialog;
    ViewModel.EditDriverRequested += ShowEditDriverDialog;
    ViewModel.RemoveDriverRequested += ShowRemoveDriverDialog;
    ViewModel.LoadingStarted += ShowLoadingIndicator;
    ViewModel.LoadingFinished += HideLoadingIndicator;
    ViewModel.AddRouteRequested += ShowAddRouteDialog;
    ViewModel.EditRouteRequested += ShowEditRouteDialog;
    ViewModel.RemoveRouteRequested += ShowRemoveRouteDialog;

    Loaded += MainPage_Loaded;
  }

  private void MainPage_Loaded(object sender, RoutedEventArgs e) {
    ViewModel.LoadVehiclesCommand.Execute(null);
    ViewModel.LoadDriversCommand.Execute(null);
    ViewModel.LoadRoutesCommand.Execute(null);
  }

  private async Task<Vehicle?> ShowAddVehicleDialog() {
    var form = CreateVehicleForm();
    var dialog =
        new ContentDialog() { Title = "Adicionar Veículo",
                              PrimaryButtonText = "Adicionar",
                              CloseButtonText = "Cancelar",
                              DefaultButton = ContentDialogButton.Primary,
                              Content = form,
                              XamlRoot = this.XamlRoot };

    while (true) {
      var result = await dialog.ShowAsync();

      if (result == ContentDialogResult.Primary) {
        var vehicle = CreateVehicleFromForm(form);
        if (vehicle.IsValid(out var validationResults)) {
          return vehicle;
        } else {
          await ShowValidationErrorsDialog(validationResults);
        }
      } else {
        return null;
      }
    }
  }

  private async Task<Vehicle?> ShowEditVehicleDialog(Vehicle vehicle) {
    var form = CreateVehicleForm(vehicle);
    var dialog =
        new ContentDialog() { Title = "Editar Veículo",
                              PrimaryButtonText = "Salvar",
                              CloseButtonText = "Cancelar",
                              DefaultButton = ContentDialogButton.Primary,
                              Content = form,
                              XamlRoot = this.XamlRoot };

    while (true) {
      var result = await dialog.ShowAsync();

      if (result == ContentDialogResult.Primary) {
        var updatedVehicle = CreateVehicleFromForm(form);
        updatedVehicle.Id = vehicle.Id; // Mantenha o ID original
        if (updatedVehicle.IsValid(out var validationResults)) {
          return updatedVehicle;
        } else {
          await ShowValidationErrorsDialog(validationResults);
        }
      } else {
        return null;
      }
    }
  }

  private async Task<bool> ShowRemoveVehicleDialog(Vehicle vehicle) {
    var dialog = new ContentDialog() {
      Title = "Remover Veículo",
      Content =
          $"Tem certeza que deseja remover o veículo {vehicle.Model} ({vehicle.LicensePlate})?",
      PrimaryButtonText = "Remover",
      CloseButtonText = "Cancelar",
      DefaultButton = ContentDialogButton.Close,
      XamlRoot = this.XamlRoot
    };

    var result = await dialog.ShowAsync();

    return result == ContentDialogResult.Primary;
  }

  private static StackPanel CreateVehicleForm(Vehicle? vehicle = null) {
    return new StackPanel { Children = {
      new TextBox { Header = "Modelo", Name = "ModelTextBox",
                    Text = vehicle?.Model ?? "" },
      new NumberBox { Header = "Ano", Name = "YearNumberBox",
                      Value = vehicle?.Year ?? DateTime.Now.Year },
      new TextBox { Header = "Placa", Name = "LicensePlateTextBox",
                    Text = vehicle?.LicensePlate ?? "" },
      new NumberBox { Header = "Capacidade", Name = "CapacityNumberBox",
                      Value = vehicle?.Capacity ?? 0 },
      new ComboBox { Header = "Tipo", Name = "TypeComboBox",
                     ItemsSource = Enum.GetValues(typeof(VehicleType)),
                     SelectedItem = vehicle?.Type ?? VehicleType.Car },
      new ComboBox { Header = "Status", Name = "StatusComboBox",
                     ItemsSource = Enum.GetValues(typeof(VehicleStatus)),
                     SelectedItem = vehicle?.Status ?? VehicleStatus.Available }
    } };
  }

  private static Vehicle CreateVehicleFromForm(StackPanel form) {
    return new Vehicle {
      Model = ((TextBox)form.FindName("ModelTextBox")).Text,
      Year = (int)((NumberBox)form.FindName("YearNumberBox")).Value,
      LicensePlate = ((TextBox)form.FindName("LicensePlateTextBox")).Text,
      Capacity = (double)((NumberBox)form.FindName("CapacityNumberBox")).Value,
      Type =
          (VehicleType)((ComboBox)form.FindName("TypeComboBox")).SelectedItem,
      Status = (VehicleStatus)((ComboBox)form.FindName("StatusComboBox"))
                   .SelectedItem
    };
  }

  private async Task
  ShowValidationErrorsDialog(ICollection<ValidationResult> validationResults) {
    var errorMessages =
        string.Join("\n", validationResults.Select(vr => vr.ErrorMessage));
    var errorDialog = new ContentDialog() {
      Title = "Erro de Validação",
      Content = $"Por favor, corrija os seguintes erros:\n\n{errorMessages}",
      CloseButtonText = "OK", XamlRoot = this.XamlRoot
    };

    await errorDialog.ShowAsync();
  }
  private async Task<Driver?> ShowAddDriverDialog() {
    var form = CreateDriverForm();
    var dialog =
        new ContentDialog() { Title = "Adicionar Motorista",
                              PrimaryButtonText = "Adicionar",
                              CloseButtonText = "Cancelar",
                              DefaultButton = ContentDialogButton.Primary,
                              Content = form,
                              XamlRoot = this.XamlRoot };

    while (true) {
      var result = await dialog.ShowAsync();

      if (result == ContentDialogResult.Primary) {
        var driver = CreateDriverFromForm(form);
        if (driver.IsValid(out var validationResults)) {
          return driver;
        } else {
          await ShowValidationErrorsDialog(validationResults);
        }
      } else {
        return null;
      }
    }
  }

  private async Task<Driver?> ShowEditDriverDialog(Driver driver) {
    var form = CreateDriverForm(driver);
    var dialog =
        new ContentDialog() { Title = "Editar Motorista",
                              PrimaryButtonText = "Salvar",
                              CloseButtonText = "Cancelar",
                              DefaultButton = ContentDialogButton.Primary,
                              Content = form,
                              XamlRoot = this.XamlRoot };

    while (true) {
      var result = await dialog.ShowAsync();

      if (result == ContentDialogResult.Primary) {
        var updatedDriver = CreateDriverFromForm(form);
        updatedDriver.Id = driver.Id; // Mantenha o ID original
        if (updatedDriver.IsValid(out var validationResults)) {
          return updatedDriver;
        } else {
          await ShowValidationErrorsDialog(validationResults);
        }
      } else {
        return null;
      }
    }
  }

  private async Task<bool> ShowRemoveDriverDialog(Driver driver) {
    var dialog = new ContentDialog() {
      Title = "Remover Motorista",
      Content = $"Tem certeza que deseja remover o motorista {driver.Name}?",
      PrimaryButtonText = "Remover",
      CloseButtonText = "Cancelar",
      DefaultButton = ContentDialogButton.Close,
      XamlRoot = this.XamlRoot
    };

    var result = await dialog.ShowAsync();

    return result == ContentDialogResult.Primary;
  }

  private static StackPanel CreateDriverForm(Driver? driver = null) {
    return new StackPanel { Children = {
      new TextBox { Header = "Nome", Name = "NameTextBox",
                    Text = driver?.Name ?? "" },
      new TextBox { Header = "Número da Licença", Name = "LicenseNumberTextBox",
                    Text = driver?.LicenseNumber ?? "" },
      new DatePicker { Header = "Expiração da Licença",
                       Name = "LicenseExpirationDatePicker",
                       Date = driver?.LicenseExpirationDate ?? DateTime.Now },
      new ComboBox { Header = "Status", Name = "StatusComboBox",
                     ItemsSource = Enum.GetValues(typeof(DriverStatus)),
                     SelectedItem = driver?.Status ?? DriverStatus.Available }
    } };
  }

  private static Driver CreateDriverFromForm(StackPanel form) {
    DatePicker licenseExpirationDatePicker =
        (DatePicker)form.FindName("LicenseExpirationDatePicker");
    DateTimeOffset? selectedDate = licenseExpirationDatePicker.Date;

    return new Driver {
      Name = ((TextBox)form.FindName("NameTextBox")).Text,
      LicenseNumber = ((TextBox)form.FindName("LicenseNumberTextBox")).Text,
      LicenseExpirationDate =
          selectedDate.HasValue ? selectedDate.Value.DateTime : default,
      Status =
          (DriverStatus)((ComboBox)form.FindName("StatusComboBox")).SelectedItem
    };
  }
  private async Task<Route?> ShowAddRouteDialog() {
    var form = CreateRouteForm();
    var dialog =
        new ContentDialog() { Title = "Adicionar Rota",
                              PrimaryButtonText = "Adicionar",
                              CloseButtonText = "Cancelar",
                              DefaultButton = ContentDialogButton.Primary,
                              Content = form,
                              XamlRoot = this.XamlRoot };

    while (true) {
      var result = await dialog.ShowAsync();

      if (result == ContentDialogResult.Primary) {
        var route = CreateRouteFromForm(form);
        var validationResults = route.Validate();
        if (validationResults.Count == 0) {
          return route;
        } else {
          await ShowValidationErrorsDialog(validationResults);
        }
      } else {
        return null;
      }
    }
  }
  private async Task<Route?> ShowEditRouteDialog(Route route) {
    var form = CreateRouteForm(route);
    var dialog =
        new ContentDialog() { Title = "Editar Rota",
                              PrimaryButtonText = "Salvar",
                              CloseButtonText = "Cancelar",
                              DefaultButton = ContentDialogButton.Primary,
                              Content = form,
                              XamlRoot = this.XamlRoot };

    while (true) {
      var result = await dialog.ShowAsync();

      if (result == ContentDialogResult.Primary) {
        var updatedRoute = CreateRouteFromForm(form);
        updatedRoute.Id = route.Id; // Mantém o ID original
        var validationResults = updatedRoute.Validate();
        if (validationResults.Count == 0) {
          return updatedRoute;
        } else {
          await ShowValidationErrorsDialog(validationResults);
        }
      } else {
        return null;
      }
    }
  }

  private async Task<bool> ShowRemoveRouteDialog(Route route) {
    var dialog = new ContentDialog() {
      Title = "Remover Rota",
      Content =
          $"Tem certeza que deseja remover a rota de {route.StartLocation} para {route.EndLocation}?",
      PrimaryButtonText = "Remover",
      CloseButtonText = "Cancelar",
      DefaultButton = ContentDialogButton.Close,
      XamlRoot = this.XamlRoot
    };

    var result = await dialog.ShowAsync();

    return result == ContentDialogResult.Primary;
  }

  private StackPanel CreateRouteForm(Route? route = null) {
    var drivers = new ObservableCollection<Driver>(ViewModel.Drivers);
    var vehicles = new ObservableCollection<Vehicle>(ViewModel.Vehicles);

    var startLocationComboBox =
        new ComboBox { Header = "Origem", Name = "StartLocationComboBox",
                       ItemsSource = ViewModel.Cities,
                       SelectedItem = route?.StartLocation
                                      ?? ViewModel.Cities.FirstOrDefault()
                                      ?? "" };

    var endLocationComboBox =
        new ComboBox { Header = "Destino", Name = "EndLocationComboBox",
                       ItemsSource = ViewModel.Cities,
                       SelectedItem = route?.EndLocation
                                      ?? ViewModel.Cities.FirstOrDefault()
                                      ?? "" };

    var distanceNumberBox =
        new NumberBox { Header = "Distância (km)", Name = "DistanceNumberBox",
                        Value = route?.Distance ?? 0, IsEnabled = false };

    var daysNumberBox =
        new NumberBox { Header = "Dias", Name = "DaysNumberBox",
                        Value = route?.EstimatedDuration.Days ?? 0,
                        Minimum = 0 };
    var hoursNumberBox =
        new NumberBox { Header = "Horas", Name = "HoursNumberBox",
                        Value = route?.EstimatedDuration.Hours ?? 0,
                        Minimum = 0, Maximum = 23 };
    var minutesNumberBox =
        new NumberBox { Header = "Minutos", Name = "MinutesNumberBox",
                        Value = route?.EstimatedDuration.Minutes ?? 0,
                        Minimum = 0, Maximum = 59 };

    void UpdateDistanceAndDuration() {
      if (startLocationComboBox.SelectedItem is string startLocation &&
          endLocationComboBox.SelectedItem is string endLocation) {
        var distance =
            new CityDistances().GetDistance(startLocation, endLocation);
        distanceNumberBox.Value = distance >= 0 ? distance : 0;

        // Calcular duração estimada
        if (distance > 0) {
          double averageSpeed = 80; // km/h
          double hours = distance / averageSpeed;
          var duration = TimeSpan.FromHours(hours);
          daysNumberBox.Value = duration.Days;
          hoursNumberBox.Value = duration.Hours;
          minutesNumberBox.Value = duration.Minutes;
        }
      }
    }

    startLocationComboBox.SelectionChanged += (s, e) =>
        UpdateDistanceAndDuration();
    endLocationComboBox.SelectionChanged += (s, e) =>
        UpdateDistanceAndDuration();

    return new StackPanel { Children = {
      startLocationComboBox, endLocationComboBox, distanceNumberBox,
      new StackPanel { Orientation = Orientation.Horizontal,
                       Children = { daysNumberBox, hoursNumberBox,
                                    minutesNumberBox } },
      new ComboBox { Header = "Motorista", Name = "DriverComboBox",
                     ItemsSource = drivers, DisplayMemberPath = "Name",
                     SelectedItem = route?.Driver ?? drivers.FirstOrDefault() },
      new ComboBox { Header = "Veículo", Name = "VehicleComboBox",
                     ItemsSource = vehicles, DisplayMemberPath = "Model",
                     SelectedItem =
                         route?.Vehicle ?? vehicles.FirstOrDefault() }
    } };
  }
  private Route CreateRouteFromForm(StackPanel form) {
    var startLocationComboBox =
        (ComboBox)form.FindName("StartLocationComboBox");
    var endLocationComboBox = (ComboBox)form.FindName("EndLocationComboBox");
    var daysNumberBox = (NumberBox)form.FindName("DaysNumberBox");
    var hoursNumberBox = (NumberBox)form.FindName("HoursNumberBox");
    var minutesNumberBox = (NumberBox)form.FindName("MinutesNumberBox");
    var driverComboBox = (ComboBox)form.FindName("DriverComboBox");
    var vehicleComboBox = (ComboBox)form.FindName("VehicleComboBox");

    var distance = new CityDistances().GetDistance(
        startLocationComboBox.SelectedItem as string ?? "",
        endLocationComboBox.SelectedItem as string ?? "");

    var estimatedDuration =
        new TimeSpan((int)daysNumberBox.Value, (int)hoursNumberBox.Value,
                     (int)minutesNumberBox.Value, 0);

    return new Route { StartLocation =
                           startLocationComboBox.SelectedItem as string ?? "",
                       EndLocation =
                           endLocationComboBox.SelectedItem as string ?? "",
                       Distance = distance,
                       EstimatedDuration = estimatedDuration,
                       Driver = driverComboBox.SelectedItem as Driver,
                       Vehicle = vehicleComboBox.SelectedItem as Vehicle };
  }
  private void ShowLoadingIndicator() {
    // Implemente a lógica para mostrar um indicador de carregamento
    // Por exemplo, você pode ter um ProgressRing na sua UI:
    // LoadingProgressRing.IsActive = true;
    // LoadingProgressRing.Visibility = Visibility.Visible;
  }

  private void HideLoadingIndicator() {
    // Implemente a lógica para esconder o indicador de carregamento
    // LoadingProgressRing.IsActive = false;
    // LoadingProgressRing.Visibility = Visibility.Collapsed;
  }
}