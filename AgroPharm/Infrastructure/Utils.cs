using AgroPharm.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace AgroPharm.Infrastructure
{
    public class Utils
    {
        public readonly string _connectionString;
        public Utils(AppDbContext config)
        {
            _connectionString = config.GetConnectionString();   
        }

        public void IncomeProduct(string productName)
        {
            try
            {
                int productId = 0;
                using (var db = new MySqlConnection(_connectionString))
                {

                    productId = Convert.ToInt32(db.Query("SELECT ID FROM products WHERE ProductName = @ProductName;", productName));
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}
