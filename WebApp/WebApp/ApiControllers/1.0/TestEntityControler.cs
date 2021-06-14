using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.App.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PublicApi.DTO.v1.DTO.Items;
using PublicApi.DTO.v1.Mappers;

namespace WebApp.ApiControllers._1._0
{
    /// <inheritdoc />
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class TestEntityController : ControllerBase
        {
            private readonly AppDbContext _context;
            private readonly AppUnitOfWork _uow;
            private readonly TestEntityDTOMapper _mapper;


            /// <inheritdoc />
            public TestEntityController(AppDbContext context)
            {
                _context = context;
                _uow = new AppUnitOfWork(context);
                _mapper = new TestEntityDTOMapper();
            }

            // GET: api/TestEntity
            /// <summary>
            /// Get all TestEntities
            /// </summary>
            /// <returns></returns>
            [HttpGet]
            public async Task<IEnumerable<TestEntityDTO>> GetAll()
            {
                var records = await _uow.TestEntityRepository.GetAllAsync();
                return records.Select(e => _mapper.Map(e));
            }

            // GET: api/TestEntity/5
            /// <summary>
            /// Get single TestEntity
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            [HttpGet("{id}")]
            public async Task<IActionResult> GetById(Guid id)
            {
                if (!await _uow.TestEntityRepository.ExistsAsync(id))
                {
                    return NotFound();
                }
                var record = await _uow.TestEntityRepository.FirstOrDefaultAsync(id);


                return Ok(_mapper.Map(record));
            }

            // PUT: api/TestEntity/5
            // To protect from overposting attacks, please enable the specific properties you want to bind to, for
            // more details see https://aka.ms/RazorPagesCRUD.
            /// <summary>
            /// Put TestEntity
            /// </summary>
            /// <param name="id"></param>
            /// <param name="TestEntity"></param>
            /// <returns></returns>
            [HttpPut("{id}")]
            public async Task<IActionResult> PutById(Guid id, TestEntityDTO testEntity)
            {
                if (id != testEntity.Id)
                {
                    return BadRequest();
                }

                var record = await _uow.TestEntityRepository.FirstOrDefaultAsync(id);

                // Do update
                // genre.Name = genreEditDTO.Name;

                await _uow.TestEntityRepository.UpdateAsync(record);


                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                }

                return NoContent();
            }

            // POST: api/TestEntity
            // To protect from overposting attacks, please enable the specific properties you want to bind to, for
            // more details see https://aka.ms/RazorPagesCRUD.
            /// <summary>
            /// Post TestEntity
            /// </summary>
            /// <param name="testEntity"></param>
            /// <returns></returns>
            [HttpPost]
            public async Task<ActionResult<TestEntityDTO>> Post(TestEntityDTO testEntity)
            {
                var record = _uow.TestEntityRepository.Add(_mapper.Map(testEntity));
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetById", new {id = record.Id}, record);
            }

            // DELETE: api/TestEntity/5
            /// <summary>
            /// Delete TestEntity
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteById(Guid id)
            {
                if (!await _uow.TestEntityRepository.ExistsAsync(id))
                {
                    return NotFound();
                }
                var record = await _uow.TestEntityRepository.FirstOrDefaultAsync(id);

                await _uow.TestEntityRepository.RemoveAsync(record);
                await _context.SaveChangesAsync();

                return StatusCode(200,_mapper.Map(record));
            }
        }
    
}