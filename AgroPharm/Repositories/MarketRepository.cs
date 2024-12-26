using AgroPharm.Interfaces;
using AgroPharm.Models.Request;
using AgroPharm.Models.Response;
using Dapper;
using MySql.Data.MySqlClient;
using System.Data;

namespace AgroPharm.Repositories
{
    public class MarketRepository
    {
        private readonly string _connectionString;
        public MarketRepository(string configuration) {
            _connectionString = configuration;
        }
        public async Task<IEnumerable<Market>> GetMarketList()
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                var getQuery = @"SELECT market.`ID`, Products.`ProductName`, Market.`obemProducts` FROM market JOIN products ON market.`ProductNameID` = products.`ID`;";
                var resultQuery = await connection.QueryAsync<Market>(getQuery);
                return resultQuery;
            }
            catch (Exception ex)
            {
                // Optionally log the exception here
                throw;
            }
        }
        public void IncomeProduct(MarketRequest marketRequest)
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                var sql = "INSERT INTO market (ProductNameID, obemProducts) VALUES (@ProductNameID, @ObemProducts) ON DUPLICATE KEY UPDATE obemProducts = obemProducts + VALUES (obemProducts);";
                connection.Execute(sql, marketRequest);
            }
            catch (Exception ex)
            { }
        }

        public void OutcomeProduct(MarketRequest marketRequest) 
        {

            try
            {
                using var connection = new MySqlConnection(_connectionString);
                var sql = "INSERT INTO market (ProductNameID, obemProducts) VALUES (@ProductNameID, @ObemProducts) ON DUPLICATE KEY UPDATE obemProducts = obemProducts - VALUES (obemProducts);";
                connection.Execute(sql, marketRequest);
            }
            catch (Exception ex)
            { }
        } 
    }
}
