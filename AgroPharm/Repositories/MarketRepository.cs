using AgroPharm.Interfaces;
using AgroPharm.Models.Market;
using Dapper;
using Microsoft.AspNetCore.Mvc;
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
        public IEnumerable<Market> GetMarketList()
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                var getQuery = @"SELECT market.`ID`, market.`ProductNameID`,  Products.`ProductName`, Market.`obemProducts` FROM market JOIN products ON market.`ProductNameID` = products.`ID`;";
                var resultQuery = connection.Query<Market>(getQuery);
                return resultQuery;
            }
            catch(MySqlException e)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public string IncomeProduct(MarketRequest marketRequest)
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                var sql = "INSERT INTO market (ProductNameID, obemProducts) VALUES (@ProductNameID, @ObemProducts) ON DUPLICATE KEY UPDATE obemProducts = obemProducts + VALUES (obemProducts);";
                connection.Execute(sql, marketRequest);
                return "OK";
            }
            catch (Exception ex)
            {
                return $"Ошибка при обновлении таблицы Market: {ex.Message}";
            }
        }

        public string OutcomeProduct(MarketRequest marketRequest) 
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                var getQuantity = "SELECT obemProducts FROM market WHERE ProductNameID = @ProductNameID;";
                var res = connection.Query<Market>(getQuantity, new { marketRequest.ProductNameID }).FirstOrDefault();
                if (res.ObemProducts>=marketRequest.obemProducts) 
                {
                    var sql = "INSERT INTO market (ProductNameID, obemProducts) VALUES (@ProductNameID, @ObemProducts) ON DUPLICATE KEY UPDATE obemProducts = obemProducts - VALUES (obemProducts);";
                    connection.Execute(sql, marketRequest);
                    return "OK";
                }
                else
                {
                    return $"Внимание: Недостаточное количество товара на складе.\n Количество такого товара на складе = {res.ObemProducts}. Удалить невозможно!";
                }
            }
            catch (Exception ex)
            {
                return $"Ошибка при обновлении таблицы Market: {ex.Message}";
            }
        } 
    }
}
