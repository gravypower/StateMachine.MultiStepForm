using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace StateMachine.MultiStepForm.Contexts
{
    public class TriggerContext
    {
        private readonly IHttpContextAccessor _accessor;

        private IDictionary<string, object> TriggerArgs
        {
            get
            {
                if (!_accessor.HttpContext.Items.ContainsKey("triggerArgs"))
                {
                    _accessor.HttpContext.Items.Add("triggerArgs", new Dictionary<string, object>());
                }

                return _accessor.HttpContext.Items["triggerArgs"] as Dictionary<string, object>;
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

            return (TArg)TriggerArgs[trigger.Key];
        }

        public void AddArg<TArg>(Trigger trigger, TArg arg)
        {
            TriggerArgs.Add(trigger.Key, arg);
        }
    }
}
