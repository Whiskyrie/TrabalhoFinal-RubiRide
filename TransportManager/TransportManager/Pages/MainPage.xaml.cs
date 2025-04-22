namespace TransportManager;

[Bindable]
public sealed partial class MainPage : Page
{
  public MainPageViewModel ViewModel { get; }

  public MainPage()
  {
    InitializeComponent();

    // Ensure that Services is initialized
    if (App.Services == null)
    {
      throw new InvalidOperationException("Service provider is not initialized.");
    }

    var dbContext = App.Services.GetRequiredService<TransportDbContext>();
    var vehicleRepository = new VehicleRepository(dbContext);
    var driverRepository = new DriverRepository(dbContext);
    var routeRepository = new RouteRepository(dbContext);

    ViewModel = new MainPageViewModel(vehicleRepository, driverRepository, routeRepository);
    DataContext = ViewModel;

    // Configure os delegados de eventos do ViewModel
    SetupViewModelDelegates();

    // Registre o manipulador de evento carregado para carregar dados iniciais
    Loaded += MainPage_Loaded;
  }

  private void SetupViewModelDelegates()
  {
    // Configurar delegados para diálogos de veículos
    ViewModel.AddVehicleRequested = ShowAddVehicleDialog;
    ViewModel.EditVehicleRequested = ShowEditVehicleDialog;
    ViewModel.RemoveVehicleRequested = ShowRemoveVehicleDialog;

    // Configurar delegados para diálogos de motoristas
    ViewModel.AddDriverRequested = ShowAddDriverDialog;
    ViewModel.EditDriverRequested = ShowEditDriverDialog;
    ViewModel.RemoveDriverRequested = ShowRemoveDriverDialog;

    // Configurar delegados para diálogos de rotas
    ViewModel.AddRouteRequested = ShowAddRouteDialog;
    ViewModel.EditRouteRequested = ShowEditRouteDialog;
    ViewModel.RemoveRouteRequested = ShowRemoveRouteDialog;

    // Configurar delegados para feedback de carregamento e erros
    ViewModel.LoadingStarted = ShowLoadingIndicator;
    ViewModel.LoadingFinished = HideLoadingIndicator;
    ViewModel.ShowErrorMessage = ShowErrorMessage;
  }

  private void MainPage_Loaded(object sender, RoutedEventArgs e)
  {
    // Carregar dados iniciais
    ViewModel.LoadVehiclesCommand.Execute(null);
    ViewModel.LoadDriversCommand.Execute(null);
    ViewModel.LoadRoutesCommand.Execute(null);
  }

  private void TabButton_Checked(object sender, RoutedEventArgs e)
  {
    if (sender is RadioButton radioButton && radioButton.Tag is string sectionName)
    {
      // Ocultar todas as seções
      VehiclesSection.Visibility = Visibility.Collapsed;
      DriversSection.Visibility = Visibility.Collapsed;
      RoutesSection.Visibility = Visibility.Collapsed;

      // Mostrar apenas a seção selecionada
      if (FindName(sectionName) is UIElement section)
      {
        section.Visibility = Visibility.Visible;
      }
    }
  }

  private void RefreshButton_Click(object sender, RoutedEventArgs e)
  {
    // Atualiza os dados com base na aba atual
    if (VehiclesSection.Visibility == Visibility.Visible)
    {
      ViewModel.LoadVehiclesCommand.Execute(null);
    }
    else if (DriversSection.Visibility == Visibility.Visible)
    {
      ViewModel.LoadDriversCommand.Execute(null);
    }
    else if (RoutesSection.Visibility == Visibility.Visible)
    {
      ViewModel.LoadRoutesCommand.Execute(null);
    }
  }

  #region Navegação responsiva

  private void VehiclesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
  {
    // Em telas menores, quando um item é selecionado, mostra os detalhes e oculta a lista
    if (Window.Current.Bounds.Width < 800 && ViewModel.SelectedVehicle != null)
    {
      VehiclesList.Visibility = Visibility.Collapsed;
      VehicleDetails.Visibility = Visibility.Visible;
    }
  }

