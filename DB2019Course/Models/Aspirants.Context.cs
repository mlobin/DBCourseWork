﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Article> Article { get; set; }
        public virtual DbSet<Aspirant> Aspirant { get; set; }
        public virtual DbSet<Auth> Auth { get; set; }
        public virtual DbSet<Defence> Defence { get; set; }
        public virtual DbSet<Disser> Disser { get; set; }
        public virtual DbSet<Exam> Exam { get; set; }
        public virtual DbSet<Group> Group { get; set; }
        public virtual DbSet<IndPlan> IndPlan { get; set; }
        public virtual DbSet<Leader> Leader { get; set; }
        public virtual DbSet<Result> Result { get; set; }
        public virtual DbSet<Review> Review { get; set; }
        public virtual DbSet<Voting> Voting { get; set; }
        public virtual DbSet<Work> Work { get; set; }
        public virtual DbSet<Logged> Logged { get; set; }

        public virtual int JoinNames(string table, Nullable<int> num, ObjectParameter result)
        {
            var tableParameter = table != null ?
                new ObjectParameter("table", table) :
                new ObjectParameter("table", typeof(string));
    
            var numParameter = num.HasValue ?
                new ObjectParameter("num", num) :
                new ObjectParameter("num", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("JoinNames", tableParameter, numParameter, result);
        }
    }
}