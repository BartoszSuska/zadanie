namespace Zadanie.Models {
    public class Contact {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public DateOnly BirthDate { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; } = null!;
        public int? SubcategoryId { get; set; }
        public Subcategory? Subcategory { get; set; }
        public string? CustomSubcategory { get; set; }
    }
}
