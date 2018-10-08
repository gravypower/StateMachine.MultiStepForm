using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace StateMachine.MultiStepForm.Contexts
{
    public class TriggerContext
    {
        private const string TriggerArgsKey = "triggerArgs";
        private readonly IHttpContextAccessor _accessor;
        private const string TriggerKey = "Trigger";
        
        private IDictionary<string, object> TriggerArgs
        {
            get
            {
                if (!_accessor.HttpContext.Items.ContainsKey(TriggerArgsKey))
                {
                    _accessor.HttpContext.Items.Add(TriggerArgsKey, new Dictionary<string, object>());
                }

                return _accessor.HttpContext.Items[TriggerArgsKey] as Dictionary<string, object>;
            }
        }

        public TriggerContext(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public TArg GetArg<TArg>(Trigger trigger)
        {
            if (!TriggerArgs.ContainsKey(trigger.Key))
                return default(TArg);

            if (!(TriggerArgs[trigger.Key] is TArg))
                return default(TArg);

            return (TArg) TriggerArgs[trigger.Key];
        }

        public void AddArg<TArg>(Trigger trigger, TArg arg)
        {
            TriggerArgs.Add(trigger.Key, arg);
        }

        public string GetTriggerToken(Trigger state)
        {
            var stateTokenSessionKey = $"{TriggerKey}-{state.Key}";
            if (_accessor.HttpContext.Session.Keys.Contains(stateTokenSessionKey))
            {
                return _accessor.HttpContext.Session.GetString(stateTokenSessionKey);
            }

            var stateToken = TokenGenerator.GetUniqueToken(64);

            _accessor.HttpContext.Session.SetString(stateToken, state.Key);
            _accessor.HttpContext.Session.SetString(stateTokenSessionKey, stateToken);

            return stateToken;
        }

        public string GetTriggerKeyFromToken(string token)
            => _accessor.HttpContext.Session.Keys.Contains(token)
                ? _accessor.HttpContext.Session.GetString(token)
                : throw new Exception("Trigger Token was not present in session");
    }
}
