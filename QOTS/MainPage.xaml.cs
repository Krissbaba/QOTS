using QOTS;
using SQLite;


public class Quote
{
	[PrimaryKey, AutoIncrement]
	public int Id { get; set; }

	public string Text { get; set; }
	public string BookTitle { get; set; }
	public string Author { get; set; }
	public int PageNumber { get; set; }

	public bool IsFavorite { get; set; }

	public string BookInfo => $"{Author} - {BookTitle} (oldal: {PageNumber})";
}

namespace QOTS
{
	public partial class MainPage : ContentPage
	{
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
				PageNumber = int.TryParse(PageEntry.Text, out int page) ? page : 0
			};

			await database.AddQuoteAsync(quote);

			LoadQuotes();

			QuoteEntry.Text = "";
			BookTitleEntry.Text = "";
			AuthorEntry.Text = "";
			PageEntry.Text = "";
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


	}
}