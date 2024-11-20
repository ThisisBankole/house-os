using System;
using HouseOs.Models;

namespace HouseOs.Services;

public interface IGroceryService
{
    Task <List<Grocery>> GetGroceriesAsync(int userId);
    Task<Grocery> GetGroceryByIdAsync(int id, int userId);
    Task<Grocery> CreateGroceryAsync(GroceryDTO obj, int userId);
    Task<Grocery> UpdateGroceryAsync(int id, GroceryDTO obj, int userId);
    Task<bool> DeleteGroceryAsync(int id, int userId);
    Task<bool> GroceryExistsAsync(int id);
    Task<List<Grocery>> ScanReceiptAsync(Stream receiptStream, int userId);

    // I want to add a method for searching groceries by name
    Task<List<Grocery>> SearchGroceriesAsync(string name, int userId);
    Task<AnalyticsPeriod> GetAnalyticsDataAsync(int userId, DateTime startDate, DateTime endDate);
    // method  to get popular items
    Task<IEnumerable<PopularItems>> GetPopularItemsAsync(int userId, int count);
    Task<Dictionary<string, List<PriceTrend>>> GetPriceTrendsAsync(int userId);
    Task<List<Grocery>> UploadGroceryAsync(IFormFile file, int userId);

}
