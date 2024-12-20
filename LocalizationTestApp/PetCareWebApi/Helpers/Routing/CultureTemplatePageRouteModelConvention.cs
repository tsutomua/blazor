using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace TestLocalization.Helpers.Routing;

public class CultureTemplatePageRouteModelConvention: IPageRouteModelConvention
{
    public void Apply(PageRouteModel pageRouteModel)
    {
        foreach (SelectorModel selectorModel in pageRouteModel.Selectors)
        {
            string template = selectorModel.AttributeRouteModel.Template;

            if (template.StartsWith("MicrosoftIdentity")) continue;  // Skip MicrosoftIdentity pages

            selectorModel.AttributeRouteModel.Order = -1;

            // Prepend the /{culture?}/ route value to allow for route-based localization
            //selectorModel.AttributeRouteModel.Template = AttributeRouteModel.CombineTemplates("{culture?}", template);
            selectorModel.AttributeRouteModel.Template = AttributeRouteModel.CombineTemplates("{culture:cultureConstraint}", template);
        }
    }
}
