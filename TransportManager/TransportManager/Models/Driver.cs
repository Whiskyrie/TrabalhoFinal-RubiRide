namespace TransportManager.Models;

public class Driver : BaseEntity {
  private string _licenseNumber = string.Empty;
  private DateTime _licenseExpirationDate;
  private DriverStatus _status;

  [Required(ErrorMessage = "O número da licença é obrigatório")]
  [StringLength(20, MinimumLength = 20,
                ErrorMessage =
                    "O número da licença deve ter exatamente 20 caracteres")]
  public string LicenseNumber {
    get => _licenseNumber;
    set => SetProperty(ref _licenseNumber, value);
  }

  [Required(ErrorMessage = "A data de expiração da licença é obrigatória")] [DataType(
      DataType.Date)] [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
                                     ApplyFormatInEditMode =
                                         true)] public DateTime
      LicenseExpirationDate {
    get => _licenseExpirationDate.Date;
    set => SetProperty(ref _licenseExpirationDate, value.Date);
  }

  public DriverStatus Status {
    get => _status;
    set => SetProperty(ref _status, value);
  }

  public bool IsLicenseValid => LicenseExpirationDate > DateTime.Now;

  public string Details =>
      $"{Name} - {LicenseNumber} (Expira em: {LicenseExpirationDate:yyyy-MM-dd})";

  public override void Update() {
    base.Update();
    // Adicione aqui qualquer lógica específica de atualização para motoristas
  }

  public bool IsValid(out ICollection<ValidationResult> validationResults) {
    var context =
        new ValidationContext(this, serviceProvider: null, items: null);
    validationResults = new List<ValidationResult>();
    return Validator.TryValidateObject(this, context, validationResults, true);
  }
}

public enum DriverStatus { Available, OnDuty, OnLeave, Inactive }