using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace StateMachine.MultiStepForm.Contexts
{
    public class StateContext
    {
        private const string StateModelsKey = "stateModels";
        private const string StateKey = "stateToken";
        private readonly IHttpContextAccessor _accessor;

        private IDictionary<string, object> StateModels
        {
            get
            {
                if (!_accessor.HttpContext.Items.ContainsKey(StateModelsKey))
                {
                    _accessor.HttpContext.Items.Add(StateModelsKey, new Dictionary<string, object>());
                }

                return _accessor.HttpContext.Items[StateModelsKey] as Dictionary<string, object>;
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

        public string GetStateToken(State state)
        {
            var stateTokenSessionKey = $"{StateKey}-{state.Key}";
            if (_accessor.HttpContext.Session.Keys.Contains(stateTokenSessionKey))
            {
                return _accessor.HttpContext.Session.GetString(stateTokenSessionKey);
            }

            var stateToken = TokenGenerator.GetUniqueToken(64);

            _accessor.HttpContext.Session.SetString(stateToken, state.Key);
            _accessor.HttpContext.Session.SetString(stateTokenSessionKey, stateToken);

            return stateToken;
        }

        public string GetStateKeyFromToken(string token)
            => _accessor.HttpContext.Session.Keys.Contains(token)
                ? _accessor.HttpContext.Session.GetString(token)
                : throw new Exception("State Token was not present in session");

    }
}