using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProductId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column(TypeName = "nvarchar(50)")]
    public string Code { get; set; }

    [Required]
    [MaxLength(250)]
    [Column(TypeName = "nvarchar(250)")]
    public string Name { get; set; }

    [MaxLength(4000)]
    [Column(TypeName = "nvarchar(4000)")]
    public string Description { get; set; }

    [Required]
    [Column(TypeName = "date")]
    public DateTime ExpiryDate { get; set; }

    [Required]
    [MaxLength(50)]
    [Column(TypeName = "nvarchar(50)")]
    public string Category { get; set; }

    [Column(TypeName = "varbinary(max)")]
    public byte[] Image { get; set; }

    [Required]
    [Column(TypeName = "bit")]
    public bool Status { get; set; }

    [Required]
    [Column(TypeName = "datetime")]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreationDate { get; set; }
}
