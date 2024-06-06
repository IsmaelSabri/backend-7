﻿using Homes.Models;

namespace Homes.Services
{
    /// <summary>
    /// An interface which helps to do intermediate calculation before projecting calls to the core component. 
    /// </summary>
    /// <typeparam name="T">Any class which decorates with ElasticsearchType and Description attribute.</typeparam>
    public interface IElasticService<T> //where T : class
    {
        Task<string> AddDocumentAsync(T value);
        Task<T> GetDocumentAsync(string id);
        Task<IEnumerable<T>> GetAllDocuments();
        Task<string> UpdateDocumentAsync(T value);
        Task<string> DeleteDocumentAsync(string id);
        public string GenerateRandomAlphanumericString();
        IQueryable<T> GetAllPagedDocuments();
        IQueryable<Flat> SearchFlatDocumentsAsync();
        IQueryable<House> SearchHouseDocumentsAsync();
        IQueryable<Room> SearchRoomDocumentsAsync();
        IQueryable<HolidayRent> SearchHolidayRentDocumentsAsync();
        IQueryable<NewProject> SearchNewProjectDocumentsAsync();
    }
}