using AgroPharm.Models.Sell;

namespace AgroPharm.Interfaces
{
    public interface ISellProduct
    {
        IEnumerable<SellProductResponse> GetSellProducts();
        SellProductRequest Create(SellProductRequest sellProduct);
        public string Delete(int id);
        public SellProductRequest Edit(SellProductRequest sellProduct);
    }
}
