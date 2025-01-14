using AgroPharm.Interfaces;
using AgroPharm.Models.Request;
using AgroPharm.Models.Response;
using Dapper;
using MySql.Data.MySqlClient;

namespace AgroPharm.Repositories
{
    public class SellProductRepo : ISellProduct
    {
        private readonly string _connectingString;
        public SellProductRepo(string conn)
        {
            _connectingString = conn;            
        }
        public SellProductRequest Create(SellProductRequest sellProduct)
        {
            try
            {
                using var db = new MySqlConnection(_connectingString);
                var sqlQuery = "INSERT INTO sellproducts (ProductNameID, CustomerNameID, SellProductPrice, SellProductPriceUSD, SellProductObem, SellProductSumPrice, SellProductSumPriceUSD, SellProductDate, SellComment) VALUES (@ProductNameID, @CustomerNameID, @SellProductPrice, @SellProductPriceUSD, @SellProductObem, @SellProductSumPrice, @SellProductSumPriceUSD, @SellProductDate, @SellComment);";
                db.Execute(sqlQuery, sellProduct);
                return sellProduct;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string Delete(int id)
        {
            try
            {
                using var db = new MySqlConnection(_connectingString);
                var sqlQuery = "DELETE FROM sellProducts WHERE ID = @id;";
                db.Execute(sqlQuery, new { id });
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public SellProductRequest Edit(SellProductRequest sellProduct)
        {
            try
            {
                using var db = new MySqlConnection(_connectingString);
                var sqlQuery = "UPDATE sellproducts SET ProductNameID=@ProductNameID, CustomerNameID = @CustomerNameID, SellProductPrice = @SellProductPrice, SellProductPriceUSD = @SellProductPriceUSD, SellProductObem = @SellProductObem, SellProductSumPrice = @SellProductSumPrice, SellProductSumPriceUSD = @SellProductSumPriceUSD, SellProductDate = @SellProductDate, SellComment = @SellComment WHERE ID = @Id;";
                db.Execute(sqlQuery, sellProduct);
                return sellProduct;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IEnumerable<SellProductResponse> GetSellProducts()
        {
            try
            {
                IEnumerable<SellProductResponse> sellProducts;
                using var db = new MySqlConnection(_connectingString);
                var sqlQuery = "SELECT c.`CustomerName`, p.`ProductName`, buy.*  " +
                    "FROM sellproducts AS buy                 " +
                    "LEFT JOIN Products AS p ON buy.`ProductNameID` = p.`ID`    " +
                    "LEFT JOIN customers AS c ON buy.`CustomerNameID` = c.`ID`" +
                    "ORDER BY ID DESC;";
                sellProducts = db.Query<SellProductResponse>(sqlQuery).ToList();
                return sellProducts;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
