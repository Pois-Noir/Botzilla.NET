namespace Botzilla.Server;

internal struct Component
{
    public string Name;
    public string[] Groups;
    public string Address;

    public Component(string name, string[] groups, string address)
    {
        Name = name;
        Groups = groups;
        Address = address;
    }
}


internal class Registry
{
    private Dictionary<string, Component> _components = new();
    private Mutex _mutex = new();
    
    
    public Registry()
    { }

    
    public bool TryGetComponent(string name, out Component component)
    {
        try
        {
            _mutex.WaitOne();
            
            component = default;
            return _components.TryGetValue(name, out component);
        }
        finally
        {
            _mutex.ReleaseMutex();
        }
    }
    
    
    public Component TryAddComponent(string name, string address)
    {
        try
        {
            _mutex.WaitOne();
            
            if (_components.ContainsKey(name))
            {
                throw new InvalidOperationException($"A component with the name \"{name}\" exists already.");
            }

            var asComponent = new Component(name, [], address);
            _components.Add(name, asComponent);
            return asComponent;
        }
        finally
        {
            _mutex.ReleaseMutex();
        }
    }

    
    public bool TryDeleteComponent(string name)
    {
        try
        {
            _mutex.WaitOne();
            return _components.Remove(name);
        }
        finally
        {
            _mutex.ReleaseMutex();
        }
    }
}