using PluginLib;

namespace ClassLibrary2;

public class PluginDemo2 : IHelloWorldPlugin
{
    public string Execute()
    {
        return "HelloWorld3";
    }
}