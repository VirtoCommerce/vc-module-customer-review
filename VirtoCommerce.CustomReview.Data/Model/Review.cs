using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomReview.Data.Model
{
    /// <summary>
    /// Model review
    /// </summary>
    public class Review : AuditableEntity
    {
        public string Author { get; set; }
        public string Text { get; set; }
        //ToDo enum
        public int Rate { get; set; }

        public bool Approved { get; set; }
        public string ProductId { get; set; }

    }
}
