using Microsoft.AspNetCore.Http;

namespace StateMachine.MultiStepForm.Example.Queries.DeepThought
{
    public class GetYourQuestionQueryHandler:IQueryHandler<GetYourQuestion, string>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetYourQuestionQueryHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public string Handle(GetYourQuestion query)
        {
            return _httpContextAccessor.HttpContext.Session.GetString("YourQuestion");
        }
    }
}
