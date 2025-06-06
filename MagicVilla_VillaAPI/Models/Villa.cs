﻿using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models
{
    public class Villa
    {
        public int Id { get; set; }

		[Required]
        public string Name { get; set; }
		public string Details { get; set; }
		public double Rate { get; set; }
		public int SqFt { get; set; }
		public int Occupancy { get; set; }
		public string? ImgUrl { get; set; }
		public string? ImageLocalPath { get; set; }

		public string Amenity { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime UpdatedDate { get; set; }

	}
}
