using AgroPharm.Interfaces;
using AgroPharm.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace AgroPharm.Repositories
{
    public class OrganizationRepo : IOrganization
    {
        private readonly string _connectionString;
        public OrganizationRepo(string config)
        {
            _connectionString = config;
        }
        public async Task<Organization> CreateOrganization(Organization organization)
        {
            try
            {
                using var db = new MySqlConnection(_connectionString);
                var sqlQuery = "INSERT INTO organizations (organizationName) VALUES (@organizationName);";
                var res = await db.ExecuteAsync(sqlQuery, organization);
                return organization;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string DeleteOrganization(int id)
        {
            try
            {
                using var db = new MySqlConnection(_connectionString);

                var sqlQuery = "DELETE FROM organizations WHERE ID = @id;";
                db.Execute(sqlQuery, new { id });

                return $"Наименование товара с ID {id} удалён!";
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Organization> GetOrganizations()
        {
            try
            {
                using var db = new MySqlConnection(_connectionString);
                var sqlQuery = "SELECT * FROM organizations ORDER BY ID DESC;";
                IEnumerable<Organization> organizations = db.Query<Organization>(sqlQuery);
                return organizations;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public void UpdateOrganization(Organization organization)
        {
            try
            {
                using var db = new MySqlConnection(_connectionString);
                db.Execute("UPDATE organizations SET organizationName = @organizationName WHERE id = @id;", organization);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool CheckOrganziationName(string organizationName)
        {
            try
            {
                using var db = new MySqlConnection(_connectionString);
                var sql = "SELECT organizationName FROM organizations WHERE organizationName = @organizationName;";
                var result = db.Query(sql, new { organizationName });

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
