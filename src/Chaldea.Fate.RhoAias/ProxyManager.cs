﻿namespace Chaldea.Fate.RhoAias;

public interface IProxyManager
{
    Task CreateProxyAsync(Proxy entity);
    Task UpdateProxyAsync(Proxy entity);
    Task RemoveProxyAsync(Guid id);
    Task<List<Proxy>> GetProxyListAsync();
    Task<int> CountProxyAsync();
    Task UpdateProxyListAsync(Guid clientId, List<Proxy> proxies);
    Task UpdateStatusAsync(Guid id, bool disabled);
}

internal class ProxyManager : IProxyManager
{
    private readonly IForwarderManager _forwarderManager;
    private readonly IRepository<Proxy> _proxyRepository;

    public ProxyManager(IRepository<Proxy> proxyRepository, IForwarderManager forwarderManager)
    {
        _proxyRepository = proxyRepository;
        _forwarderManager = forwarderManager;
    }

    public async Task CreateProxyAsync(Proxy entity)
    {
        if (await _proxyRepository.AnyAsync(x => x.Name == entity.Name))
        {
            return;
        }
        entity.EnsureId();
        entity.EnsureLocalIp();
        await _proxyRepository.InsertAsync(entity);
        // get with client
        var proxy = await _proxyRepository.GetAsync(x => x.Id == entity.Id, y => y.Client);
        _forwarderManager.Register(proxy);
    }

    public async Task UpdateProxyAsync(Proxy entity)
    {
        if (await _proxyRepository.AnyAsync(x => x.Name == entity.Name && x.Id != entity.Id)) return;
        var item = await _proxyRepository.GetAsync(x => x.Id == entity.Id, y => y.Client);
        if (item == null) return;
        // try to remove old config
        _forwarderManager.UnRegister(item);
        item.Update(entity);
        item.EnsureLocalIp();
        await _proxyRepository.UpdateAsync(item);
        // add new config
        _forwarderManager.Register(item);
    }

    public async Task RemoveProxyAsync(Guid id)
    {
        _forwarderManager.UnRegister(id);
        await _proxyRepository.DeleteAsync(x => x.Id == id);
    }

    public async Task<List<Proxy>> GetProxyListAsync()
    {
        var list = await _proxyRepository.GetListAsync(x => x.Client);
        return list;
    }

    public async Task<int> CountProxyAsync()
    {
        return await _proxyRepository.CountAsync();
    }

    public async Task UpdateProxyListAsync(Guid clientId, List<Proxy>? proxies)
    {
        // serverProxies is from server db.
        var serverProxies = await _proxyRepository.GetListAsync(x => x.ClientId == clientId, y => y.Client);
        var register = serverProxies;
        var client = serverProxies.FirstOrDefault()?.Client;
        if (proxies != null)
        {
            var update = new List<Proxy>();
            var insert = new List<Proxy>();
            // proxies is from client config.(eg: appsettings.json, or k8s-ingress)
            foreach (var proxy in proxies)
            {
                proxy.ClientId = clientId;
                proxy.Client = client;
                var exists = serverProxies.FirstOrDefault(x => x.Name == proxy.Name);
                if (exists != null)
                {
                    _forwarderManager.UnRegister(exists); // try to remove old config
                    exists.Update(proxy);
                    exists.EnsureLocalIp();
                    update.Add(exists); // update from client
                }
                else
                {
                    proxy.EnsureId();
                    proxy.EnsureLocalIp();
                    insert.Add(proxy); // add new from client
                }
            }

            await _proxyRepository.UpdateManyAsync(update);
            await _proxyRepository.InsertManyAsync(insert);
            register = serverProxies.Concat(insert).ToList();
        }

        if (register is { Count: > 0 }) _forwarderManager.Register(register);
    }

    public async Task UpdateStatusAsync(Guid id, bool disabled)
    {
        var entity = await _proxyRepository.GetAsync(x => x.Id == id, y => y.Client);
        if (entity == null) return;
        if (disabled != entity.Disabled)
        {
            entity.Disabled = disabled;
            await _proxyRepository.UpdateAsync(entity);
            if (disabled)
            {
                _forwarderManager.UnRegister(entity);
            }
            else
            {
                _forwarderManager.Register(entity);
            }
        }
    }
}