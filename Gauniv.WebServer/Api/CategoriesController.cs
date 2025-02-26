using AutoMapper;
using Gauniv.WebServer.Data;
using Gauniv.WebServer.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gauniv.WebServer.Api
{
    [Route("api/1.0.0/[controller]/[action]")]
    [ApiController]
    public class CategoriesController(ApplicationDbContext appDbContext, IMapper mapper) : ControllerBase
    {
        private readonly ApplicationDbContext _appDbContext = appDbContext;
        private readonly IMapper _mapper = mapper;

        // GET: api/1.0.0/Categories/List
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> List()
        {
            var categories = await _appDbContext.Categories.ToListAsync();
            var categoryDtos = _mapper.Map<List<CategoryDto>>(categories);
            return Ok(categoryDtos);
        }

        // GET: api/1.0.0/Categories/Get/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            var category = await _appDbContext.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            var categoryDto = _mapper.Map<CategoryDto>(category);
            return Ok(categoryDto);
        }

        // POST: api/1.0.0/Categories/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CategoryDto dto)
        {
            var category = _mapper.Map<Category>(dto);
            _appDbContext.Categories.Add(category);
            await _appDbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = category.Id }, _mapper.Map<CategoryDto>(category));
        }

        // PUT: api/1.0.0/Categories/Update/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryDto dto)
        {
            var category = await _appDbContext.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            _mapper.Map(dto, category);
            await _appDbContext.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/1.0.0/Categories/Delete/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _appDbContext.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            _appDbContext.Categories.Remove(category);
            await _appDbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
