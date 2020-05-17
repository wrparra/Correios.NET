var target = Argument("target", "Default");
var projectName = "Correios.NET";
var solutionName = "Correios.NET";
var frameworks = new Dictionary<String, String>
{
    { "netstandard2.0", "netstandard2.0" },
};

#load tools/correios_net.cake

RunTarget(target);