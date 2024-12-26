using AgroPharm.Models;

namespace AgroPharm.Interfaces
{
    public interface IProduct
    {
        IEnumerable<Product> GetProducts();
        public Task<Product> CreateProduct(Product product);
        public void UpdateProduct(Product product);
        public string DeleteProduct(int id);

    }
}
