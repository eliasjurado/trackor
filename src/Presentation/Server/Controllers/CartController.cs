using MediatR;
using Medical.Application.Features.Cart.Commands.AddToCart;
using Medical.Application.Features.Cart.Commands.RemoveItemFromCart;
using Medical.Application.Features.Cart.Commands.StoreCartItems;
using Medical.Application.Features.Cart.Commands.UpdateQuantity;
using Medical.Application.Features.Cart.Query.GetCartItemsCount;
using Medical.Application.Features.Cart.Query.GetCartProducts;
using Medical.Application.Features.Cart.Query.GetDbCartProducts;
using Medical.Shared.Cart;
using Medical.Shared.Constant;
using Medical.Shared.Response.Abstract;
using Medical.Shared.Response.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace Medical.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CartController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("products")]
        public async Task<ActionResult<IResponse>> GetCartProducts(List<CartItemDto> cartItems)
        {
            var response = await _mediator.Send(new GetCartProductsQueryRequest(cartItems));
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<IResponse>> StoreCartItems(List<CartItemDto> cartItems)
        {
            var result = await _mediator.Send(new StoreCartItemsCommandRequest(cartItems));
            if (!result.Success)
            {
                return new DataResponse<List<CartProductResponse>>(new List<CartProductResponse>(), HttpStatusCodes.NotFound);
            }
            else
            {
                return Ok(await _mediator.Send(new GetDbCartProductsQueryRequest()));
            }

        }

        [HttpPost("add")]
        public async Task<ActionResult<IResponse>> AddToCart(CartItemDto cartItem)
        {
            var response = await _mediator.Send(new AddToCartCommandRequest(cartItem));
            return Ok(response);
        }

        [HttpPut("update-quantity")]
        public async Task<ActionResult<IResponse>> UpdateQuantity(CartItemDto cartItem)
        {
            var response = await _mediator.Send(new UpdateQuantityCommandRequest(cartItem));
            return Ok(response);
        }

        [HttpDelete("{productId}/{productTypeId}")]
        public async Task<ActionResult<IResponse>> RemoveItemFromCart(int productId, int productTypeId)
        {
            var response = await _mediator.Send(new RemoveItemFromCartCommandRequest(productId, productTypeId));
            return Ok(response);
        }

        [HttpGet("count")]
        public async Task<ActionResult<IResponse>> GetCartItemsCount()
        {
            var response = await _mediator.Send(new GetCartItemsCountQueryRequest());
            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<IResponse>> GetDbCartProducts()
        {
            var response = await _mediator.Send(new GetDbCartProductsQueryRequest());
            return Ok(response);
        }
    }
}
