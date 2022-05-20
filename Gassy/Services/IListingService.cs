using Gassy.Models;

namespace Gassy.Services
{
    public interface IListingService
    {
        Task<IEnumerable<Listing>> GetListings();
        Task<Listing> AddListing(Listing listing); 
    }

}