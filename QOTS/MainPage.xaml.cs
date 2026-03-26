using QOTS;
using SQLite;


public class Quote
{
	[PrimaryKey, AutoIncrement]
	public int Id { get; set; }
	public string Text { get; set; } = "";
	public string BookTitle { get; set; } = "";
	public string Author { get; set; } = "";
	public string Tags { get; set; } = "";
	public int PageNumber { get; set; }
	public bool IsFavorite { get; set; }
	public string BookInfo => $"{Author} - {BookTitle} (oldal: {PageNumber})";
}

namespace QOTS
{
	public partial class MainPage : ContentPage
	{
		List<Quote> allQuotes = new List<Quote>();
		List<Quote> quotes = new List<Quote>();
		DatabaseService database = new DatabaseService();

		public MainPage()
		{
			InitializeComponent();
			LoadQuotes();
		}

		private async void LoadQuotes()
		{
			await database.Init();
			var quotes = await database.GetQuotesAsync();

			allQuotes = quotes;

			MainThread.BeginInvokeOnMainThread(() =>
			{
				QuotesList.ItemsSource = quotes;
			});
		}

		private async void OnSaveClicked(object sender, EventArgs e)
		{
			var quote = new Quote
			{
				Text = QuoteEntry.Text,
				BookTitle = BookTitleEntry.Text,
				Author = AuthorEntry.Text,
				PageNumber = int.TryParse(PageEntry.Text, out int page) ? page : 0,
				Tags = TagsEntry.Text ?? ""
			};

			await database.AddQuoteAsync(quote);

			LoadQuotes();

			QuoteEntry.Text = "";
			BookTitleEntry.Text = "";
			AuthorEntry.Text = "";
			PageEntry.Text = "";
			TagsEntry.Text = "";
		}

		private async void OnDeleteClicked(object sender, EventArgs e)
		{
			bool confirm = await DisplayAlert("Törlés", "Biztos törlöd?", "Igen", "Nem");

			if (!confirm)
				return;

			var button = sender as Button;
			var quote = button?.CommandParameter as Quote;

			if (quote == null)
				return;

			await database.DeleteQuoteAsync(quote);

			LoadQuotes();
		}

		private async void OnFavoriteClicked(object sender, EventArgs e)
		{
			var button = sender as Button;
			var quote = button?.CommandParameter as Quote;

			if (quote == null)
				return;

			quote.IsFavorite = !quote.IsFavorite;

			await database.UpdateQuoteAsync(quote);

			LoadQuotes();
		}

		private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
		{
			string searchText = e.NewTextValue?.ToLower() ?? "";

			var filtered = allQuotes.Where(q =>
				q.Text.ToLower().Contains(searchText) ||
				q.BookTitle.ToLower().Contains(searchText) ||
				q.Author.ToLower().Contains(searchText) ||
				q.Tags.ToLower().Contains(searchText)
			).ToList();

			QuotesList.ItemsSource = filtered;
		}


	}
}