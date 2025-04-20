using TransportManager.Validation;

public class DriverValidator : IValidator<Driver>
{
    public ValidationResult Validate(Driver entity)
    {
        var result = new ValidationResult();

        // Validação de Name
        if (string.IsNullOrWhiteSpace(entity.Name))
        {
            result.AddError("O nome é obrigatório", nameof(entity.Name));
        }
        else if (entity.Name.Length > 100)
        {
            result.AddError("O nome não pode ter mais de 100 caracteres", nameof(entity.Name));
        }

        // Validação de LicenseNumber
        if (string.IsNullOrWhiteSpace(entity.LicenseNumber))
        {
            result.AddError("O número da licença é obrigatório", nameof(entity.LicenseNumber));
        }
        else if (entity.LicenseNumber.Length != 20)
        {
            result.AddError("O número da licença deve ter exatamente 20 caracteres", nameof(entity.LicenseNumber));
        }

        // Validação de LicenseExpirationDate
        if (entity.LicenseExpirationDate < DateTime.Today)
        {
            result.AddError("A data de expiração da licença não pode ser no passado", nameof(entity.LicenseExpirationDate));
        }

        return result;
    }
}