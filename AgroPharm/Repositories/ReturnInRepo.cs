using AgroPharm.Interfaces;
using AgroPharm.Models.Request;
using AgroPharm.Models.Response;
using Dapper;
using MySql.Data.MySqlClient;

namespace AgroPharm.Repositories
{

    public class ReturnInRepo : IReturnIn
    {
        private readonly string _connectingString;
        public ReturnInRepo(string config)
        {
            _connectingString = config;
        }
        public ReturnInRequest Create(ReturnInRequest returnIn)
        {
            try
            {
                using var db = new MySqlConnection(_connectingString);
                var sqlQuery = "INSERT INTO buyproducts (ProductNameID, OrganizationNameID, BuyProductPrice, BuyProductPriceUSD, BuyProductObem, BuyProductSumPrice, BuyProductSumPriceUSD, BuyProductDate, BuyComment) VALUES (@ProductNameID, @OrganizationNameID, @BuyProductPrice, @BuyProductPriceUSD, @BuyProductObem, @BuyProductSumPrice, @BuyProductSumPriceUSD, @BuyProductDate, @BuyComment);";
                db.Execute(sqlQuery, returnIn);
                return returnIn;
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

                var sqlQuery = "DELETE FROM returnIn WHERE ID = @id;";
                db.Execute(sqlQuery, new { id });
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public ReturnInRequest Edit(ReturnInRequest returnIn)
        {
            try
            {
                using var db = new MySqlConnection(_connectingString);
                var sqlQuery = "UPDATE buyproducts SET ProductNameID=@ProductNameID, OrganizationNameID = @OrganizationNameID, BuyProductPrice = @BuyProductPrice, BuyProductPriceUSD = @BuyProductPriceUSD, BuyProductObem = @BuyProductObem, BuyProductSumPrice = @BuyProductSumPrice, BuyProductSumPriceUSD = @BuyProductSumPriceUSD, BuyProductDate = @BuyProductDate, BuyComment = @BuyComment WHERE ID = @Id;";
                db.Execute(sqlQuery, returnIn);
                return returnIn;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IEnumerable<ReturnInResponse> GetReturnInProducts()
        {
            try
            {
                IEnumerable<ReturnInResponse> returnIn;
                using var db = new MySqlConnection(_connectingString);
                var sqlQuery = "SELECT c.`CustomerName`, p.`ProductName`, rcp.* " +
                    "FROM returncustomerproducts AS rcp " +
                    "LEFT JOIN Products AS p ON rcp.`ProductNameID` = p.`ID` " +
                    "LEFT JOIN customers AS c ON rcp.`CustomerNameID` = c.`ID`" +
                    "ORDER BY ID DESC;";
                returnIn = db.Query<ReturnInResponse>(sqlQuery).ToList();
                return returnIn;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
