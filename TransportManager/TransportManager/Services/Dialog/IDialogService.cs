namespace TransportManager.Services;

public interface IDialogService
{
    Task<Vehicle?> ShowAddVehicleDialogAsync();
    Task<Vehicle?> ShowEditVehicleDialogAsync(Vehicle vehicle);
    Task<bool> ShowRemoveVehicleDialogAsync(Vehicle vehicle);
    
    Task<Driver?> ShowAddDriverDialogAsync();
    Task<Driver?> ShowEditDriverDialogAsync(Driver driver);
    Task<bool> ShowRemoveDriverDialogAsync(Driver driver);
    
    Task<Route?> ShowAddRouteDialogAsync(ObservableCollection<Driver> drivers, ObservableCollection<Vehicle> vehicles, ObservableCollection<string> cities);
    Task<Route?> ShowEditRouteDialogAsync(Route route, ObservableCollection<Driver> drivers, ObservableCollection<Vehicle> vehicles, ObservableCollection<string> cities);
    Task<bool> ShowRemoveRouteDialogAsync(Route route);
    
    Task ShowValidationErrorsDialogAsync(ICollection<ValidationResult> validationResults);
    Task ShowErrorMessageAsync(string message);
}