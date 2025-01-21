using AgroPharm.Interfaces;
using AgroPharm.Models;
using AgroPharm.Models.Response;
using Dapper;
using MySql.Data.MySqlClient;

namespace AgroPharm.Repositories
{
    public class CurrencyRepo
    {
        private readonly string _connectionString;
        public CurrencyRepo(string configuration)
        {
            _connectionString = configuration;
        }
        public IEnumerable<Currency> GetCurrencyList()
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                var getQuery = "SELECT * FROM currencytable ORDER BY ID DESC;";
                var resultQuery = connection.Query<Currency>(getQuery);
                return resultQuery;
            }
            catch (MySqlException e)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string GetLastCurrency()
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                connection.Open();

                var getQuery = "SELECT USDtoTJS FROM currencytable ORDER BY ID DESC LIMIT 1;";
                var result = connection.QuerySingleOrDefault<string>(getQuery);
                return result ?? "No data found";
            }
            catch (MySqlException e)
            {
                throw new Exception($"Ошибка базы данных: {e.Message}", e);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при получении курса: {ex.Message}", ex);
            }
        }

        public Currency AddCurrency(Currency currency) 
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                connection.Open();

                var sqlQuery = "INSERT INTO currencytable (USDtoTJS, DateChangeCurrency) VALUES (@USDtoTJS, @DateChangeCurrency);";
                connection.Execute(sqlQuery, currency);
                return currency;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при обновлении курса: {ex.Message}", ex);
            }
        }

    }
}
