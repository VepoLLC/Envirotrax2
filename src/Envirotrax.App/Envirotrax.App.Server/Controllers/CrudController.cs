
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers
{
    public abstract class CrudController<TDto> : CrudController<TDto, int>
        where TDto : class, IDto<int>
    {
        protected CrudController(IServiceBase<TDto, int> service)
            : base(service)
        {
        }
    }

    public abstract class CrudController<TDto, TKey> : ProtectedController
        where TDto : class, IDto<TKey>
    {
        private readonly IServiceBase<TDto, TKey> _service;

        public CrudController(IServiceBase<TDto, TKey> service)
        {
            _service = service;
        }

        protected virtual async Task<IPagedData<TDto>> ProcessGetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync(pageInfo, query, cancellationToken);
        }

        [HttpGet]
        public virtual async Task<IActionResult> GetAllAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
        {
            var dtoList = await ProcessGetAllAsync(pageInfo, query, cancellationToken);
            return Ok(dtoList);
        }

        protected virtual async Task<TDto?> ProcessGetAsync(TKey id, CancellationToken cancellationToken)
        {
            return await _service.GetAsync(id, cancellationToken);
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetAsync(TKey id, CancellationToken cancellationToken)
        {
            var dto = await ProcessGetAsync(id, cancellationToken);

            if (dto != null)
            {
                return Ok(dto);
            }

            return NotFound();
        }

        protected virtual async Task<TDto> ProcessAddAsync(TDto dto)
        {
            return await _service.AddAsync(dto);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AddAsync(TDto dto)
        {
            var added = await ProcessAddAsync(dto);

            if (added != null)
            {
                return Ok(added);
            }

            return Conflict();
        }

        protected virtual async Task<TDto> ProcessUpdateAsync(TDto dto)
        {
            return await _service.UpdateAsync(dto);
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> UpdateAsync(TKey id, TDto dto)
        {
            if (dto?.Id?.Equals(id) == false)
            {
                return BadRequest();
            }

            var updated = await ProcessUpdateAsync(dto!);

            if (updated != null)
            {
                return Ok(updated);
            }

            return Conflict();
        }

        protected virtual async Task<TDto?> ProcessDeleteAsync(TKey id)
        {
            return await _service.DeleteAsync(id);
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> DeleteAsync(TKey id)
        {
            var deleted = await ProcessDeleteAsync(id);

            if (deleted != null)
            {
                return Ok(deleted);
            }

            return NotFound();
        }

        protected virtual async Task<TDto?> ProcessReactivateAsync(TKey id)
        {
            return await _service.ReactivateAsync(id);
        }

        [HttpDelete("{id}/reactivate")]
        public virtual async Task<IActionResult> ReactivateAsync(TKey id)
        {
            var reactivated = await ProcessReactivateAsync(id);

            if (reactivated != null)
            {
                return Ok(reactivated);
            }

            return NotFound();
        }
    }
}