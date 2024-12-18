﻿using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace TestLocalization.Helpers.Routing;

public class CultureTemplatePageRouteModelConvention: IPageRouteModelConvention
{
    public void Apply(PageRouteModel model)
    {
        foreach (SelectorModel selector in model.Selectors)
        {
            string template = selector.AttributeRouteModel.Template;

            if (template.StartsWith("MicrosoftIdentity")) continue;  // Skip MicrosoftIdentity pages

            // Prepend {culture}/ to the page routes allow for route-based localization
            selector.AttributeRouteModel.Order = -1;
            //selector.AttributeRouteModel.Template = AttributeRouteModel.CombineTemplates("{culture:cultureConstraint}", template);

            // Prepend the /{culture?}/ route value to allow for route-based localization
            selector.AttributeRouteModel.Template = AttributeRouteModel.CombineTemplates("{culture?}", template);
        }
    }
}
