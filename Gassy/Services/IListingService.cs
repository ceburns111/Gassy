using Gassy.Models;

namespace Gassy.Services
{
    public interface IListingService
    {
        Task<IEnumerable<Listing>> GetAllListings();
        Task<Listing> AddListing(Listing listing); 
    }

}