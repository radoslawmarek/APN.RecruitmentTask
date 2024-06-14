namespace APN.RecruitmentTask.Contracts.ApiContracts.Books;

public record CreateBookRequest(
    string Title,
    decimal Price,
    int Bookstand,
    int Shelf,
    IReadOnlyCollection<BookAuthorRequest> Authors
    );