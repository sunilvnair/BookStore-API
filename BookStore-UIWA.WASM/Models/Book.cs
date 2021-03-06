﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookStore_UIWA_WASM.Models
{
    public class Book
    {
		public int Id { get; set; }
		[Required]
		public string Title { get; set; }
		public int? Year { get; set; }
		[Required]
		public string ISBN { get; set; }
		
		[DisplayName("Summary")]
		[StringLength(2500)]
		public string Summary { get; set; }
		public string Image { get; set; }
		public decimal? Price { get; set; }
		[Required]

		public int AuthorId { get; set; }

		public virtual Author Author { get; set; }
	}
}