  private void DriversList_SelectionChanged(object sender, SelectionChangedEventArgs e)
  {
    // Em telas menores, quando um item é selecionado, mostra os detalhes e oculta a lista
    if (Window.Current.Bounds.Width < 800 && ViewModel.SelectedDriver != null)
    {
      DriversList.Visibility = Visibility.Collapsed;
      DriverDetails.Visibility = Visibility.Visible;
    }
  }

  private void RoutesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
  {
    // Em telas menores, quando um item é selecionado, mostra os detalhes e oculta a lista
    if (Window.Current.Bounds.Width < 800 && ViewModel.SelectedRoute != null)
    {
      RoutesList.Visibility = Visibility.Collapsed;
      RouteDetails.Visibility = Visibility.Visible;
    }
  }

  private void BackToListButton_Click(object sender, RoutedEventArgs e)
  {
    // Voltar para a lista de veículos em telas menores
    VehicleDetails.Visibility = Visibility.Collapsed;
    VehiclesList.Visibility = Visibility.Visible;
  }

  private void BackToDriversListButton_Click(object sender, RoutedEventArgs e)
  {
    // Voltar para a lista de motoristas em telas menores
    DriverDetails.Visibility = Visibility.Collapsed;
    DriversList.Visibility = Visibility.Visible;
  }

  private void BackToRoutesListButton_Click(object sender, RoutedEventArgs e)
  {
    // Voltar para a lista de rotas em telas menores
    RouteDetails.Visibility = Visibility.Collapsed;
    RoutesList.Visibility = Visibility.Visible;
  }

  #endregion

  #region Métodos para Veículos

  private async Task<Vehicle?> ShowAddVehicleDialog()
  {
    var dialog = new ContentDialog
    {
      Title = "Adicionar Veículo",
      PrimaryButtonText = "Adicionar",
      CloseButtonText = "Cancelar",
      DefaultButton = ContentDialogButton.Primary,
      Content = CreateVehicleForm(),
      XamlRoot = XamlRoot
    };

    while (true)
    {
      var result = await dialog.ShowAsync();

      if (result == ContentDialogResult.Primary)
      {
        if (dialog.Content is not StackPanel form)
        {
          ShowErrorMessage("Erro ao criar veículo: formulário não encontrado");
          return null;
        }

        var vehicle = CreateVehicleFromForm(form);
        if (vehicle.IsValid(out var validationResults))
        {
          return vehicle;
        }
        else
        {
          await ShowValidationErrorsDialog(validationResults);
        }
      }
      else
      {
        return null;
      }
    }
  }

  private async Task<Vehicle?> ShowEditVehicleDialog(Vehicle vehicle)
  {
    var form = CreateVehicleForm(vehicle);
    var dialog = new ContentDialog
    {
      Title = "Editar Veículo",
      PrimaryButtonText = "Salvar",
      CloseButtonText = "Cancelar",
      DefaultButton = ContentDialogButton.Primary,
      Content = form,
      XamlRoot = XamlRoot
    };

    while (true)
    {
      var result = await dialog.ShowAsync();

      if (result == ContentDialogResult.Primary)
      {
        var updatedVehicle = CreateVehicleFromForm(form);
        updatedVehicle.Id = vehicle.Id; // Manter o ID original
        if (updatedVehicle.IsValid(out var validationResults))
        {
          return updatedVehicle;
        }
        else
        {
          await ShowValidationErrorsDialog(validationResults);
        }
      }
      else
      {
        return null;
      }
    }
  }

