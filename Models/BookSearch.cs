using Microsoft.AspNetCore.Mvc.Rendering;

namespace Book_Store.Models
{
    public class BookSearch
    {
        public List<Book> Books { get; set; }
        public SelectList? Orderss { get; set; }
        public string? BooksSearch { get; set; }
    }
}
