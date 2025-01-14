using AgroPharm.Models.Request;
using AgroPharm.Models.Response;

namespace AgroPharm.Interfaces
{
    public interface IReturnOut
    {
        IEnumerable<ReturnOutResponse> GetReturnOutProducts();
        ReturnOutRequest Create(ReturnOutRequest returnOut);
        public string Delete(int id);
        public ReturnOutRequest Edit(ReturnOutRequest returnOut);
    }
}
