using AgroPharm.Models;

namespace AgroPharm.Interfaces
{
    public interface ICustomer
    {
        IEnumerable<Customer> GetCustomers();
        public Task<Customer> CreateCustomer(Customer product);
        public void UpdateCustomer(Customer product);
        public string DeleteCustomer(int id);
    }
}
