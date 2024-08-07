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
    ViewModel = new MainPageViewModel(vehicleRepository);
    DataContext = ViewModel;
    ViewModel.AddVehicleRequested += ShowAddVehicleDialog;
    ViewModel.EditVehicleRequested += ShowEditVehicleDialog;
    ViewModel.RemoveVehicleRequested += ShowRemoveVehicleDialog;
    ViewModel.LoadingStarted += ShowLoadingIndicator;
    ViewModel.LoadingFinished += HideLoadingIndicator;

    Loaded += MainPage_Loaded;
  }

  private void MainPage_Loaded(object sender, RoutedEventArgs e) {
    ViewModel.LoadVehiclesCommand.Execute(null);
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