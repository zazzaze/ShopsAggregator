using System;
using System.Collections.Generic;

namespace ShopsAggregator.Models
{
    public class IconPostForm
    {
        public List<Int32> IconBytesArr { get; set; }
        public Int32 ToId { get; set; }
        
        public IconPostForm()
        {
            
        }

        public IconPostForm(Int32 toId)
        {
            this.ToId = toId;
        }
    }
}