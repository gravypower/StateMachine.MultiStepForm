using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Routing;

namespace StateMachine.MultiStepForm
{
    public class RequireRequestModelTypeAttribute : ActionMethodSelectorAttribute
    {
        private readonly Type _modelParameterType;

        public RequireRequestModelTypeAttribute(Type modelParameterType = null)
        {
            _modelParameterType = modelParameterType;
        }

        public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
        {
            if (!routeContext.HttpContext.Request.HasFormContentType) return false;

            var modelType = action.Parameters.SingleOrDefault(p => p.ParameterType == _modelParameterType);
            var modelProperties = modelType?.ParameterType.GetProperties();
            if (modelProperties == null) return false;

            var isModel = false;
            var form = routeContext.HttpContext.Request.Form;
            foreach (var modelProperty in modelProperties)
            {
                isModel = form.ContainsKey(modelProperty.Name);
            }

            return isModel;
        }
    }
}
