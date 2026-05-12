using MediatR;
using Medicare.Application.Commands;
using Medicare.Application.DTOs;
using Medicare.Application.Queries;
using Medicare.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Medicare.Api.Controllers
{
    [ApiController]
    [Route("api/menus")]
    public class MenuController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MenuController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(MenuDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<MenuDto>> CreateMenu([FromBody] CreateMenuRequest request, CancellationToken cancellationToken)
        {
            var forbiddenResult = EnsureAdminAccess();
            if (forbiddenResult is not null)
            {
                return forbiddenResult;
            }

            try
            {
                var result = await _mediator.Send(new CreateMenuCommand(request), cancellationToken);
                return CreatedAtAction(nameof(GetMenuById), new { id = result.Id }, result);
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
        [ProducesResponseType(typeof(PagedResult<MenuDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PagedResult<MenuDto>>> GetMenus(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null,
            [FromQuery] MealType? mealType = null,
            CancellationToken cancellationToken = default)
        {
            var forbiddenResult = EnsureAdminAccess();
            if (forbiddenResult is not null)
            {
                return forbiddenResult;
            }

            try
            {
                var query = new GetMenusQuery(new MenuFilterRequest
                {
                    Page = page,
                    PageSize = pageSize,
                    DateFrom = dateFrom,
                    DateTo = dateTo,
                    MealType = mealType,
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
        [ProducesResponseType(typeof(MenuDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MenuDto>> GetMenuById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var forbiddenResult = EnsureAdminAccess();
            if (forbiddenResult is not null)
            {
                return forbiddenResult;
            }

            try
            {
                var result = await _mediator.Send(new GetMenuByIdQuery(id), cancellationToken);
                return Ok(result);
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(new { success = false, message = exception.Message });
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(MenuDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MenuDto>> UpdateMenu([FromRoute] int id, [FromBody] UpdateMenuRequest request, CancellationToken cancellationToken)
        {
            var forbiddenResult = EnsureAdminAccess();
            if (forbiddenResult is not null)
            {
                return forbiddenResult;
            }

            try
            {
                var result = await _mediator.Send(new UpdateMenuCommand(id, request), cancellationToken);
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

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMenu([FromRoute] int id, CancellationToken cancellationToken)
        {
            var forbiddenResult = EnsureAdminAccess();
            if (forbiddenResult is not null)
            {
                return forbiddenResult;
            }

            try
            {
                await _mediator.Send(new DeleteMenuCommand(id), cancellationToken);
                return NoContent();
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
