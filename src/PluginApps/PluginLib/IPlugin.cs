namespace PluginLib;

public interface IPlugin
{
    bool Execute();
}

public interface IHelloWorldPlugin
{
    string Execute();
}