using System;
using System.Collections.Generic;
using System.Text;

namespace ImageToPDFConverter.Models
{
    public class ImagesModel
    {
        public string FileName { get; set; }
        public string Uri { get; set; }
        public string Image { get; set; }

        public int Page { get; set; }
    }
}
