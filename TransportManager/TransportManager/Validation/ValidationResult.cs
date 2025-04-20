public class ValidationResult
{
    private readonly List<ValidationError> _errors = new();

    public bool IsValid => !_errors.Any();


    public IReadOnlyList<ValidationError> Errors => _errors.AsReadOnly();

    public void AddError(string errorMessage, string? propertyName = null)
    {
        _errors.Add(new ValidationError(errorMessage, propertyName));
    }
    public void AddErrors(IEnumerable<ValidationError> errors)
    {
        _errors.AddRange(errors);
    }

    public ValidationResult Combine(ValidationResult other)
    {
        AddErrors(other.Errors);
        return this;
    }

    public string GetErrorMessages()
    {
        return string.Join(Environment.NewLine, _errors.Select(e => e.ErrorMessage));
    }
}
