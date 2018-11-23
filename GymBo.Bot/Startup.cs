using System;
using System.Linq;
using GymBo.Bot.Models;
using GymBo.Bot.Services.MachineLearning;
using GymBo.Bot.Services.Sender;
using GymBo.Bot.Services.Smba;
using GymBo.Bot.Services.Sql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GymBo.Bot
{
    public class Startup
    {
        private ILoggerFactory _loggerFactory;
        private bool _isProduction = false;

        public Startup(IHostingEnvironment env)
        {
            _isProduction = env.IsProduction();
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var sqlConnection = Configuration.GetConnectionString("sqldb_connection");
            string appId = Configuration.GetSection("appId")?.Value;
            string appSecret = Configuration.GetSection("appSecret")?.Value;
            string mlUrl = Configuration.GetSection("mlConnection")?.Value;
            var storageConnection = Configuration.GetConnectionString("storage_connection");
            var secretKey = Configuration.GetSection("botFileSecret")?.Value;
            var botFilePath = Configuration.GetSection("botFilePath")?.Value;
            var botConfig = BotConfiguration.Load(botFilePath ?? @".\BotConfiguration.bot", secretKey);

            services.AddBot<GymBot>(options =>
            {
                services.AddSingleton(sp => botConfig ?? throw new InvalidOperationException($"The .bot config file could not be loaded. ({botConfig})"));

                var environment = _isProduction ? "production" : "development";
                var service = botConfig.Services.FirstOrDefault(s => s.Type == "endpoint" && s.Name == environment);
                if (!(service is EndpointService endpointService))
                {
                    throw new InvalidOperationException($"The .bot file does not contain an endpoint with name '{environment}'.");
                }

                options.CredentialProvider = new SimpleCredentialProvider(endpointService.AppId, endpointService.AppPassword);
                ILogger logger = _loggerFactory.CreateLogger<GymBot>();

                options.OnTurnError = async (context, exception) =>
                {
                    logger.LogError($"Exception caught : {exception}");
                    await context.SendActivityAsync("Sorry, it looks like something went wrong.");
                };

                const string DefaultBotContainer = "gymbotdata";

                IStorage dataStore = new Microsoft.Bot.Builder.Azure.AzureBlobStorage(storageConnection, DefaultBotContainer);

                var conversationState = new ConversationState(dataStore);

                options.State.Add(conversationState);
            });
            services.AddSingleton<ISmbaServices>(new SmbaServices(appId, appSecret, storageConnection));
            services.AddSingleton<ISqlServices>(new SqlServices(sqlConnection));
            services.AddScoped<ISenderServices, SenderServices>();
            services.AddSingleton<IMachineLearningServices>(new MachineLearningServices(mlUrl));
            services.AddSingleton<GymAccessors>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<BotFrameworkOptions>>().Value;
                if (options == null)
                {
                    throw new InvalidOperationException("BotFrameworkOptions must be configured prior to setting up the state accessors");
                }

                var conversationState = options.State.OfType<ConversationState>().FirstOrDefault();
                if (conversationState == null)
                {
                    throw new InvalidOperationException("ConversationState must be defined and added before adding conversation-scoped state accessors.");
                }

                var accessors = new GymAccessors(conversationState)
                {
                    GymUser = conversationState.CreateProperty<GymUser>("GymUser"),
                    ConversationDialogState = conversationState.CreateProperty<DialogState>("DialogState"),
                };

                return accessors;
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseBotFramework();
        }
    }
}
