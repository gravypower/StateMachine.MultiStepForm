using System;
using Microsoft.AspNetCore.Http;

namespace StateMachine.MultiStepForm.Example.Commands.DeepThought
{
    public class SubmitYourQuestionCommandHandler:ICommandHandler<SubmitYourQuestion>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SubmitYourQuestionCommandHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Handle(SubmitYourQuestion command)
        {
            _httpContextAccessor.HttpContext.Session.SetString("YourQuestion", command.Question);
        }
    }
}
