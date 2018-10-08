using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore;
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
        private readonly Container _container = new Container();

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

            _container.Verify(VerificationOption.VerifyAndDiagnose);

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
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IControllerActivator>(
                new SimpleInjectorControllerActivator(_container));
            services.AddSingleton<IViewComponentActivator>(
                new SimpleInjectorViewComponentActivator(_container));

            services.EnableSimpleInjectorCrossWiring(_container);
            services.UseSimpleInjectorAspNetRequestScoping(_container);
        }

        private void InitialiseContainer(IApplicationBuilder app)
        {
            // Add application presentation components:
            _container.RegisterMvcControllers(app);
            _container.RegisterMvcViewComponents(app);

            _container.Register<ICommandHandler<SubmitYourQuestion>, SubmitYourQuestionCommandHandler>();
            _container.Register<IQueryHandler<GetYourQuestion, string>, GetYourQuestionQueryHandler>();
            _container.Register<Specification<AnswerViewModel>, MeaningOfLifeSpecification>();
            _container.Register<AbstractStateMachine<State, Trigger>, DeepThoughtStateMachine>();

            _container.RegisterDecorator(typeof(ICommandHandler<>), typeof(VerboseLoggingCommandHandlerDecorator<>));

            var assembly = GetType().Assembly;
            var triggers = _container.GetTypesToRegister(typeof(Trigger), assembly);

            foreach (var trigger in triggers)
            {
                var registration = Lifestyle.Scoped.CreateRegistration(trigger, _container);
                _container.AddRegistration(trigger, registration);
            }

            var states = _container.GetTypesToRegister(typeof(State), assembly);

            foreach (var state in states)
            {
                var registration = Lifestyle.Scoped.CreateRegistration(state, _container);
                _container.AddRegistration(state, registration);
            }

            _container.Collection.Register<Trigger>(assembly);
            _container.Collection.Register<State>(assembly);

            _container.Register<StateContext>();
            _container.Register<TriggerContext>();

            // Allow Simple Injector to resolve services from ASP.NET Core.
            _container.AutoCrossWireAspNetComponents(app);
        }
    }
}