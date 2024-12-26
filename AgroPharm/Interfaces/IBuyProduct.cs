using AgroPharm.Models.Request;
using AgroPharm.Models.Response;

namespace AgroPharm.Interfaces
{
    public interface IBuyProduct
    {
        IEnumerable<BuyProductResponse> GetBuyProducts();
        BuyProductRequest Create(BuyProductRequest buyProduct);

    }
}
