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
        var query = context.Customers.AsQueryable();
        
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


        var data = await query.OrderBy(q => q.Name).ToListAsync(cancellationToken);
        var result = new List<CustomerListQueryResponse>();

        foreach (var item in data)
        {
            var resultItem = new CustomerListQueryResponse
            {
                Id = item.Id,
                Name = item.Name,
                Address = item.Address,
                Email = item.Email,
                Phone = item.Phone,
                Iban = item.Iban,
            };

            //record della tabella CustomerCategory
            var customerCategory = await context.CustomerCategories.SingleOrDefaultAsync(q => q.Id == item.CustomerCategoryId, cancellationToken);
            if (customerCategory is not null)
                resultItem.CustomerCategory = new CustomerListQueryResponseCustomerCategory
                {
                    Code = customerCategory.Code,
                    Description = customerCategory.Description
                };


            result.Add(resultItem);
        }

        return result;
    }
}