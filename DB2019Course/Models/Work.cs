//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DB2019Course.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Work
    {
        public int Id { get; set; }
        public int AuthorPass { get; set; }
        public string Name { get; set; }
        public string Theme { get; set; }
        public string Department { get; set; }
        public string UDC { get; set; }
    
        public virtual Article Article { get; set; }
        public virtual Aspirant Aspirant { get; set; }
        public virtual Disser Disser { get; set; }
    }
}
