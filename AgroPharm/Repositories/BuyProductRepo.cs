
using AgroPharm.Interfaces;
using AgroPharm.Models.Buy;
using Dapper;
using MySql.Data.MySqlClient;

namespace AgroPharm.Repositories
{
    public class BuyProductRepo : IBuyProduct
    {
        private readonly string _connectingString;
        public BuyProductRepo(string config)
        {
            _connectingString = config;
        }

        public BuyProductRequest Create(BuyProductRequest buyProduct)
        {
            try
            {
                using var db = new MySqlConnection(_connectingString);
                var sqlQuery = "INSERT INTO buyproducts (ProductNameID, OrganizationNameID, BuyProductPrice, BuyProductPriceUSD, BuyProductObem, BuyProductSumPrice, BuyProductSumPriceUSD, BuyProductDate, BuyComment) VALUES (@ProductNameID, @OrganizationNameID, @BuyProductPrice, @BuyProductPriceUSD, @BuyProductObem, @BuyProductSumPrice, @BuyProductSumPriceUSD, @BuyProductDate, @BuyComment);";
                db.Execute(sqlQuery, buyProduct);
                return buyProduct;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<BuyProductResponse> GetBuyProducts()
        {
            try
            {
                IEnumerable<BuyProductResponse> buyProducts;
                using var db = new MySqlConnection(_connectingString);                
                var sqlQuery = "SELECT org.`OrganizationName`,p.`ProductName`, buy.* " +
                    "FROM BuyProducts AS buy " +
                    "LEFT JOIN Products AS p ON buy.`ProductNameID` = p.`ID` " +
                    "LEFT JOIN Organizations AS org ON buy.`OrganizationNameID` = org.`ID` " +
                    "ORDER BY ID DESC";
                buyProducts = db.Query<BuyProductResponse>(sqlQuery).ToList();                
                return buyProducts;
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

                var sqlQuery = "DELETE FROM buyProducts WHERE ID = @id;";
                db.Execute(sqlQuery, new { id });
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message; 
            }
        }

        public BuyProductRequest Edit(BuyProductRequest buyProduct)
        {
            try
            {
                using var db = new MySqlConnection(_connectingString);
                var sqlQuery = "UPDATE buyproducts SET ProductNameID=@ProductNameID, OrganizationNameID = @OrganizationNameID, BuyProductPrice = @BuyProductPrice, BuyProductPriceUSD = @BuyProductPriceUSD, BuyProductObem = @BuyProductObem, BuyProductSumPrice = @BuyProductSumPrice, BuyProductSumPriceUSD = @BuyProductSumPriceUSD, BuyProductDate = @BuyProductDate, BuyComment = @BuyComment WHERE ID = @Id;";
                db.Execute(sqlQuery, buyProduct);
                return buyProduct;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public BuyProductPrices GetBuyProductTotal()
        {
            try
            {
                using var db = new MySqlConnection(_connectingString);
                var sqlQuery = "SELECT SUM(buy.`BuyProductSumPrice`) AS BuyProductSumPrice, SUM(buy.`BuyProductSumPriceUSD`) AS BuyProductSumPriceUSD FROM buyproducts buy;";
                BuyProductPrices? buyProducts = db.QuerySingleOrDefault<BuyProductPrices>(sqlQuery);
                return buyProducts;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        
    }
}
