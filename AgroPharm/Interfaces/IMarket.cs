using AgroPharm.Models.Response;

namespace AgroPharm.Interfaces
{
    public interface IMarket
    {
       Task<IEnumerable<Market>> GetMarketList();
    }
}
