using MediatR;

namespace Backend.Features.Customers;


public class CustomerListQuery : IRequest<List<CustomerListQueryResponse>>
{
    public string? Name { get; set; }
    //filtro opzionale “SearchText” per cercare nei campi “Nome” ed “Email”.
    public string? SearchText { get; set; }
}

public class CustomerListQueryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Address { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Iban { get; set; } = "";
    public CustomerListQueryResponseCustomerCategory? CustomerCategory { get; set; }
}

public class CustomerListQueryResponseCustomerCategory
{
    public string Code { get; set; } = "";
    public string Description { get; set; } = "";
}

internal class CustomerListQueryHandler(BackendContext context) : IRequestHandler<CustomerListQuery, List<CustomerListQueryResponse>>
{
    private readonly BackendContext context = context;

    public async Task<List<CustomerListQueryResponse>> Handle(CustomerListQuery request, CancellationToken cancellationToken)
    {
            var query = context.Customers
            .Include(c => c.CustomerCategory)
            .AsQueryable();
        
        //Filtro per cercare nei campi “Nome” ed “Email”.
        if (!string.IsNullOrEmpty(request.SearchText))
        {
            var lowered = request.SearchText.ToLower();

            query = query.Where(q =>
                EF.Functions.Like(q.Name, $"%{lowered}%") ||
                EF.Functions.Like(q.Email, $"%{lowered}%")
            );
        }

        if (!string.IsNullOrEmpty(request.Name))
            query = query.Where(q => q.Name.ToLower().Contains(request.Name.ToLower()));



        var result = await query
            .OrderBy(q => q.Name)
            .Select(q => new CustomerListQueryResponse
            {
                Id = q.Id,
                Name = q.Name,
                Address = q.Address,
                Email = q.Email,
                Phone = q.Phone,
                Iban = q.Iban,
                CustomerCategory = q.CustomerCategory != null
                    ? new CustomerListQueryResponseCustomerCategory
                    {
                        Code = q.CustomerCategory.Code,
                        Description = q.CustomerCategory.Description
                    }
                    : null
            })
            .ToListAsync(cancellationToken);

        return result;
    }
}