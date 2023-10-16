using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MVCProject.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ProductServices
{
    private readonly ProductContext _context;

    public ProductServices(ProductContext context)
    {
        _context = context;
    }


    public Product GetProductById(int id)
    {
        return _context.Products.FirstOrDefault(p => p.ProductId == id);
    }
    public byte[] GetImageById(int productId)
    {
        // Assuming you have a GetProductById method to retrieve the product
        var product = GetProductById(productId);

        if (product != null)
        {
            // Return the image binary data from the product
            return product.Image;
        }

        // Return null or handle the case where the product is not found
        return null;
    }



   public void UpsertProduct(Product product)
{
    try
    {
        _context.Database.ExecuteSqlRaw("EXEC UpsertProduct @ProductId, @Code, @Name, @Description, @ExpiryDate, @Category, @Image, @Status",
            new SqlParameter("ProductId", product.ProductId),
            new SqlParameter("Code", product.Code),
            new SqlParameter("Name", product.Name),
            new SqlParameter("Description", product.Description),
            new SqlParameter("ExpiryDate", product.ExpiryDate),
            new SqlParameter("Category", product.Category),
            new SqlParameter("Image", product.Image),
            new SqlParameter("Status", product.Status)
        );
    }
    catch (DbUpdateException ex)
    {
        // Log the exception or handle it as required
        throw new Exception("An error occurred while upserting the product.", ex);
    }
}
    public void DeleteProduct(int productId)
    {
        try
        {
            _context.Database.ExecuteSqlRaw("EXEC DeleteProduct @ProductId",
       new SqlParameter("ProductId", productId));
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as required
            throw new Exception("An error occurred while Deleting the product.", ex);
        }
    }

    public PaginatedList<Product> SearchProducts(string searchQuery, int pageNumber, int pageSize)
    {
        try
        {
            // Check if the search query is a valid date
            bool isDate = DateTime.TryParse(searchQuery, out DateTime parsedDate);

        var query = _context.Products
            .Where(p => p.Name.Contains(searchQuery) ||
                        p.Description.Contains(searchQuery) ||
                        p.Code.Contains(searchQuery) ||
                        p.ProductId.ToString().Contains(searchQuery) ||
                        p.Category.Contains(searchQuery) ||
                       (isDate && (p.ExpiryDate == parsedDate ))||
                (isDate && p.CreationDate.Date == parsedDate.Date));

            // Count the total number of products matching the search query
            var totalProducts = query.Count();

        // Apply pagination and retrieve the relevant products
        var items = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        // Create a paginated list based on the retrieved products and other details
        var products = new PaginatedList<Product>(items, totalProducts, pageNumber, pageSize);

        return products;
        }
        catch (Exception ex)
        {
            // Handle the exception here
            // You can log the exception, provide a default value, or perform any other necessary action
            // For example:
            Console.WriteLine($"An error occurred: {ex.Message}");
            return new PaginatedList<Product>(new List<Product>(), 0, pageNumber, pageSize); // Return an empty list
        }
    }
    public PaginatedList<Product> GetPaginatedProducts(int pageIndex, int pageSize, string sortOrder)
    {
        try
        {
            var products = _context.Products.FromSqlRaw("EXEC SP_GetPagedProducts @PageNumber, @PageSize, @SortOrder",
                new SqlParameter("@PageNumber", pageIndex),
                new SqlParameter("@PageSize", pageSize),
                new SqlParameter("@SortOrder", sortOrder)).ToList();

            // Modify this logic to correctly handle pagination and sorting
            // ...

            return new PaginatedList<Product>(products, products.Count, pageIndex, pageSize);
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as required
            throw new Exception("An error occurred in GetpaginatedProducts.", ex);
        }
    }

    public int GetTotalProductCount()
    {
        return _context.Products.Count();
    }



}
