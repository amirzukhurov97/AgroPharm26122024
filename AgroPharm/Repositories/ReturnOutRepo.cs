using AgroPharm.Interfaces;
using AgroPharm.Models.Request;
using AgroPharm.Models.Response;
using Dapper;
using MySql.Data.MySqlClient;

namespace AgroPharm.Repositories
{
    public class ReturnOutRepo : IReturnOut
    {
        private readonly string _connectingString;
        public ReturnOutRepo(string config)
        {
            _connectingString = config;
        }
        public ReturnOutRequest Create(ReturnOutRequest returnOut)
        {
            try
            {
                using var db = new MySqlConnection(_connectingString);
                var sqlQuery = "INSERT INTO returnorganizationproducts (ProductNameID, OrganizationNameID, ReturnOutProductPrice, ReturnOutProductPriceUSD, ReturnOutProductObem, ReturnOutSumProductPrice, ReturnOutSumProductPriceUSD, ReturnOutProductDate, ReturnOutComment) VALUES (@ProductNameID, @OrganizationNameID, @ReturnOutProductPrice, @ReturnOutProductPriceUSD, @ReturnOutProductObem, @ReturnOutProductSumPrice, @ReturnOutProductSumPriceUSD, @ReturnOutProductDate, @ReturnOutComment);";
                db.Execute(sqlQuery, returnOut);
                return returnOut;
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

                var sqlQuery = "DELETE FROM returnorganizationproducts WHERE ID = @id;";
                db.Execute(sqlQuery, new { id });
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public ReturnOutRequest Edit(ReturnOutRequest returnOut)
        {
            try
            {
                using var db = new MySqlConnection(_connectingString);
                var sqlQuery = "UPDATE returnorganizationproducts SET ProductNameID=@ProductNameID, OrganizationNameID = @OrganizationNameID, ReturnOutProductPrice = @ReturnOutProductPrice, ReturnOutProductPriceUSD = @ReturnOutProductPriceUSD, ReturnOutProductObem = @ReturnOutProductObem, ReturnOutSumProductPrice = @ReturnOutProductSumPrice, ReturnOutSumProductPriceUSD = @ReturnOutProductSumPriceUSD, ReturnOutProductDate = @ReturnOutProductDate, ReturnOutComment = @ReturnOutComment WHERE ID = @Id;";
                db.Execute(sqlQuery, returnOut);
                return returnOut;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IEnumerable<ReturnOutResponse> GetReturnOutProducts()
        {
            try
            {
                IEnumerable<ReturnOutResponse> returnOut;
                using var db = new MySqlConnection(_connectingString);
                var sqlQuery = "SELECT o.`OrganizationName`, p.`ProductName`, rop.* " +
                    "FROM returnorganizationproducts AS rop " +
                    "LEFT JOIN Products AS p ON rop.`ProductNameID` = p.`ID` " +
                    "LEFT JOIN organizations AS o ON rop.`OrganizationNameID` = o.`ID` " +
                    "ORDER BY ID DESC;";
                returnOut = db.Query<ReturnOutResponse>(sqlQuery).ToList();
                return returnOut;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ReturnOutResponse GetReturnOutTotal()
        {
            try
            {
                using var db = new MySqlConnection(_connectingString);
                var sqlQuery = "SELECT SUM(ret.`ReturnOutSumProductPrice`) AS ReturnOutSumProductPrice, SUM(ret.`ReturnOutSumProductPriceUSD`) AS ReturnOutSumProductPriceUSD FROM returnorganizationproducts ret;";
                ReturnOutResponse? returnOutProducts = db.QuerySingleOrDefault<ReturnOutResponse>(sqlQuery);
                return returnOutProducts;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
