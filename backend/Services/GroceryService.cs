using HouseOs.Models;
using Microsoft.EntityFrameworkCore; 
using HouseOs.Data;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using System.Text.Json;

namespace HouseOs.Services;

public class GroceryService: IGroceryService
{
    private readonly ApplicationContext _context;
    private readonly IConfiguration _config;
    private readonly ILogger<GroceryService> _logger;

    public GroceryService(ApplicationContext context, IConfiguration config, ILogger<GroceryService> logger)
    {
        _context = context;
        _config = config;
        _logger = logger;
    }


    public async Task<List<Grocery>> GetGroceriesAsync(int userId)
    {
        return await _context.Groceries.Where(x => x.UserId == userId).ToListAsync();
    }


    public async Task<Grocery> GetGroceryByIdAsync(int id, int userId)
    {
        var grocery = await _context.Groceries.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (grocery != null)
        {
            return grocery;
        }
        else
        {
            throw new Exception("Grocery not found.");
        }
    }

    public async Task<Grocery> CreateGroceryAsync(GroceryDTO obj, int userId)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        var grocery = new Grocery
        {
            Name = obj.Name ?? throw new ArgumentNullException(nameof(obj.Name)),
            Quantity = obj.Quantity,
            TotalCost = obj.TotalCost,
            UserId = userId
        };

