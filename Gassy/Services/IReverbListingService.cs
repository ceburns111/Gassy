using Microsoft.AspNetCore.Mvc;
using Gassy.Models.ReverbModels;

namespace Gassy.Services
{
    public interface IReverbListingService
    {
        Task<IEnumerable<ReverbListing>> GetListings(); 
        Task<ReverbListing> GetListing(int id);
        Task<ReverbListing> CreateListing(ReverbListing listing); 
        Task<ReverbListing> UpdateListing(ReverbListing listing);
        Task<int> DeleteListing(int reverbId);
    }

}