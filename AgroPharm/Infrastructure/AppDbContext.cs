using Microsoft.Extensions.Configuration;


namespace AgroPharm.Infrastructure
{
    
    public class AppDbContext
    {
        private readonly IConfiguration _configuration;
        public AppDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GetConnectionString()
        {
            return _configuration.GetConnectionString("DefaultConnection");      
        }
    }
}