  private async Task<bool> ShowRemoveVehicleDialog(Vehicle vehicle)
  {
    var dialog = new ContentDialog
    {
      Title = "Remover Veículo",
      Content = $"Tem certeza que deseja remover o veículo {vehicle.Model} ({vehicle.LicensePlate})?",
      PrimaryButtonText = "Remover",
      CloseButtonText = "Cancelar",
      DefaultButton = ContentDialogButton.Close,
      XamlRoot = XamlRoot
    };

    var result = await dialog.ShowAsync();
    return result == ContentDialogResult.Primary;
  }

  private StackPanel CreateVehicleForm(Vehicle? vehicle = null)
  {
    var stackPanel = new StackPanel { Spacing = 16 };

    // Modelo
    var modelTextBox = new TextBox
    {
      Header = "Modelo",
      PlaceholderText = "Ex: Volkswagen Constellation",
      Name = "ModelTextBox",
      Text = vehicle?.Model ?? "",
      Style = (Style)Resources["FormTextBoxStyle"]
    };
    stackPanel.Children.Add(modelTextBox);

    // Ano
    var yearNumberBox = new NumberBox
    {
      Header = "Ano",
      Minimum = 1900,
      Maximum = DateTime.Now.Year + 1,
      Value = vehicle?.Year ?? DateTime.Now.Year,
      Name = "YearNumberBox",
      Margin = new Thickness(0, 0, 0, 8)
    };
    stackPanel.Children.Add(yearNumberBox);

    // Placa
    var licensePlateTextBox = new TextBox
    {
      Header = "Placa",
      PlaceholderText = "Ex: ABC1234",
      Name = "LicensePlateTextBox",
      Text = vehicle?.LicensePlate ?? "",
      Style = (Style)Resources["FormTextBoxStyle"]
    };
    stackPanel.Children.Add(licensePlateTextBox);

    // Capacidade
    var capacityNumberBox = new NumberBox
    {
      Header = "Capacidade (toneladas)",
      Minimum = 0,
      Value = vehicle?.Capacity ?? 0,
      Name = "CapacityNumberBox",
      Margin = new Thickness(0, 0, 0, 8)
    };
    stackPanel.Children.Add(capacityNumberBox);

    // Tipo
    var typeComboBox = new ComboBox
    {
      Header = "Tipo",
      ItemsSource = Enum.GetValues(typeof(VehicleType)),
      SelectedItem = vehicle?.Type ?? VehicleType.Car,
      Name = "TypeComboBox",
      Style = (Style)Resources["FormComboBoxStyle"]
    };
    stackPanel.Children.Add(typeComboBox);

    // Status
    var statusComboBox = new ComboBox
    {
      Header = "Status",
      ItemsSource = Enum.GetValues(typeof(VehicleStatus)),
      SelectedItem = vehicle?.Status ?? VehicleStatus.Available,
      Name = "StatusComboBox",
      Style = (Style)Resources["FormComboBoxStyle"]
    };
    stackPanel.Children.Add(statusComboBox);

    return stackPanel;
  }

  private Vehicle CreateVehicleFromForm(StackPanel form)
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

  #region Métodos para Motoristas

  private async Task<Driver?> ShowAddDriverDialog()
  {
    var dialog = new ContentDialog
    {
      Title = "Adicionar Motorista",
      PrimaryButtonText = "Adicionar",
      CloseButtonText = "Cancelar",
      DefaultButton = ContentDialogButton.Primary,
      Content = CreateDriverForm(),
      XamlRoot = XamlRoot
    };

    while (true)
    {
      var result = await dialog.ShowAsync();

      if (result == ContentDialogResult.Primary)
      {
        if (dialog.Content is not StackPanel form)
        {
          ShowErrorMessage("Erro ao criar motorista: formulário não encontrado");
          return null;
        }

        var driver = CreateDriverFromForm(form);
        if (driver.IsValid(out var validationResults))
        {
          return driver;
        }
        else
        {
          await ShowValidationErrorsDialog(validationResults);
        }
      }
      else
      {
        return null;
      }
    }
  }

