namespace APN.RecruitmentTask.Domain.Book;

public class Book: EntityBase<int>
{
    public required string Title { get; set; }
    public required decimal Price { get; set; }
    public required int BookStand { get; set; }
    public required int Shelf { get; set; }
    public required IEnumerable<BookAuthor> Authors { get; set; }
}