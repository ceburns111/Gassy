using Microsoft.AspNetCore.Mvc;
using Gassy.Models; 
namespace Gassy.Services
{
    public interface IReverbListingService
    {
        Task<IEnumerable<ReverbListingDto>> GetListings(); 
        Task<ReverbListingDto> GetListing(int id);
        Task<ReverbListingDto> CreateListing(ReverbListingDto listing); 
        Task<ReverbListingDto> UpdateListing(ReverbListingDto listing);
        Task<int> DeleteListing(int reverbId);
    }

}