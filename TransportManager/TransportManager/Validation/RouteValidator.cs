using TransportManager.Validation;

public class RouteValidator : IValidator<Route>
{
    public ValidationResult Validate(Route entity)
    {
        var result = new ValidationResult();

        // Validação de StartLocation
        if (string.IsNullOrWhiteSpace(entity.StartLocation))
        {
            result.AddError("A localização de início é obrigatória", nameof(entity.StartLocation));
        }

        // Validação de EndLocation
        if (string.IsNullOrWhiteSpace(entity.EndLocation))
        {
            result.AddError("A localização de destino é obrigatória", nameof(entity.EndLocation));
        }

        // Validação de Distance
        if (entity.Distance <= 0)
        {
            result.AddError("A distância deve ser maior que zero", nameof(entity.Distance));
        }

        // Validação de EstimatedDuration
        if (entity.EstimatedDuration <= TimeSpan.Zero)
        {
            result.AddError("A duração estimada deve ser maior que zero", nameof(entity.EstimatedDuration));
        }

        // Validação de Driver
        if (entity.Driver == null)
        {
            result.AddError("Um motorista deve ser selecionado", nameof(entity.Driver));
        }

        // Validação de Vehicle
        if (entity.Vehicle == null)
        {
            result.AddError("Um veículo deve ser selecionado", nameof(entity.Vehicle));
        }

        return result;
    }
}