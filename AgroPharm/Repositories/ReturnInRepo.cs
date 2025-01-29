using AgroPharm.Interfaces;
using AgroPharm.Models.ReturnIn;
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
                var sqlQuery = "INSERT INTO returncustomerproducts (ProductNameID, CustomerNameID, ReturnInProductPrice, ReturnInProductPriceUSD, ReturnInProductObem, ReturnInSumProductPrice, ReturnInSumProductPriceUSD, ReturnInProductDate, ReturnInComment) VALUES (@ProductNameID, @CustomerNameID, @ReturnInProductPrice, @ReturnInProductPriceUSD, @ReturnInProductObem, @ReturnInProductSumPrice, @ReturnInProductSumPriceUSD, @ReturnInProductDate, @ReturnInComment);";
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

                var sqlQuery = "DELETE FROM returncustomerproducts WHERE ID = @id;";
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
                var sqlQuery = "UPDATE returncustomerproducts SET ProductNameID=@ProductNameID, CustomerNameID = @CustomerNameID, ReturnInProductPrice = @ReturnInProductPrice, ReturnInProductPriceUSD = @ReturnInProductPriceUSD, ReturnInProductObem = @ReturnInProductObem, ReturnInSumProductPrice = @ReturnInProductSumPrice, ReturnInSumProductPriceUSD = @ReturnInProductSumPriceUSD, ReturnInProductDate = @ReturnInProductDate, ReturnInComment = @ReturnInComment WHERE ID = @Id;";
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

        public ReturnInProductPrices? GetReturnInTotal()
        {
            try
            {
                using var db = new MySqlConnection(_connectingString);
                var sqlQuery = "SELECT SUM(ret.`ReturnInSumProductPrice`) AS ReturnInSumProductPrice, SUM(ret.`ReturnInSumProductPriceUSD`) AS ReturnInSumProductPriceUSD FROM returncustomerproducts ret;";
                ReturnInProductPrices? returnInProducts = db.QuerySingleOrDefault<ReturnInProductPrices>(sqlQuery);
                return returnInProducts;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
