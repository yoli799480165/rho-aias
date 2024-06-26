﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chaldea.Fate.RhoAias.Dashboard;

public class ClientCreateDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = default!;
}

public class ClientDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Version { get; set; }
    public string? Token { get; set; }
    public string? Endpoint { get; set; }
    public string? ConnectionId { get; set; }
    public bool Status { get; set; }
}

[Authorize(Roles = Role.User)]
[ApiController]
[Route("api/dashboard/client")]
public class ClientController : ControllerBase
{
    private readonly IClientManager _clientManager;
    private readonly IMapper _mapper;

    public ClientController(IClientManager clientManager, IMapper mapper)
    {
        _clientManager = clientManager;
        _mapper = mapper;
    }

    [HttpPut]
    [Route("create")]
    public async Task CreateAsync(ClientCreateDto dto)
    {
        var entity = _mapper.Map<ClientCreateDto, Client>(dto);
        await _clientManager.CreateClientAsync(entity);
    }

    [HttpPost]
    [Route("update")]
    public async Task UpdateAsync(ClientCreateDto dto)
    {
        if (!dto.Id.HasValue) return;
        var entity = await _clientManager.GetClientAsync(dto.Id.Value);
        if (entity == null) return;
        var newEntity = _mapper.Map(dto, entity);
        await _clientManager.UpdateClientAsync(newEntity);
    }

    [HttpDelete]
    [Route("remove")]
    public async Task RemoveAsync(Guid id)
    {
        await _clientManager.RemoveClientAsync(id);
    }

    [HttpGet]
    [Route("list")]
    public async Task<ICollection<ClientDto>> GetListAsync()
    {
        var clients = await _clientManager.GetClientListAsync();
        return _mapper.Map<List<Client>, ICollection<ClientDto>>(clients);
    }
}