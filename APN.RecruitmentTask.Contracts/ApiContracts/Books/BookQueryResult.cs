namespace APN.RecruitmentTask.Contracts.ApiContracts.Books;

public record BookQueryResult(
    int Id,
    string Title,
    decimal Price,
    int Bookstand,
    int Shelf,
    IReadOnlyCollection<BookAuthorQueryResult> Authors
    );