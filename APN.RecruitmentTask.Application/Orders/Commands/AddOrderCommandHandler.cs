using APN.RecruitmentTask.Application.Orders.Commands.Validators;
using APN.RecruitmentTask.Application.Services;
using APN.RecruitmentTask.Domain.Order;
using ErrorOr;
using MediatR;

namespace APN.RecruitmentTask.Application.Orders.Commands;

public class AddOrderCommandHandler(
    IOrdersRepository ordersRepository,
    IBooksRepository booksRepository,
    AddOrderCommandValidator commandValidator) : IRequestHandler<AddOrderCommand, ErrorOr<Guid>>
{
    public async Task<ErrorOr<Guid>> Handle(AddOrderCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await commandValidator.ValidateAsync(request, cancellationToken);
        if (validationResult.IsValid is false)
        {
            return Error.Validation(
                code: $"{nameof(AddOrderCommandHandler)}.ValidationFailed",
                description: string.Join(" ", validationResult.Errors.Select(x => x.ErrorMessage))
            );
        }
        
        var booksIds = request.OrderLines
            .Select(x => x.BookId)
            .Distinct()
            .ToArray();

        var books = (await booksRepository.GetBooksByIds(booksIds, cancellationToken))
            .ToArray();
        
        if (booksIds.Length != books.Length)
        {
            return Error.Validation(
                code: $"{nameof(AddOrderCommandHandler)}.BooksNotFound",
                description: "Some books were not found"
            );
        }
        
        var orderId = Guid.NewGuid();

        Order order = new()
        {
            Id = orderId,
            OrderLines = request.OrderLines
        };

        await ordersRepository.AddOrder(order, cancellationToken);

        return orderId;
    }
}