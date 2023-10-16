using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MVCProject.Models;
using MVCProject.Models.Context;
using MVCProject.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Controllers
{
    public class ProductController : Controller
    {
            // GET: ProductController
            private readonly ProductContext _product;
            private readonly ProductServices _productServices;


        public ProductController(ProductContext product, ProductServices services)
            {
                this._product = product;
            this._productServices = services;
            }
        // Controller action
        public IActionResult Index(int pageIndex = 1, int pageSize = 5, string sortOrder = "date_desc", string searchQuery = null)
        {
            ///This variable will hold the paginated products that will be displayed on the view.
            PaginatedList<Product> paginatedProducts;

            if (!string.IsNullOrEmpty(searchQuery))
            {
                // Set the current filter for maintaining the search query
                ViewBag.CurrentFilter = searchQuery;
                paginatedProducts = _productServices.SearchProducts(searchQuery, pageIndex, pageSize);
            }
            else
            {
                var products = _productServices.GetPaginatedProducts(pageIndex, pageSize, sortOrder);
                paginatedProducts = new PaginatedList<Product>(products, _productServices.GetTotalProductCount(), pageIndex, pageSize);
            }

            return View(paginatedProducts);
        }


        /// <summary>
        /// Image  ConvertIFormFileToByteArray(IFormFile image)
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private byte[] ConvertIFormFileToByteArray(IFormFile image)
        {
            using (var memoryStream = new MemoryStream())
            {
                image.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
        public IActionResult GetImage(int id)
        {
            // Retrieve the image binary data from your database based on the given ID.
            byte[] imageData = _productServices.GetImageById(id);

            if (imageData != null)
            {
                // Determine the MIME type of the image (e.g., image/jpeg, image/png).
                string contentType = "image/jpeg"; // Adjust this based on your image type.

                // Return the image as a FileResult.
                return File(imageData, contentType);
            }
            else
            {
                // Handle a case where the image is not found or other appropriate action.
                return NotFound();
            }
        }


        // GET: ProductController/Delete/5
        public ActionResult Delete(int id)
        {
            // Fetch the product by id
            var product = _productServices.GetProductById(id);

            if (product == null)
            {
                return NotFound(); // Or any appropriate error handling
            }

            // Map the product to the view model
            var model = new ProductViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                ExpiryDate = product.ExpiryDate,
                Category = product.Category,
                Status = product.Status
            };

            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                _productServices.DeleteProduct(id);

                // Redirect to the product listing page (e.g., Index)
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
   
        public ActionResult Create(int? id)
        {
            ProductViewModel model;
            if (id != null)
            {
                // Fetch the product by id for editing
                var existingProduct = _productServices.GetProductById((int)id);
                if (existingProduct == null)
                {
                    return NotFound(); // Or any appropriate error handling
                }

                // Map the existing product to the view model
                model = new ProductViewModel
                {
                    ProductId = existingProduct.ProductId,
                    Code = existingProduct.Code,
                    Name = existingProduct.Name,
                    Description = existingProduct.Description,
                    ExpiryDate = existingProduct.ExpiryDate,
                    Category = existingProduct.Category,
                    // Assuming Image is not part of the initial edit page
                    Status = existingProduct.Status
                };
            }
            else
            {
                // Create a new product
                model = new ProductViewModel();
            }

            // Customize the model as needed before passing it to the view

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ProductId != 0)
                {
                    // Fetch the product by id for editing
                    var existingProduct = _productServices.GetProductById(model.ProductId);
                    if (existingProduct == null)
                    {
                        return NotFound(); // Or any appropriate error handling
                    }

                    // Map the view model data to the existing product
                    existingProduct.Code = model.Code;
                    existingProduct.Name = model.Name;
                    existingProduct.Description = model.Description;
                    existingProduct.ExpiryDate = model.ExpiryDate;
                    existingProduct.Category = model.Category;
                    existingProduct.Status = model.Status;

                    // Handle the Image data accordingly based on your implementation
                    if (model.Image != null)
                    {
                        existingProduct.Image = ConvertIFormFileToByteArray(model.Image);
                    }

                    // Call the UpdateProduct method from the repository to update the product in the database
                    _productServices.UpsertProduct(existingProduct);
                }
                else
                {
                    // Create a new product
                    var newProduct = new Product
                    {
                        Code = model.Code,
                        Name = model.Name,
                        Description = model.Description,
                        ExpiryDate = model.ExpiryDate,
                        Category = model.Category,
                        // Handle the Image data accordingly based on your implementation
                        Image = ConvertIFormFileToByteArray(model.Image),
                        Status = model.Status
                    };

                    // Call the AddProduct method from the repository to add the new product to the database
                    _productServices.UpsertProduct(newProduct);
                }

                // Redirect to the product listing page
                return RedirectToAction("Index");
            }

            return View(model);
        }

    }
}
