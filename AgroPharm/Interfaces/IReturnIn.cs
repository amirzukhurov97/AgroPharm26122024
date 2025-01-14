using AgroPharm.Models.Request;
using AgroPharm.Models.Response;

namespace AgroPharm.Interfaces
{
    public interface IReturnIn
    {
        IEnumerable<ReturnInResponse> GetReturnInProducts();
        ReturnInRequest Create(ReturnInRequest returnIn);
        public string Delete(int id);
        public ReturnInRequest Edit(ReturnInRequest returnIn);
    }
}
