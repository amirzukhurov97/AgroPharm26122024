using AgroPharm.Infrastructure;
using AgroPharm.Interfaces;
using AgroPharm.Models;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using MySql.Data.MySqlClient;

namespace AgroPharm.Repositories
{
    public class ProductRepo : IProduct
    {
        private readonly string _connectionString;
        public ProductRepo(string config)
        {
            _connectionString = config;
        }
        public async Task<Product> CreateProduct(Product product)
        {
            try
            {
                using var db = new MySqlConnection(_connectionString);  
                var sqlQuery = "INSERT INTO products (ProductName) VALUES (@ProductName);";
                var res = await db.ExecuteAsync(sqlQuery, product);  
                return product;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }

        public string DeleteProduct(int id)
        {
            try
            {
                using var db = new MySqlConnection(_connectionString);
                
                var sqlQuery = "DELETE FROM products WHERE ID = @id;";
                db.Execute(sqlQuery, new { id });
                
                return $"Наименование товара с ID {id} удалён!";
            }
            catch (Exception ex)
            {

                return $"Ошибка при удалении {ex.Message}";
            }
        }

        public IEnumerable<Product> GetProducts()
        {
            try
            {                
                using var db = new MySqlConnection(_connectionString);                
                var sqlQuery = "SELECT * FROM products ORDER BY ID DESC;";
                IEnumerable<Product> products = db.Query<Product>(sqlQuery);                
                return products;
            }
            catch (Exception ex) 
            {

                throw new Exception(ex.Message);
            }         
        }

        public void UpdateProduct(Product product)
        {
            try
            {
                using var db = new MySqlConnection(_connectionString);
                db.Execute("UPDATE products SET productname = @productname WHERE id = @id;", product);                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool CheckProductName(string productName)
        {
            try
            {
                using var db = new MySqlConnection(_connectionString);
                var sql = "SELECT productName FROM products WHERE productName = @productName;";
                var result = db.Query(sql, new { productName });

                bool res = result.Count() != 0 ? true : false;
                return res;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }
    }
}
