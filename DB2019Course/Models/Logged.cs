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
    
    public partial class Logged
    {
        public string Session { get; set; }
        public string Login { get; set; }
    
        public virtual Auth Auth { get; set; }
    }
}