using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace QOTS
{
	public class DatabaseService
	{
		private SQLiteAsyncConnection _db;

		public async Task UpdateQuoteAsync(Quote quote)
		{
			await _db.UpdateAsync(quote);
		}

		public async Task DeleteQuoteAsync(Quote quote)
		{
			await _db.DeleteAsync(quote);
		}

		public async Task Init()
		{
			if (_db != null)
				return;

			var dbPath = Path.Combine(FileSystem.AppDataDirectory, "quotes.db");
			_db = new SQLiteAsyncConnection(dbPath);

			await _db.CreateTableAsync<Quote>();
		}

		public async Task AddQuoteAsync(Quote quote)
		{
			await _db.InsertAsync(quote);
		}

		public async Task<List<Quote>> GetQuotesAsync()
		{
			return await _db.Table<Quote>().ToListAsync();
		}
	}
}
