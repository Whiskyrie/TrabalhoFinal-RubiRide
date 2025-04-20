namespace TransportManager.Services;

public class DialogService : IDialogService
{
    private readonly XamlRoot _xamlRoot;
    private readonly CityDistances _cityDistances;

    public DialogService(XamlRoot xamlRoot)
    {
        _xamlRoot = xamlRoot ?? throw new ArgumentNullException(nameof(xamlRoot));
        _cityDistances = new CityDistances();
    }

    #region Métodos para Vehicle

    public async Task<Vehicle?> ShowAddVehicleDialogAsync()
    {
        var form = CreateVehicleForm();
        var dialog = new ContentDialog()
        {
            Title = "Adicionar Veículo",
            PrimaryButtonText = "Adicionar",
            CloseButtonText = "Cancelar",
            DefaultButton = ContentDialogButton.Primary,
            Content = form,
            XamlRoot = _xamlRoot
        };

        while (true)
        {
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var vehicle = CreateVehicleFromForm(form);
                if (vehicle.IsValid(out var validationResults))
                {
                    return vehicle;
                }
                else
                {
                    await ShowValidationErrorsDialogAsync(validationResults);
                }
            }
            else
            {
                return null;
            }
        }
    }

    public async Task<Vehicle?> ShowEditVehicleDialogAsync(Vehicle vehicle)
    {
        var form = CreateVehicleForm(vehicle);
        var dialog = new ContentDialog()
        {
            Title = "Editar Veículo",
            PrimaryButtonText = "Salvar",
            CloseButtonText = "Cancelar",
            DefaultButton = ContentDialogButton.Primary,
            Content = form,
            XamlRoot = _xamlRoot
        };

        while (true)
        {
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var updatedVehicle = CreateVehicleFromForm(form);
                updatedVehicle.Id = vehicle.Id; // Mantém o ID original
                if (updatedVehicle.IsValid(out var validationResults))
                {
                    return updatedVehicle;
                }
                else
                {
                    await ShowValidationErrorsDialogAsync(validationResults);
                }
            }
            else
            {
                return null;
            }
        }
    }

    public async Task<bool> ShowRemoveVehicleDialogAsync(Vehicle vehicle)
    {
        var dialog = new ContentDialog()
        {
            Title = "Remover Veículo",
            Content = $"Tem certeza que deseja remover o veículo {vehicle.Model} ({vehicle.LicensePlate})?",
            PrimaryButtonText = "Remover",
            CloseButtonText = "Cancelar",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = _xamlRoot
        };

        var result = await dialog.ShowAsync();
        return result == ContentDialogResult.Primary;
    }

    private static StackPanel CreateVehicleForm(Vehicle? vehicle = null)
    {
        return new StackPanel 
        { 
            Children = 
            {
                new TextBox { Header = "Modelo", Name = "ModelTextBox", Text = vehicle?.Model ?? "" },
                new NumberBox { Header = "Ano", Name = "YearNumberBox", Value = vehicle?.Year ?? DateTime.Now.Year },
                new TextBox { Header = "Placa", Name = "LicensePlateTextBox", Text = vehicle?.LicensePlate ?? "" },
                new NumberBox { Header = "Capacidade", Name = "CapacityNumberBox", Value = vehicle?.Capacity ?? 0 },
                new ComboBox { Header = "Tipo", Name = "TypeComboBox", ItemsSource = Enum.GetValues(typeof(VehicleType)), SelectedItem = vehicle?.Type ?? VehicleType.Car },
                new ComboBox { Header = "Status", Name = "StatusComboBox", ItemsSource = Enum.GetValues(typeof(VehicleStatus)), SelectedItem = vehicle?.Status ?? VehicleStatus.Available }
            }
        };
    }

    private static Vehicle CreateVehicleFromForm(StackPanel form)
    {
        return new Vehicle 
        {
            Model = ((TextBox)form.FindName("ModelTextBox")).Text,
            Year = (int)((NumberBox)form.FindName("YearNumberBox")).Value,
            LicensePlate = ((TextBox)form.FindName("LicensePlateTextBox")).Text,
            Capacity = (double)((NumberBox)form.FindName("CapacityNumberBox")).Value,
            Type = (VehicleType)((ComboBox)form.FindName("TypeComboBox")).SelectedItem,
            Status = (VehicleStatus)((ComboBox)form.FindName("StatusComboBox")).SelectedItem
        };
    }

    #endregion

    #region Métodos para Driver

    public async Task<Driver?> ShowAddDriverDialogAsync()
    {
        var form = CreateDriverForm();
        var dialog = new ContentDialog()
        {
            Title = "Adicionar Motorista",
            PrimaryButtonText = "Adicionar",
            CloseButtonText = "Cancelar",
            DefaultButton = ContentDialogButton.Primary,
            Content = form,
            XamlRoot = _xamlRoot
        };

        while (true)
        {
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var driver = CreateDriverFromForm(form);
                if (driver.IsValid(out var validationResults))
                {
                    return driver;
                }
                else
                {
                    await ShowValidationErrorsDialogAsync(validationResults);
                }
            }
            else
            {
                return null;
            }
        }
    }

    public async Task<Driver?> ShowEditDriverDialogAsync(Driver driver)
    {
        var form = CreateDriverForm(driver);
        var dialog = new ContentDialog()
        {
            Title = "Editar Motorista",
            PrimaryButtonText = "Salvar",
            CloseButtonText = "Cancelar",
            DefaultButton = ContentDialogButton.Primary,
            Content = form,
            XamlRoot = _xamlRoot
        };

        while (true)
        {
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var updatedDriver = CreateDriverFromForm(form);
                updatedDriver.Id = driver.Id; // Mantém o ID original
                if (updatedDriver.IsValid(out var validationResults))
                {
                    return updatedDriver;
                }
                else
                {
                    await ShowValidationErrorsDialogAsync(validationResults);
                }
            }
            else
            {
                return null;
            }
        }
    }

    public async Task<bool> ShowRemoveDriverDialogAsync(Driver driver)
    {
        var dialog = new ContentDialog()
        {
            Title = "Remover Motorista",
            Content = $"Tem certeza que deseja remover o motorista {driver.Name}?",
            PrimaryButtonText = "Remover",
            CloseButtonText = "Cancelar",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = _xamlRoot
        };

        var result = await dialog.ShowAsync();
        return result == ContentDialogResult.Primary;
    }

    private static StackPanel CreateDriverForm(Driver? driver = null)
    {
        return new StackPanel 
        { 
            Children = 
            {
                new TextBox { Header = "Nome", Name = "NameTextBox", Text = driver?.Name ?? "" },
                new TextBox { Header = "Número da Licença", Name = "LicenseNumberTextBox", Text = driver?.LicenseNumber ?? "" },
                new DatePicker { Header = "Expiração da Licença", Name = "LicenseExpirationDatePicker", Date = driver?.LicenseExpirationDate ?? DateTime.Now.AddYears(1) },
                new ComboBox { Header = "Status", Name = "StatusComboBox", ItemsSource = Enum.GetValues(typeof(DriverStatus)), SelectedItem = driver?.Status ?? DriverStatus.Available }
            }
        };
    }

    private static Driver CreateDriverFromForm(StackPanel form)
    {
        DatePicker licenseExpirationDatePicker = (DatePicker)form.FindName("LicenseExpirationDatePicker");
        DateTimeOffset? selectedDate = licenseExpirationDatePicker.Date;

        return new Driver 
        {
            Name = ((TextBox)form.FindName("NameTextBox")).Text,
            LicenseNumber = ((TextBox)form.FindName("LicenseNumberTextBox")).Text,
            LicenseExpirationDate = selectedDate.HasValue ? selectedDate.Value.DateTime : DateTime.Now.AddYears(1),
            Status = (DriverStatus)((ComboBox)form.FindName("StatusComboBox")).SelectedItem
        };
    }

    #endregion

    #region Métodos para Route

    public async Task<Route?> ShowAddRouteDialogAsync(
        ObservableCollection<Driver> drivers, 
        ObservableCollection<Vehicle> vehicles, 
        ObservableCollection<string> cities)
    {
        var form = CreateRouteForm(null, drivers, vehicles, cities);
        var dialog = new ContentDialog()
        {
            Title = "Adicionar Rota",
            PrimaryButtonText = "Adicionar",
            CloseButtonText = "Cancelar",
            DefaultButton = ContentDialogButton.Primary,
            Content = form,
            XamlRoot = _xamlRoot
        };

        while (true)
        {
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var route = CreateRouteFromForm(form);
                var validationResults = route.Validate();
                if (validationResults.Count == 0)
                {
                    return route;
                }
                else
                {
                    await ShowValidationErrorsDialogAsync(validationResults);
                }
            }
            else
            {
                return null;
            }
        }
    }

    public async Task<Route?> ShowEditRouteDialogAsync(
        Route route, 
        ObservableCollection<Driver> drivers, 
        ObservableCollection<Vehicle> vehicles, 
        ObservableCollection<string> cities)
    {
        var form = CreateRouteForm(route, drivers, vehicles, cities);
        var dialog = new ContentDialog()
        {
            Title = "Editar Rota",
            PrimaryButtonText = "Salvar",
            CloseButtonText = "Cancelar",
            DefaultButton = ContentDialogButton.Primary,
            Content = form,
            XamlRoot = _xamlRoot
        };

        while (true)
        {
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var updatedRoute = CreateRouteFromForm(form);
                updatedRoute.Id = route.Id; // Mantém o ID original
                var validationResults = updatedRoute.Validate();
                if (validationResults.Count == 0)
                {
                    return updatedRoute;
                }
                else
                {
                    await ShowValidationErrorsDialogAsync(validationResults);
                }
            }
            else
            {
                return null;
            }
        }
    }

    public async Task<bool> ShowRemoveRouteDialogAsync(Route route)
    {
        var dialog = new ContentDialog()
        {
            Title = "Remover Rota",
            Content = $"Tem certeza que deseja remover a rota de {route.StartLocation} para {route.EndLocation}?",
            PrimaryButtonText = "Remover",
            CloseButtonText = "Cancelar",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = _xamlRoot
        };

        var result = await dialog.ShowAsync();
        return result == ContentDialogResult.Primary;
    }

    private StackPanel CreateRouteForm(
        Route? route, 
        ObservableCollection<Driver> drivers, 
        ObservableCollection<Vehicle> vehicles, 
        ObservableCollection<string> cities)
    {
        var startLocationComboBox = new ComboBox 
        { 
            Header = "Origem", 
            Name = "StartLocationComboBox",
            ItemsSource = cities,
            SelectedItem = route?.StartLocation ?? cities.FirstOrDefault() ?? ""
        };

        var endLocationComboBox = new ComboBox 
        { 
            Header = "Destino", 
            Name = "EndLocationComboBox",
            ItemsSource = cities,
            SelectedItem = route?.EndLocation ?? cities.FirstOrDefault() ?? ""
        };

        var distanceNumberBox = new NumberBox 
        { 
            Header = "Distância (km)", 
            Name = "DistanceNumberBox",
            Value = route?.Distance ?? 0, 
            IsEnabled = false 
        };

        var daysNumberBox = new NumberBox 
        { 
            Header = "Dias", 
            Name = "DaysNumberBox",
            Value = route?.EstimatedDuration.Days ?? 0,
            Minimum = 0 
        };
        
        var hoursNumberBox = new NumberBox 
        { 
            Header = "Horas", 
            Name = "HoursNumberBox",
            Value = route?.EstimatedDuration.Hours ?? 0,
            Minimum = 0, 
            Maximum = 23 
        };
        
        var minutesNumberBox = new NumberBox 
        { 
            Header = "Minutos", 
            Name = "MinutesNumberBox",
            Value = route?.EstimatedDuration.Minutes ?? 0,
            Minimum = 0, 
            Maximum = 59 
        };

        void UpdateDistanceAndDuration()
        {
            if (startLocationComboBox.SelectedItem is string startLocation &&
                endLocationComboBox.SelectedItem is string endLocation)
            {
                var distance = _cityDistances.GetDistance(startLocation, endLocation);
                distanceNumberBox.Value = distance >= 0 ? distance : 0;

                // Calcular duração estimada
                if (distance > 0)
                {
                    double averageSpeed = 80; // km/h
                    double hours = distance / averageSpeed;
                    var duration = TimeSpan.FromHours(hours);
                    daysNumberBox.Value = duration.Days;
                    hoursNumberBox.Value = duration.Hours;
                    minutesNumberBox.Value = duration.Minutes;
                }
            }
        }

        startLocationComboBox.SelectionChanged += (s, e) => UpdateDistanceAndDuration();
        endLocationComboBox.SelectionChanged += (s, e) => UpdateDistanceAndDuration();

        return new StackPanel 
        { 
            Children = 
            {
                startLocationComboBox, 
                endLocationComboBox, 
                distanceNumberBox,
                new StackPanel 
                { 
                    Orientation = Orientation.Horizontal,
                    Children = 
                    { 
                        daysNumberBox, 
                        hoursNumberBox,
                        minutesNumberBox 
                    } 
                },
                new ComboBox 
                { 
                    Header = "Motorista", 
                    Name = "DriverComboBox",
                    ItemsSource = drivers, 
                    DisplayMemberPath = "Name",
                    SelectedItem = route?.Driver ?? drivers.FirstOrDefault() 
                },
                new ComboBox 
                { 
                    Header = "Veículo", 
                    Name = "VehicleComboBox",
                    ItemsSource = vehicles, 
                    DisplayMemberPath = "Model",
                    SelectedItem = route?.Vehicle ?? vehicles.FirstOrDefault() 
                }
            }
        };
    }

    private Route CreateRouteFromForm(StackPanel form)
    {
        var startLocationComboBox = (ComboBox)form.FindName("StartLocationComboBox");
        var endLocationComboBox = (ComboBox)form.FindName("EndLocationComboBox");
        var distanceNumberBox = (NumberBox)form.FindName("DistanceNumberBox");
        var daysNumberBox = (NumberBox)form.FindName("DaysNumberBox");
        var hoursNumberBox = (NumberBox)form.FindName("HoursNumberBox");
        var minutesNumberBox = (NumberBox)form.FindName("MinutesNumberBox");
        var driverComboBox = (ComboBox)form.FindName("DriverComboBox");
        var vehicleComboBox = (ComboBox)form.FindName("VehicleComboBox");

        // Usar o valor do campo de distância ou calcular se necessário
        var distance = distanceNumberBox.Value;
        if (distance <= 0 && startLocationComboBox.SelectedItem is string startLocation && 
            endLocationComboBox.SelectedItem is string endLocation)
        {
            distance = _cityDistances.GetDistance(startLocation, endLocation);
            if (distance < 0) distance = 0;
        }

        var estimatedDuration = new TimeSpan(
            (int)daysNumberBox.Value, 
            (int)hoursNumberBox.Value,
            (int)minutesNumberBox.Value, 
            0);

        return new Route 
        { 
            StartLocation = startLocationComboBox.SelectedItem as string ?? "",
            EndLocation = endLocationComboBox.SelectedItem as string ?? "",
            Distance = distance,
            EstimatedDuration = estimatedDuration,
            Driver = driverComboBox.SelectedItem as Driver,
            Vehicle = vehicleComboBox.SelectedItem as Vehicle 
        };
    }

    #endregion

    #region Métodos utilitários

    public async Task ShowValidationErrorsDialogAsync(ICollection<ValidationResult> validationResults)
    {
        var errorMessages = string.Join("\n", validationResults.Select(vr => vr.ErrorMessage));
        var errorDialog = new ContentDialog()
        {
            Title = "Erro de Validação",
            Content = $"Por favor, corrija os seguintes erros:\n\n{errorMessages}",
            CloseButtonText = "OK",
            XamlRoot = _xamlRoot
        };

        await errorDialog.ShowAsync();
    }

    public async Task ShowErrorMessageAsync(string message)
    {
        var dialog = new ContentDialog()
        {
            Title = "Erro",
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = _xamlRoot
        };

        await dialog.ShowAsync();
    }

    #endregion
}