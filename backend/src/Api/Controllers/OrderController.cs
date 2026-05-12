using MediatR;
using Medicare.Application.Commands;
using Medicare.Application.DTOs;
using Medicare.Application.Queries;
using Medicare.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Medicare.Api.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _mediator.Send(new CreateOrderCommand(request), cancellationToken);
                Response.Headers["X-Order-Id"] = result.Id.ToString();
                return CreatedAtAction(nameof(GetOrderById), new { id = result.Id }, result);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new { success = false, message = exception.Message });
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new { success = false, message = exception.Message });
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<OrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PagedResult<OrderDto>>> GetOrders(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] OrderStatus? status = null,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null,
            [FromQuery] MealType? mealType = null,
            [FromQuery] string? customerName = null,
            CancellationToken cancellationToken = default)
        {
            var forbiddenResult = EnsureAdminAccess();
            if (forbiddenResult is not null)
            {
                return forbiddenResult;
            }

            try
            {
                var query = new GetOrdersQuery(new OrderFilterRequest
                {
                    Page = page,
                    PageSize = pageSize,
                    Status = status,
                    DateFrom = dateFrom,
                    DateTo = dateTo,
                    MealType = mealType,
                    CustomerName = customerName,
                });

                var result = await _mediator.Send(query, cancellationToken);
                return Ok(result);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new { success = false, message = exception.Message });
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDto>> GetOrderById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var forbiddenResult = EnsureAdminAccess();
            if (forbiddenResult is not null)
            {
                return forbiddenResult;
            }

            try
            {
                var result = await _mediator.Send(new GetOrderByIdQuery(id), cancellationToken);
                return Ok(result);
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(new { success = false, message = exception.Message });
            }
        }

        [HttpPatch("{id:int}/status")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDto>> UpdateOrderStatus([FromRoute] int id, [FromBody] UpdateOrderStatusRequest request, CancellationToken cancellationToken)
        {
            var forbiddenResult = EnsureAdminAccess();
            if (forbiddenResult is not null)
            {
                return forbiddenResult;
            }

            try
            {
                var result = await _mediator.Send(new UpdateOrderStatusCommand(id, request), cancellationToken);
                return Ok(result);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new { success = false, message = exception.Message });
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new { success = false, message = exception.Message });
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(new { success = false, message = exception.Message });
            }
        }

        private ObjectResult? EnsureAdminAccess()
        {
            var role = HttpContext.Items["AuthenticatedRole"]?.ToString();
            if (string.Equals(role, UserRole.Admin.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return StatusCode(StatusCodes.Status403Forbidden, new
            {
                success = false,
                message = "Forbidden",
            });
        }
    }
}