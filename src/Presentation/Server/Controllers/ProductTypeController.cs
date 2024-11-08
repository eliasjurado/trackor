﻿using MediatR;
using Medical.Application.Features.Category.Commands.UpdateCategory;
using Medical.Application.Features.Category.Query.GetCategories;
using Medical.Application.Features.ProductType.Command.AddProductType;
using Medical.Shared.ProductType;
using Medical.Shared.Response.Abstract;
using Medical.Shared.Response.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace Medical.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTypeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IResponse>> GetProductTypes()
        {
            var response = await _mediator.Send(new GetAllProductTypeQueryRequest());
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<IResponse>> AddProductType(ProductTypeDto productType)
        {
            await _mediator.Send(new AddProductTypeCommandRequest(productType));

            var response = await _mediator.Send(new GetAllProductTypeQueryRequest());
            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult<IResponse>> UpdateProductType(ProductTypeDto productType)
        {
            var result = await _mediator.Send(new UpdateProductTypeCommandRequest(productType));

            if (!result.Success)
            {
                var responseCast = (DataResponse<string>)result;

                return new DataResponse<List<ProductTypeDto>>(new List<ProductTypeDto>(), responseCast.StatusCode, responseCast.Messages.FirstOrDefault());
            }

            var response = await _mediator.Send(new GetAllProductTypeQueryRequest());
            return Ok(response);
        }
    }
}
