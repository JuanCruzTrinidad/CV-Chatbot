using CV_Chatbot.Dialogs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Declarative;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CV_Chatbot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers().AddNewtonsoftJson();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CV_Chatbot", Version = "v1" });
            });

            services.AddMvc();
            
            // Required for memory paths introduced by adaptive dialogs.
            ComponentRegistration.Add(new DialogsComponentRegistration());

            // Register declarative components
            ComponentRegistration.Add(new DeclarativeComponentRegistration());

            ComponentRegistration.Add(new LanguageGenerationComponentRegistration());


            // Register adapive dialog components
            ComponentRegistration.Add(new AdaptiveComponentRegistration());

            // Add LUIS component
            ComponentRegistration.Add(new LuisComponentRegistration());

            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Create the storage we'll be using for User and Conversation state. (Memory is great for testing purposes.)
            services.AddSingleton<IStorage, MemoryStorage>();

            // Create the User state. (Used in this bot's Dialog implementation.)
            services.AddSingleton<UserState>();

            // Create the Conversation state. (Used by the Dialog system itself.)
            services.AddSingleton<ConversationState>();

            // The Dialog that will be run by the bot.
            services.AddSingleton<RootDialog>();

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddSingleton<IBot, DialogBot<RootDialog>>();

            services.AddDirectoryBrowser();

            //services.AddSpaStaticFiles(configuration => configuration.RootPath = "wwwroot");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CV_Chatbot v1"));
            // app.UseHttpsRedirection();
            // app.UseDefaultFiles();
            // app.UseStaticFiles();
            app.UseFileServer(enableDirectoryBrowsing: true);
            app.UseRouting();

            app.UseAuthorization();

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "clientapp/src";
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
