using Avalonia.Web.Blazor;

namespace Toasters.Web;

public partial class App
{
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        WebAppBuilder.Configure<Toasters.App>()
            .SetupWithSingleViewLifetime();
    }
}