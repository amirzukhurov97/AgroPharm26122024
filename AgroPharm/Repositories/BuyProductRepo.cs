using AgroPharm.Infrastructure;
using AgroPharm.Interfaces;
using AgroPharm.Models.Request;
using AgroPharm.Models.Response;
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
                var sqlQuery = "SELECT BuyProducts.ID, BuyProducts.ProductNameID, Products.ProductName, BuyProducts.OrganizationNameID, Organizations.OrganizationName,BuyProducts.BuyProductPrice,BuyProducts.BuyProductPriceUSD, BuyProducts.BuyProductObem, BuyProducts.BuyProductSumPrice, BuyProducts.BuyProductSumPriceUSD,BuyProducts.BuyProductDate, BuyProducts.BuyComment FROM BuyProducts JOIN Products ON BuyProducts.ProductNameID = Products.ID JOIN Organizations ON BuyProducts.OrganizationNameID = Organizations.ID ORDER BY ID DESC";
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

                return $"Закупка с ID {id} удалён!";
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}
