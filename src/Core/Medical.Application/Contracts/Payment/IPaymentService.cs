using Medical.Shared.Cart;
using Microsoft.AspNetCore.Http;

namespace Medical.Application.Contracts.Payment
{
    public interface IPaymentService
    {
        Task<IResponse> CreateCheckoutSession(List<CartProductResponse> products);
        Task<IResponse> FulfillOrder(HttpRequest request);
    }
}
