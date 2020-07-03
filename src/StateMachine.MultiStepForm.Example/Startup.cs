using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddSession();
            
            services.AddLogging();
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            
            // Sets up the basic configuration that for integrating Simple Injector with
            // ASP.NET Core by setting the DefaultScopedLifestyle, and setting up auto
            // cross wiring.
            services.AddSimpleInjector(Container, options =>
            {
                // AddAspNetCore() wraps web requests in a Simple Injector scope and
                // allows request-scoped framework services to be resolved.
                options.AddAspNetCore()

                    // Ensure activation of a specific framework type to be created by
                    // Simple Injector instead of the built-in configuration system.
                    // All calls are optional. You can enable what you need. For instance,
                    // ViewComponents, PageModels, and TagHelpers are not needed when you
                    // build a Web API.
                    .AddControllerActivation()
                    .AddViewComponentActivation()
                    .AddPageModelActivation()
                    .AddTagHelperActivation();

                // Optionally, allow application components to depend on the non-generic
                // ILogger (Microsoft.Extensions.Logging) or IStringLocalizer
                // (Microsoft.Extensions.Localization) abstractions.
                options.AddLogging();
                options.AddLocalization();
            });
            
            InitializeContainer();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseSimpleInjector(Container);

            // Default ASP.NET middleware
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            
            Container.Verify();
        }
        private void InitializeContainer()
        {
            // Add application presentation components:
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
        }
        
        private void RegisterScopedCollection<TType>(Assembly assembly)
        {
            var registrationTypes = Container.GetTypesToRegister(typeof(TType), assembly);
            foreach (var registrationType in registrationTypes)
            {
                var registration = Lifestyle.Scoped.CreateRegistration(registrationType, Container);
                Container.Collection.Append(typeof(TType), registration);
                Container.AddRegistration(registrationType, registration);
            }   
        }
    }
}