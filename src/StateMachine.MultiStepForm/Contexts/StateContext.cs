using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace StateMachine.MultiStepForm.Contexts
{
    public class StateContext
    {
        private readonly IHttpContextAccessor _accessor;

        private IDictionary<string, object> StateModels
        {
            get
            {
                if (!_accessor.HttpContext.Items.ContainsKey("stateModels"))
                {
                    _accessor.HttpContext.Items.Add("stateModels", new Dictionary<string, object>());
                }

                return _accessor.HttpContext.Items["stateModels"] as Dictionary<string, object>;
            }
        }

        public StateContext(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public void SetModel(State state, object model)
        {
            StateModels.Add(state.Key, model);
        }

        public object GetModel(State state)
        {
            return StateModels.ContainsKey(state.Key) ? StateModels[state.Key] : null;
        }
    }
}
