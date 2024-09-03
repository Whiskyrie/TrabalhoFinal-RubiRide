namespace TransportManager.Models;

public abstract class BaseEntity : INotifyPropertyChanged {
  private string? _id;
  private string? _name;
  private DateTime _createdAt;
  private DateTime _updatedAt;

  public string? Id {
    get => _id;
    set => SetProperty(ref _id, value);
  }

  public string? Name {
    get => _name;
    set => SetProperty(ref _name, value);
  }

  public DateTime CreatedAt {
    get => _createdAt;
    set => SetProperty(ref _createdAt, value);
  }

  public DateTime UpdatedAt {
    get => _updatedAt;
    set => SetProperty(ref _updatedAt, value);
  }

  protected BaseEntity() {
    Id = Guid.NewGuid().ToString();
    CreatedAt = DateTime.UtcNow;
    UpdatedAt = DateTime.UtcNow;
  }

  public event PropertyChangedEventHandler? PropertyChanged;

  protected virtual void OnPropertyChanged([CallerMemberName] string
                                           ? propertyName = null) {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }

  protected bool SetProperty<T>(ref T storage, T value,
                                [CallerMemberName] string
                                ? propertyName = null) {
    if (Equals(storage, value)) {
      return false;
    }

    storage = value;
    OnPropertyChanged(propertyName);
    return true;
  }

  public virtual void Update() { UpdatedAt = DateTime.UtcNow; }
}
