using AgroPharm.Models;

namespace AgroPharm.Interfaces
{
    public interface IOrganization
    {
        IEnumerable<Organization> GetOrganizations();
        public Task<Organization> CreateOrganization(Organization organization);
        public void UpdateOrganization(Organization organization);
        public string DeleteOrganization(int id);
    }
}
