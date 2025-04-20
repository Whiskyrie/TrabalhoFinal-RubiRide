public class ValidationError(string errorMessage, string? propertyName = null)
{

    public string ErrorMessage { get; } = errorMessage;

    public string? PropertyName { get; } = propertyName;
}
