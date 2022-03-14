using System;
using System.Collections.Generic;

namespace FileTracking.Models
{
    public partial class Master
    {
        public int Id { get; set; }
        public DateTime Uploaddate { get; set; }
        public string Barcode { get; set; } = null!;
        public string Filename { get; set; } = null!;
        public string Department { get; set; } = null!;
        public string? Comment { get; set; }
    }
}