  private async Task<Driver?> ShowEditDriverDialog(Driver driver)
  {
    var form = CreateDriverForm(driver);
    var dialog = new ContentDialog
    {
      Title = "Editar Motorista",
      PrimaryButtonText = "Salvar",
      CloseButtonText = "Cancelar",
      DefaultButton = ContentDialogButton.Primary,
      Content = form,
      XamlRoot = XamlRoot
    };

    while (true)
    {
      var result = await dialog.ShowAsync();

      if (result == ContentDialogResult.Primary)
      {
        var updatedDriver = CreateDriverFromForm(form);
        updatedDriver.Id = driver.Id; // Manter o ID original
        if (updatedDriver.IsValid(out var validationResults))
        {
          return updatedDriver;
        }
        else
        {
          await ShowValidationErrorsDialog(validationResults);
        }
      }
      else
      {
        return null;
      }
    }
  }

  private async Task<bool> ShowRemoveDriverDialog(Driver driver)
  {
    var dialog = new ContentDialog
    {
      Title = "Remover Motorista",
      Content = $"Tem certeza que deseja remover o motorista {driver.Name}?",
      PrimaryButtonText = "Remover",
      CloseButtonText = "Cancelar",
      DefaultButton = ContentDialogButton.Close,
      XamlRoot = XamlRoot
    };

    var result = await dialog.ShowAsync();
    return result == ContentDialogResult.Primary;
  }

  private StackPanel CreateDriverForm(Driver? driver = null)
  {
    var stackPanel = new StackPanel { Spacing = 16 };

    // Nome
    var nameTextBox = new TextBox
    {
      Header = "Nome",
      PlaceholderText = "Ex: João Silva",
      Name = "NameTextBox",
      Text = driver?.Name ?? "",
      Style = (Style)Resources["FormTextBoxStyle"]
    };
    stackPanel.Children.Add(nameTextBox);

    // Número da Licença
    var licenseNumberTextBox = new TextBox
    {
      Header = "Número da Licença",
      PlaceholderText = "Exatos 20 caracteres",
      Name = "LicenseNumberTextBox",
      Text = driver?.LicenseNumber ?? "",
      MaxLength = 20,
      Style = (Style)Resources["FormTextBoxStyle"]
    };
    stackPanel.Children.Add(licenseNumberTextBox);

    // Expiração da Licença
    var licenseExpirationDatePicker = new DatePicker
    {
      Header = "Expiração da Licença",
      Name = "LicenseExpirationDatePicker",
      Date = driver?.LicenseExpirationDate ?? DateTime.Now.AddYears(1),
      Margin = new Thickness(0, 0, 0, 8),
      HorizontalAlignment = HorizontalAlignment.Stretch
    };
    stackPanel.Children.Add(licenseExpirationDatePicker);

    // Status
    var statusComboBox = new ComboBox
    {
      Header = "Status",
      ItemsSource = Enum.GetValues(typeof(DriverStatus)),
      SelectedItem = driver?.Status ?? DriverStatus.Available,
      Name = "StatusComboBox",
      Style = (Style)Resources["FormComboBoxStyle"]
    };
    stackPanel.Children.Add(statusComboBox);

    return stackPanel;
  }

  private Driver CreateDriverFromForm(StackPanel form)
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

  #region Métodos para Rotas

  private async Task<Route?> ShowAddRouteDialog()
  {
    var dialog = new ContentDialog
    {
      Title = "Adicionar Rota",
      PrimaryButtonText = "Adicionar",
      CloseButtonText = "Cancelar",
      DefaultButton = ContentDialogButton.Primary,
      Content = CreateRouteForm(),
      XamlRoot = XamlRoot
    };

    while (true)
    {
      var result = await dialog.ShowAsync();

      if (result == ContentDialogResult.Primary)
      {
        if (dialog.Content is not StackPanel form)
        {
          ShowErrorMessage("Erro ao criar rota: formulário não encontrado");
          return null;
        }

        var route = CreateRouteFromForm(form);
        var validationResults = route.Validate();
        if (validationResults.Count == 0)
        {
          return route;
        }
        else
        {
          await ShowValidationErrorsDialog(validationResults);
        }
      }
      else
      {
        return null;
      }
    }
  }

