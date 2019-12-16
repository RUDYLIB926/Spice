﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spice.Models
{
    public class MenuItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public enum ESpicy 
        { 
            NA = 0, 
            NotSpicy = 1,
            Spicy = 2,
            VerySpicy = 3,
        }

        public string Spicyness { get; set; }

        public string Image { get; set; }

        [Display(Name="Category Name")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [Display(Name = "Sub Category Name")]
        public int SubCategoryId { get; set; }

        [ForeignKey("SubCategoryId")]
        public virtual SubCategory SubCategory { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Price should be greater than ${1}")]
        public double Price { get; set; }
    }
}
