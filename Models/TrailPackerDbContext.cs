using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebTrail.Models;

public partial class TrailPackerDbContext : DbContext
{

    public TrailPackerDbContext()
    {
    }

    public TrailPackerDbContext(DbContextOptions<TrailPackerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category_Product> Category_Products { get; set; }

    public virtual DbSet<Category_Recipe> Category_Recipes { get; set; }

    public virtual DbSet<Hike> Hikes { get; set; }

    public virtual DbSet<Hike_Food_Plan> Hike_Food_Plans { get; set; }

    public virtual DbSet<Hike_Recipe_Suggestion> Hike_Recipe_Suggestions { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Product_Category_A> Product_Category_As { get; set; }

    public virtual DbSet<Recipe> Recipes { get; set; }

    public virtual DbSet<Recipe_Category_A> Recipe_Category_As { get; set; }

    public virtual DbSet<Recipe_Ingredient> Recipe_Ingredients { get; set; }

    public virtual DbSet<TourType> TourTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-2250EVR\\SQLEXPRESS;Database=TrailPacker;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Cyrillic_General_100_CI_AI_KS_SC_UTF8");

        modelBuilder.Entity<Category_Product>(entity =>
        {
            entity.HasKey(e => e.Category_Product_ID).HasName("PK__Category__B96A8F7FC5E62AD7");

            entity.ToTable("Category_Product");

            entity.Property(e => e.Category_Product_Name)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Category_Recipe>(entity =>
        {
            entity.HasKey(e => e.Category_Recipe_ID).HasName("PK__Category__012D31C510147EB5");

            entity.ToTable("Category_Recipe");

            entity.Property(e => e.Category_Recipe_Name)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Hike>(entity =>
        {
            entity.HasKey(e => e.Hike_ID).HasName("PK__Hike__94CE0B11ABD12F58");

            entity.ToTable("Hike");

            entity.Property(e => e.Generated_At).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.TourType).WithMany(p => p.Hikes)
                .HasForeignKey(d => d.TourTypeID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Hike_TourType");
        });

        modelBuilder.Entity<Hike_Food_Plan>(entity =>
        {
            entity.HasKey(e => e.Hike_Food_Plan_ID).HasName("PK__Hike_Foo__CAC3CC51B8D07A33");

            entity.ToTable("Hike_Food_Plan");

            entity.Property(e => e.Quantity).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Unit)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Hike).WithMany(p => p.Hike_Food_Plans)
                .HasForeignKey(d => d.Hike_ID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Hike_Food__Hike___403A8C7D");

            entity.HasOne(d => d.Product).WithMany(p => p.Hike_Food_Plans)
                .HasForeignKey(d => d.Product_ID)
                .HasConstraintName("FK__Hike_Food__Produ__412EB0B6");
        });

        modelBuilder.Entity<Hike_Recipe_Suggestion>(entity =>
        {
            entity.HasKey(e => e.Hike_Recipe_Suggestions_ID).HasName("PK__Hike_Rec__49328A6E564D1E14");

            entity.Property(e => e.Meal_Type)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Hike).WithMany(p => p.Hike_Recipe_Suggestions)
                .HasForeignKey(d => d.Hike_ID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Hike_Reci__Hike___440B1D61");

            entity.HasOne(d => d.Recipe).WithMany(p => p.Hike_Recipe_Suggestions)
                .HasForeignKey(d => d.Recipe_ID)
                .HasConstraintName("FK__Hike_Reci__Recip__44FF419A");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Product_ID).HasName("PK__Product__9834FB9A9C3C7768");

            entity.ToTable("Product");

            entity.Property(e => e.Calories_Per100g).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Carbs_Per100g).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Contains_Eggs).HasDefaultValue(false);
            entity.Property(e => e.Contains_Gluten).HasDefaultValue(false);
            entity.Property(e => e.Contains_Lactose).HasDefaultValue(false);
            entity.Property(e => e.Contains_Nuts).HasDefaultValue(false);
            entity.Property(e => e.Fat_Per100g).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Is_Vegetarian).HasDefaultValue(false);
            entity.Property(e => e.Product_Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Protein_Per100g).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Product_Category_A>(entity =>
        {
            entity.HasKey(e => e.Product_Category_As_ID);

            entity.HasOne(d => d.Category_Product).WithMany(p => p.Product_Category_As)
                .HasForeignKey(d => d.Category_Product_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Category_As_Category_Product");

            entity.HasOne(d => d.Product).WithMany(p => p.Product_Category_As)
                .HasForeignKey(d => d.Product_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Category_As_Product");
        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(e => e.Recipe_ID).HasName("PK__Recipe__0959CE394B9B6E15");

            entity.ToTable("Recipe");

            entity.Property(e => e.Contains_Eggs).HasDefaultValue(false);
            entity.Property(e => e.Contains_Nuts).HasDefaultValue(false);
            entity.Property(e => e.Is_Gluten_Free).HasDefaultValue(false);
            entity.Property(e => e.Is_Lactose_Free).HasDefaultValue(false);
            entity.Property(e => e.Is_Vegetarian).HasDefaultValue(false);
            entity.Property(e => e.Recipe_Name)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Recipe_Category_A>(entity =>
        {
            entity.HasKey(e => e.Recipe_Category_As_ID);

            entity.HasOne(d => d.Category_Recipe).WithMany(p => p.Recipe_Category_As)
                .HasForeignKey(d => d.Category_Recipe_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Recipe_Category_As_Category_Recipe");

            entity.HasOne(d => d.Recipe).WithMany(p => p.Recipe_Category_As)
                .HasForeignKey(d => d.Recipe_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Recipe_Category_As_Recipe");
        });

        modelBuilder.Entity<Recipe_Ingredient>(entity =>
        {
            entity.HasKey(e => e.Recipe_Ingredient_ID).HasName("PK__Recipe_I__CC286E07CFBD46C3");

            entity.ToTable("Recipe_Ingredient");

            entity.Property(e => e.Quantity).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Unit)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Product).WithMany(p => p.Recipe_Ingredients)
                .HasForeignKey(d => d.Product_ID)
                .HasConstraintName("FK__Recipe_In__Produ__3D5E1FD2");

            entity.HasOne(d => d.Recipe).WithMany(p => p.Recipe_Ingredients)
                .HasForeignKey(d => d.Recipe_ID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Recipe_In__Recip__3C69FB99");
        });

        modelBuilder.Entity<TourType>(entity =>
        {
            entity.HasKey(e => e.TourTypeID).HasName("PK__TourType__331F139BD254E9A1");

            entity.ToTable("TourType");

            entity.Property(e => e.TypeName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
        base.OnModelCreating(modelBuilder); //
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