  private async Task<Route?> ShowEditRouteDialog(Route route)
  {
    var form = CreateRouteForm(route);
    var dialog = new ContentDialog
    {
      Title = "Editar Rota",
      PrimaryButtonText = "Salvar",
      CloseButtonText = "Cancelar",
      DefaultButton = ContentDialogButton.Primary,
      Content = form,
      XamlRoot = XamlRoot
    };

    while (true)
    {
      var result = await dialog.ShowAsync();

      if (result == ContentDialogResult.Primary)
      {
        var updatedRoute = CreateRouteFromForm(form);
        updatedRoute.Id = route.Id; // Manter o ID original
        var validationResults = updatedRoute.Validate();
        if (validationResults.Count == 0)
        {
          return updatedRoute;
        }
        else
        {
          await ShowValidationErrorsDialog(validationResults);
        }
      }
      else
      {
        return null;
      }
    }
  }

  private async Task<bool> ShowRemoveRouteDialog(Route route)
  {
    var dialog = new ContentDialog
    {
      Title = "Remover Rota",
      Content = $"Tem certeza que deseja remover a rota de {route.StartLocation} para {route.EndLocation}?",
      PrimaryButtonText = "Remover",
      CloseButtonText = "Cancelar",
      DefaultButton = ContentDialogButton.Close,
      XamlRoot = XamlRoot
    };

    var result = await dialog.ShowAsync();
    return result == ContentDialogResult.Primary;
  }

