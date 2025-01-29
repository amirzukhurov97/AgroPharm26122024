using AgroPharm.Models.Buy;

namespace AgroPharm.Interfaces
{
    public interface IBuyProduct
    {
        IEnumerable<BuyProductResponse> GetBuyProducts();
        BuyProductRequest Create(BuyProductRequest buyProduct);
        public string Delete(int id);
        public BuyProductRequest Edit(BuyProductRequest buyProduct);

    }
}
