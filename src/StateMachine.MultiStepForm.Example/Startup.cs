using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore.Mvc;
using SimpleInjector.Lifestyles;
using StateMachine.MultiStepForm.Contexts;
using StateMachine.MultiStepForm.Example.Commands;
using StateMachine.MultiStepForm.Example.Commands.DeepThought;
using StateMachine.MultiStepForm.Example.CrossCuttingConcerns;
using StateMachine.MultiStepForm.Example.Models.DeepThought;
using StateMachine.MultiStepForm.Example.Queries;
using StateMachine.MultiStepForm.Example.Queries.DeepThought;
using StateMachine.MultiStepForm.Example.Specifications;
using StateMachine.MultiStepForm.Example.Specifications.DeepThought;
using StateMachine.MultiStepForm.Example.StateMachines.DeepThought;

namespace StateMachine.MultiStepForm.Example
{
    public class Startup
    {
        public static readonly Container Container = new Container();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddSessionStateTempDataProvider();
            services.AddSession();

            IntegrateSimpleInjector(services);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            InitialiseContainer(app);

            Container.Verify(VerificationOption.VerifyAndDiagnose);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "Default", // Route name 
                    "{controller}/{action}/{id}", // URL with parameters 
                    new {controller = "Home", action = "Index", id = ""} // Parameter defaults
                );
            });
        }

        private void IntegrateSimpleInjector(IServiceCollection services)
        {
            Container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            Container.Options.DefaultLifestyle = Lifestyle.Scoped;

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IControllerActivator>(
                new SimpleInjectorControllerActivator(Container));
            services.AddSingleton<IViewComponentActivator>(
                new SimpleInjectorViewComponentActivator(Container));

            services.EnableSimpleInjectorCrossWiring(Container);
            services.UseSimpleInjectorAspNetRequestScoping(Container);
        }

        private void InitialiseContainer(IApplicationBuilder app)
        {
            // Add application presentation components:
            Container.RegisterMvcControllers(app);
            Container.RegisterMvcViewComponents(app);

            Container.Register<ICommandHandler<SubmitYourQuestion>, SubmitYourQuestionCommandHandler>();
            Container.Register<IQueryHandler<GetYourQuestion, string>, GetYourQuestionQueryHandler>();
            
            Container.Register<AbstractStateMachine<DeepThoughtStates.State, DeepThoughtTriggers.Trigger>, DeepThoughtStateMachine>();

            Container.RegisterDecorator(typeof(ICommandHandler<>), typeof(VerboseLoggingCommandHandlerDecorator<>));

            var assembly = GetType().Assembly;
            RegisterScopedCollection<State>(assembly);
            RegisterScopedCollection<Trigger>(assembly);

            Container.Register<StateContext>();
            Container.Register<TriggerContext>();
            Container.Register<Specification<AnswerViewModel>, MeaningOfLifeSpecification>();

            // Allow Simple Injector to resolve services from ASP.NET Core.
            Container.AutoCrossWireAspNetComponents(app);
        }

        private void RegisterScopedCollection<TType>(Assembly assembly)
        {
            var registrationTypes = Container.GetTypesToRegister(typeof(TType), assembly);
            foreach (var registrationType in registrationTypes)
            {
                var registration = Lifestyle.Scoped.CreateRegistration(registrationType, Container);
                Container.Collection.Append(typeof(TType), registration);
            }   
        }
    }
}