  private StackPanel CreateRouteForm(Route? route = null)
  {
    var cityDistances = new CityDistances();
    var stackPanel = new StackPanel { Spacing = 16 };

    // Origem
    var startLocationComboBox = new ComboBox
    {
      Header = "Origem",
      ItemsSource = ViewModel.Cities,
      SelectedItem = route?.StartLocation ?? ViewModel.Cities.FirstOrDefault() ?? "",
      Name = "StartLocationComboBox",
      Style = (Style)Resources["FormComboBoxStyle"]
    };
    stackPanel.Children.Add(startLocationComboBox);

    // Destino
    var endLocationComboBox = new ComboBox
    {
      Header = "Destino",
      ItemsSource = ViewModel.Cities,
      SelectedItem = route?.EndLocation ?? ViewModel.Cities.LastOrDefault() ?? "",
      Name = "EndLocationComboBox",
      Style = (Style)Resources["FormComboBoxStyle"]
    };
    stackPanel.Children.Add(endLocationComboBox);

    // Distância (calculada automaticamente)
    var distanceNumberBox = new NumberBox
    {
      Header = "Distância (km)",
      Value = route?.Distance ?? 0,
      IsEnabled = false,
      Name = "DistanceNumberBox",
      Margin = new Thickness(0, 0, 0, 8)
    };
    stackPanel.Children.Add(distanceNumberBox);

    // Duração Estimada - Grid para organizar os componentes
    var durationPanel = new Grid
    {
      Margin = new Thickness(0, 0, 0, 8)
    };

    durationPanel.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
    durationPanel.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

    var durationHeader = new TextBlock
    {
      Text = "Duração Estimada",
      Margin = new Thickness(0, 0, 0, 8)
    };
    Grid.SetRow(durationHeader, 0);
    durationPanel.Children.Add(durationHeader);

    var durationControlsGrid = new Grid();
    durationControlsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
    durationControlsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(8) }); // Espaçador
    durationControlsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
    durationControlsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(8) }); // Espaçador
    durationControlsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

    // Duração - Dias
    var daysNumberBox = new NumberBox
    {
      Header = "Dias",
      Minimum = 0,
      Value = route?.EstimatedDuration.Days ?? 0,
      Name = "DaysNumberBox"
    };
    Grid.SetColumn(daysNumberBox, 0);
    durationControlsGrid.Children.Add(daysNumberBox);

    // Duração - Horas
    var hoursNumberBox = new NumberBox
    {
      Header = "Horas",
      Minimum = 0,
      Maximum = 23,
      Value = route?.EstimatedDuration.Hours ?? 0,
      Name = "HoursNumberBox"
    };
    Grid.SetColumn(hoursNumberBox, 2);
    durationControlsGrid.Children.Add(hoursNumberBox);

    // Duração - Minutos
    var minutesNumberBox = new NumberBox
    {
      Header = "Minutos",
      Minimum = 0,
      Maximum = 59,
      Value = route?.EstimatedDuration.Minutes ?? 0,
      Name = "MinutesNumberBox"
    };
    Grid.SetColumn(minutesNumberBox, 4);
    durationControlsGrid.Children.Add(minutesNumberBox);

    Grid.SetRow(durationControlsGrid, 1);
    durationPanel.Children.Add(durationControlsGrid);

    stackPanel.Children.Add(durationPanel);

    // Motorista
    var driverComboBox = new ComboBox
    {
      Header = "Motorista",
      ItemsSource = ViewModel.Drivers,
      DisplayMemberPath = "Name",
      SelectedItem = route?.Driver ?? ViewModel.Drivers.FirstOrDefault(),
      Name = "DriverComboBox",
      Style = (Style)Resources["FormComboBoxStyle"]
    };
    stackPanel.Children.Add(driverComboBox);

    // Veículo
    var vehicleComboBox = new ComboBox
    {
      Header = "Veículo",
      ItemsSource = ViewModel.Vehicles,
      DisplayMemberPath = "Model",
      SelectedItem = route?.Vehicle ?? ViewModel.Vehicles.FirstOrDefault(),
      Name = "VehicleComboBox",
      Style = (Style)Resources["FormComboBoxStyle"]
    };
    stackPanel.Children.Add(vehicleComboBox);

    // Configurar eventos para atualizar distância automaticamente
    void UpdateDistanceAndDuration()
    {
      if (startLocationComboBox.SelectedItem is string startLocation &&
          endLocationComboBox.SelectedItem is string endLocation)
      {
        var distance = cityDistances.GetDistance(startLocation, endLocation);
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

    // Chamar o método uma vez para inicializar os valores
    if (startLocationComboBox.SelectedItem != null && endLocationComboBox.SelectedItem != null)
    {
      UpdateDistanceAndDuration();
    }

    return stackPanel;
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
      distance = new CityDistances().GetDistance(startLocation, endLocation);
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

  #region Métodos Utilitários

  private async Task ShowValidationErrorsDialog(ICollection<ValidationResult> validationResults)
  {
    var errorMessages = string.Join("\n• ", validationResults.Select(vr => vr.ErrorMessage));

    var dialog = new ContentDialog
    {
      Title = "Erro de Validação",
      Content = $"Por favor, corrija os seguintes erros:\n\n• {errorMessages}",
      CloseButtonText = "OK",
      XamlRoot = XamlRoot
    };

    await dialog.ShowAsync();
  }

  private void ShowErrorMessage(string message)
  {
    // Configurar a mensagem de erro
    ErrorMessageText.Text = message;
    ErrorMessageBorder.Visibility = Visibility.Visible;

    // Configurar o timer para ocultar a mensagem após alguns segundos
    DispatcherQueue.TryEnqueue(async () =>
    {
      await Task.Delay(5000);
      ErrorMessageBorder.Visibility = Visibility.Collapsed;
    });
  }

  private void ShowLoadingIndicator()
  {
    LoadingIndicator.Visibility = Visibility.Visible;
  }

  private void HideLoadingIndicator()
  {
    LoadingIndicator.Visibility = Visibility.Collapsed;
  }

  #endregion
}