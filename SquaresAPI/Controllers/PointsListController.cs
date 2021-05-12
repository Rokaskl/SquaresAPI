using Microsoft.AspNetCore.Mvc;
using SquaresAPI.Exceptions;
using SquaresAPI.Models;
using SquaresAPI.Services;

namespace SquaresAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PointsListController : ControllerBase
    {
        private readonly IPointsListService _pointsListService;

        public PointsListController(IPointsListService pointsListService)
        {
            _pointsListService = pointsListService;
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var pointsList = _pointsListService.Get(id);
            if (pointsList == null) return NotFound();
            return Ok(pointsList);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_pointsListService.GetAll());
        }

        [HttpPost]
        public IActionResult Create(CreatePointsListRequest model)
        {
            return Ok(_pointsListService.Create(model));
        }

        [HttpGet("{id:int}/squares")]
        public IActionResult GetSquares(int id)
        {
            try
            {
                return Ok(_pointsListService.GetSquares(id));
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPost("removepoint")]
        public IActionResult RemovePoint(RemovePointFromListRequest model)
        {
            try
            {
                return Ok(_pointsListService.RemovePoint(model));
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpDelete]
        public IActionResult Delete(int pointsListId)
        {
            try
            {
                _pointsListService.DeletePointsList(pointsListId);
                return Ok();
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPost("addpoint")]
        public IActionResult AddPoint(AddPointToListRequest model)
        {
            try
            {
                return Ok(_pointsListService.AddPoint(model));
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (AlreadyExistsException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}