        _context.Groceries.Add(grocery);
        await _context.SaveChangesAsync();
        return grocery;
    }

    public async Task <Grocery> UpdateGroceryAsync(int id, GroceryDTO obj, int userId)
    {
        var result = await _context.Groceries.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        if (result == null)
        {
            throw new Exception("Grocery not found.");
        }

        result.Name = obj.Name ?? result.Name;
        result.Quantity = obj.Quantity;
        result.TotalCost = obj.TotalCost;

        await _context.SaveChangesAsync();
        return result;

    }

    public async Task<bool> DeleteGroceryAsync(int id, int userId)
    {
        var grocery = await _context.Groceries.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (grocery != null)
        {
            _context.Groceries.Remove(grocery);
            await _context.SaveChangesAsync();
            return true;
        }
        else
        {
            throw new Exception("Grocery not found.");
        }
    }

    public async Task<bool> GroceryExistsAsync(int id)
    {
        return await _context.Groceries.AnyAsync(x => x.Id == id);
    }

    public async Task<List<Grocery>> ScanReceiptAsync(Stream stream, int userId)
    {
        var key = _config["FormRecognizer:Key"];
        var endpoint = _config["FormRecognizer:Endpoint"];

        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(endpoint))
        {
            throw new Exception("Form Recognizer key or endpoint not found.");
        }

        AzureKeyCredential credential = new AzureKeyCredential(key);
        DocumentAnalysisClient client = new DocumentAnalysisClient(new Uri(endpoint), credential);

        AnalyzeDocumentOperation operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-receipt", stream);
        AnalyzeResult result = operation.Value;

        var groceries = new List<Grocery>();

        foreach(var document in result.Documents)
        {
            if (document.Fields.TryGetValue("Items", out var  itemsField) && itemsField.FieldType == DocumentFieldType.List)
            {
                foreach (var itemField in itemsField.Value.AsList())
                {
                    if (itemField.FieldType == DocumentFieldType.Dictionary)
                    {
                        var itemFields = itemField.Value.AsDictionary();
                        var grocery = new Grocery
                        {
                            
                            Name = GetFieldValueSafely(itemFields, "Description", "Unknown Item"),
            
                            Quantity = (int)GetNumericFieldValueSafely(itemFields, "Quantity", 1),
                            TotalCost = GetNumericFieldValueSafely(itemFields, "TotalPrice", 0),
                            UserId = userId
                        };
                        groceries.Add(grocery);
                    }

                }

            }
            
        }
        await _context.Groceries.AddRangeAsync(groceries);
        await _context.SaveChangesAsync();

        return groceries;
    }

    private string GetFieldValueSafely(IReadOnlyDictionary<string, DocumentField> fields, string fieldName, string defaultValue)

        {
            if (fields.TryGetValue(fieldName, out var field) && field.Value != null)
            {
                return field.Value.AsString() ?? defaultValue;
            }
            return defaultValue;
        }

    private decimal GetNumericFieldValueSafely(IReadOnlyDictionary<string, DocumentField> fields, string fieldName, decimal defaultValue)

        {
            if (fields.TryGetValue(fieldName, out var field) && field.Value != null)
            {
                try
                {
                    return (decimal)field.Value.AsDouble();
                }
                catch
                {
                    if (decimal.TryParse(field.Content, out decimal result))
                    {
                        return result;
                    }

                    Console.WriteLine($"Failed to parse {fieldName} value: {field.Content}");
                }

            }
            return defaultValue;
        }
        

    public async Task<List<Grocery>> SearchGroceriesAsync(string name, int userId)

    {
        var lowerCaseName = name.ToLower();
        var query = await _context.Groceries
                                    .Where(x => x.UserId == userId && x.Name.ToLower().Contains(lowerCaseName))
                                    .ToListAsync();
        return query;
    }

    public async Task<AnalyticsPeriod> GetAnalyticsDataAsync(int userId, DateTime startDate, DateTime endDate)
    {
    
        var query = await _context.Groceries
                                    .Where(x => x.UserId == userId && x.CreatedAt >= startDate.ToUniversalTime() && x.CreatedAt < endDate.ToUniversalTime())
                                    .ToListAsync();

        return new AnalyticsPeriod
        {
            Count = query.Count,
            TotalSpent = query.Sum(x => x.TotalCost),
            StartDate = startDate.ToUniversalTime(),
            EndDate = endDate.ToUniversalTime()
        };
    }

    public async Task<List<Grocery>> UploadGroceryAsync(IFormFile file, int userId)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentNullException(nameof(file));
        }

        using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream);
        var jsonContent = await reader.ReadToEndAsync();

        var uploadedGroceries = JsonSerializer.Deserialize<List<GroceryDTO>>(jsonContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (uploadedGroceries == null || uploadedGroceries.Count == 0)
        {
            throw new Exception("No groceries found in the file.");
        }

        var groceries = new List<Grocery>();

        foreach (var item in uploadedGroceries)
        {
           

            var grocery = new Grocery
            {
                Name = item.Name!,
                Quantity = item.Quantity,
                TotalCost = item.TotalCost,
                UserId = userId,
                CreatedAt = item.CreatedAt.Kind == DateTimeKind.Unspecified 
                            ? DateTime.SpecifyKind(item.CreatedAt, DateTimeKind.Utc)
                            : item.CreatedAt.ToUniversalTime()
            };
            groceries.Add(grocery);
        }
        await _context.Groceries.AddRangeAsync(groceries);
        await _context.SaveChangesAsync();
        return groceries;
    }


    public async Task<IEnumerable<PopularItems>> GetPopularItemsAsync(int userId, int count)
    {
        try{

           var query = _context.Groceries
                                .Where(x => x.UserId == userId)
                                .GroupBy(x => x.Name)
                                .Select(x => new PopularItems
                                {
                                    Name = x.Key,
                                    Count = x.Count(),
                                    AverageSpend = (decimal)x.Average(y => y.UnitCost)
                                })
                                .OrderByDescending(x => x.Count)
                                .Take(count);

            return await query.ToListAsync();
                                
            }   
            catch (Exception ex)
            {
                _logger.LogError( ex, "Error getting popular items UserId: {UserId}, Count: {Count}", userId, count);
                throw;
            }

    }


    public async Task<Dictionary<string, List<PriceTrend>>> GetPriceTrendsAsync(int userId)
    {
        var topItems = await GetPopularItemsAsync(userId, 5);
        var topItemsNames = topItems.Select(x => x.Name).ToList();

        _logger.LogInformation("Top Items: {TopItems}", topItemsNames);

        var groceries = await _context.Groceries
                                        .Where(x => x.UserId == userId 
                                        && topItemsNames.Contains(x.Name)
                                        )
                                        .ToListAsync();

        _logger.LogInformation("Groceries: {Groceries}", groceries);


        var priceTrends = groceries
                                        .GroupBy(x => new {
                                             x.Name, 
                                             Month = x.CreatedAt.Month, 
                                             Year = x.CreatedAt.Year
                                             })
                                        .Select(x => new 
                                        {
                                            Name = x.Key.Name,
                                            Month = x.Key.Month,
                                            Year = x.Key.Year,
                                            AveragePrice = Math.Round(x.Average(y => (decimal)y.UnitCost), 2)
                                        })
                                        .ToList();

        _logger.LogInformation("Price Trends: {PriceTrends}", priceTrends);

        var result = priceTrends
                        .GroupBy(x=> x.Name)
                        .ToDictionary(
                            x => x.Key, 
                            x => x.Select(y => new PriceTrend
                        {
                            Date = new DateTime (y.Year, y.Month, 1),
                            Price = Math.Round(y.AveragePrice, 2)
                        }).OrderBy(y => y.Date).ToList()
                        );

                        _logger.LogInformation("Price Trends: {PriceTrends}", result);
     
        return result;
                                
                                        
    }
       
    



}
