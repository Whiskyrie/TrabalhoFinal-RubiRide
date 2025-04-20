public class PaginatedViewModel<T> : INotifyPropertyChanged
{
    private readonly Func<int, int, string?, Task<PaginatedResult<T>>> _loadPageFunc;
    private PaginatedResult<T>? _currentPage;
    private int _pageIndex = 0;
    private int _pageSize = 20;
    private bool _isLoading;
    private string? _searchTerm;

    public ObservableCollection<T> Items { get; } = new ObservableCollection<T>();

    public bool IsLoading
    {
        get => _isLoading;
        private set
        {
            if (_isLoading != value)
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }
    }

    public bool HasPreviousPage => _currentPage?.HasPreviousPage ?? false;

    public bool HasNextPage => _currentPage?.HasNextPage ?? false;

    public int CurrentPageIndex => _pageIndex + 1; // Para exibição (1-based)

    public int TotalPages => _currentPage?.TotalPages ?? 0;

    public int TotalItems => _currentPage?.TotalCount ?? 0;

    public string? SearchTerm
    {
        get => _searchTerm;
        set
        {
            if (_searchTerm != value)
            {
                _searchTerm = value;
                OnPropertyChanged();
                // Resetar para a primeira página quando mudar o termo de busca
                _pageIndex = 0;
                LoadCurrentPageAsync().ConfigureAwait(false);
            }
        }
    }

    public ICommand NextPageCommand { get; }
    public ICommand PreviousPageCommand { get; }
    public ICommand RefreshCommand { get; }

    public PaginatedViewModel(Func<int, int, string?, Task<PaginatedResult<T>>> loadPageFunc)
    {
        _loadPageFunc = loadPageFunc ?? throw new ArgumentNullException(nameof(loadPageFunc));

        NextPageCommand = new AsyncRelayCommand(NextPageAsync, () => HasNextPage && !IsLoading);
        PreviousPageCommand = new AsyncRelayCommand(PreviousPageAsync, () => HasPreviousPage && !IsLoading);
        RefreshCommand = new AsyncRelayCommand(RefreshAsync, () => !IsLoading);
    }

    public async Task InitializeAsync()
    {
        await LoadCurrentPageAsync();
    }

    private async Task NextPageAsync()
    {
        if (HasNextPage)
        {
            _pageIndex++;
            await LoadCurrentPageAsync();
        }
    }

    private async Task PreviousPageAsync()
    {
        if (HasPreviousPage)
        {
            _pageIndex--;
            await LoadCurrentPageAsync();
        }
    }

    private async Task RefreshAsync()
    {
        await LoadCurrentPageAsync();
    }

    private async Task LoadCurrentPageAsync()
    {
        try
        {
            IsLoading = true;

            var result = await _loadPageFunc(_pageIndex, _pageSize, _searchTerm);
            _currentPage = result;

            Items.Clear();
            foreach (var item in result.Items)
            {
                Items.Add(item);
            }

            // Notificar mudanças em propriedades relacionadas
            OnPropertyChanged(nameof(HasPreviousPage));
            OnPropertyChanged(nameof(HasNextPage));
            OnPropertyChanged(nameof(CurrentPageIndex));
            OnPropertyChanged(nameof(TotalPages));
            OnPropertyChanged(nameof(TotalItems));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Erro ao carregar página: {ex.Message}");
            // Tratar erro conforme necessário
        }
        finally
        {
            IsLoading = false;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}