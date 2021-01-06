using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Tpk.DataServices.Client.Components.Base
{
    public class LoadingProcess : ComponentBase
    {
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            var seq = 0;

            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "class", "w-100 text-center mt-5");

            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "class", "mt-md-5");
            builder.CloseElement();

            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "class", "mdi mdi-selection-ellipse mdi-48px mdi-spin");
            builder.CloseElement();

            builder.OpenElement(++seq, "div");
            builder.AddContent(++seq, "Loading...");
            builder.CloseElement();
            builder.CloseElement();
        }
    }
}