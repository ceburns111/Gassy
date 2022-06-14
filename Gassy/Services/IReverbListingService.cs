using Microsoft.AspNetCore.Mvc;
using Gassy.Models; 

namespace Gassy.Services
{
    public interface IReverbListingService
    {
        Task<IEnumerable<ListingDto>> GetListings(); 
        Task<ListingDto> GetListing(int id);
        Task<ListingDto> CreateListing(ListingDto listing); 
        Task<ListingDto> UpdateListing(ListingDto listing);
        Task<int> DeleteListing(int reverbId);
    }

}