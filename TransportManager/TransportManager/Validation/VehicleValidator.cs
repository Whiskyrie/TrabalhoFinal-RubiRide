using TransportManager.Validation;

public class VehicleValidator : IValidator<Vehicle>
{
    public ValidationResult Validate(Vehicle entity)
    {
        var result = new ValidationResult();

        // Validação de Model
        if (string.IsNullOrWhiteSpace(entity.Model))
        {
            result.AddError("O modelo é obrigatório", nameof(entity.Model));
        }
        else if (entity.Model.Length > 100)
        {
            result.AddError("O modelo não pode ter mais de 100 caracteres", nameof(entity.Model));
        }

        // Validação de Year
        if (entity.Year < 1900 || entity.Year > 2100)
        {
            result.AddError("O ano deve estar entre 1900 e 2100", nameof(entity.Year));
        }

        // Validação de LicensePlate
        if (string.IsNullOrWhiteSpace(entity.LicensePlate))
        {
            result.AddError("A placa é obrigatória", nameof(entity.LicensePlate));
        }
        else if (entity.LicensePlate.Length > 20)
        {
            result.AddError("A placa não pode ter mais de 20 caracteres", nameof(entity.LicensePlate));
        }

        // Validação de Capacity
        if (entity.Capacity < 0)
        {
            result.AddError("A capacidade deve ser um número positivo", nameof(entity.Capacity));
        }

        return result;
    }
}