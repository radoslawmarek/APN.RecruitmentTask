using APN.RecruitmentTask.Application.Books.Commands;
using APN.RecruitmentTask.Application.Books.Queries;
using APN.RecruitmentTask.Contracts.ApiContracts.Books;
using APN.RecruitmentTask.Domain.Book;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APN.RecruitmentTask.Api.Endpoints;

public static class BooksEndpoints
{
    private static readonly string[] EndpointTags = new[] { "Books" };

    public static void AddBooksEndpoints(this WebApplication application)
    {
        application.MapGet("/api/books", [Authorize] async ([FromServices] IMediator mediator) =>
            {
                var result = await mediator.Send(new GetBooksQuery());
                result.Match(
                    bookList => Results.Ok(bookList),
                    errors => Results.BadRequest(string.Join(", ", errors.Select(e => e.Description)))
                );
            })
            .WithName("GetBooks")
            .WithDescription("Get list of books")
            .WithTags(EndpointTags)
            .Produces<IEnumerable<BookQueryResult>>();
        
        application.MapPost("/api/books", [Authorize] async ([FromServices] IMediator mediator, [FromBody] CreateBookRequest request) =>
            {
                var result = await mediator.Send(new AddBookCommand(request.Title, 
                    request.Price, 
                    request.Bookstand, 
                    request.Shelf, 
                    request.Authors.Select(a => new BookAuthor() {FirstName = a.FirstName, LastName = a.LastName}).ToList()));
                
                result.Match(
                    bookId => Results.Created($"/api/books/{bookId}", bookId),
                    errors => Results.BadRequest(string.Join(", ", errors.Select(e => e.Description)))
                );
            })
            .WithName("CreateBook")
            .WithDescription("Create a new book")
            .WithTags(EndpointTags)
            .Accepts<CreateBookRequest>(false, "application/json")
            .Produces<BookQueryResult>();
    }
}