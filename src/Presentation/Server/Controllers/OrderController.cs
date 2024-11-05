using MediatR;
using Medical.Application.Features.Order.Query.GetOrder;
using Medical.Application.Features.Order.Query.GetOrderDetails;
using Medical.Shared.Response.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Medical.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IResponse>> GetOrders()
        {
            var response = await _mediator.Send(new GetOrderQueryRequest());
            return Ok(response);
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<IResponse>> GetOrdersDetails(int orderId)
        {
            var response = await _mediator.Send(new GetOrderDeatilsQueryRequest(orderId));
            return Ok(response);
        }
    }
}
