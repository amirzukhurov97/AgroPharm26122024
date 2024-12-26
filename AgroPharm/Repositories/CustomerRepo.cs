using AgroPharm.Interfaces;
using AgroPharm.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace AgroPharm.Repositories
{
    public class CustomerRepo : ICustomer
    {
        private readonly string _connectionString;
        public CustomerRepo(string config)
        {
            _connectionString = config;
        }
        public async Task<Customer> CreateCustomer(Customer customer)
        {
            try
            {
                using var db = new MySqlConnection(_connectionString);
                var sqlQuery = "INSERT INTO products (ProductName) VALUES (@ProductName);";
                var res = await db.ExecuteAsync(sqlQuery, customer);
                return customer;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public string DeleteCustomer(int id)
        {
            try
            {
                using var db = new MySqlConnection(_connectionString);

                var sqlQuery = "DELETE FROM customers WHERE ID = @id;";
                db.Execute(sqlQuery, new { id });

                return $"Наименование полупателя с ID {id} удалён!";
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Customer> GetCustomers()
        {
            try
            {
                using var db = new MySqlConnection(_connectionString);
                var sqlQuery = "SELECT * FROM customers ORDER BY ID DESC;";
                IEnumerable<Customer> customers = db.Query<Customer>(sqlQuery);
                return customers;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public void UpdateCustomer(Customer customers)
        {
            try
            {
                using var db = new MySqlConnection(_connectionString);
                db.Execute("UPDATE customers SET customername = @customername WHERE id = @id;", customers);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool CheckCustomerName(string customerName)
        {
            try
            {
                using var db = new MySqlConnection(_connectionString);
                var sql = "SELECT customerName FROM customers WHERE customerName = @customerName;";
                var result = db.Query(sql, new { customerName });

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
