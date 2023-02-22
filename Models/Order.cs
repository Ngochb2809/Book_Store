namespace Book_Store.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int Qty { get; set; }
        public double Price { get; set; }
        public string Phone { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }

        public virtual Book? Book { get; set; }
        public virtual User? User { get; set; }
    }
}
