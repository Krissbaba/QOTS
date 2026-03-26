namespace QOTS;

public partial class FavoritesPage : ContentPage
{
	DatabaseService database = new DatabaseService();

	public FavoritesPage()
	{
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		await database.Init();

		var allQuotes = await database.GetQuotesAsync();
		var favorites = allQuotes.Where(q => q.IsFavorite).ToList();

		FavoritesList.ItemsSource = favorites;
	}
}