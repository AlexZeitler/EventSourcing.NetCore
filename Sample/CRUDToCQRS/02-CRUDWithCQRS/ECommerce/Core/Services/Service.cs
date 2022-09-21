using AutoMapper;
using ECommerce.Core.Repositories;
using ECommerce.Core.Requests;

namespace ECommerce.Core.Services;

public class Service<TEntity>: IService
    where TEntity : class, IEntity, new()
{
    private readonly IRepository<TEntity> repository;
    private readonly IMapper mapper;

    public Service(IRepository<TEntity> repository, IMapper mapper)
    {
        this.repository = repository;
        this.mapper = mapper;
    }

    public Task CreateAsync<TCreateRequest, TCreateResponse>(TCreateRequest request,
        CancellationToken ct)
    {
        var entity = mapper.Map<TCreateRequest, TEntity>(request);

        repository.Add(entity);

        return repository.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync<TUpdateRequest, TUpdateResponse>(
        TUpdateRequest request,
        CancellationToken ct
    ) where TUpdateRequest : IUpdateRequest
    {
        var entity = await GetEntityByIdAsync(request.Id, ct);

        entity = mapper.Map(request, entity);

        repository.Update(entity);

        await repository.SaveChangesAsync(ct);
    }

    public async Task DeleteByIdAsync(Guid id, CancellationToken ct)
    {
        var entity = await GetEntityByIdAsync(id, ct);

        repository.Delete(entity);

        await repository.SaveChangesAsync(ct);
    }

    protected async Task<TEntity> GetEntityByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await repository.FindByIdAsync(id, ct);

        if (result == null)
            throw new ArgumentException($"{typeof(TEntity).Name} with id '{id}' was not found");

        return result;
    }
}