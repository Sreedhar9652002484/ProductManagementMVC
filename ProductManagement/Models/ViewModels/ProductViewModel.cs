using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCProject.ViewModels
{
    public class ProductViewModel
    {
        public int ProductId { get; set; } // Add this property

        [Required(ErrorMessage = "Code is required")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(250, ErrorMessage = "Name cannot exceed 250 characters")]
        public string Name { get; set; }

        [MaxLength(4000, ErrorMessage = "Description cannot exceed 4000 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Expiry Date is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ExpiryDate { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; }

        [Display(Name = "Image")]
        [Required(ErrorMessage = "Image is required")]

        public IFormFile Image { get; set; }

        [Required(ErrorMessage = "Status is required")]

        public bool Status { get; set; }
    }
}
