using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using NServiceBus;
using NServiceBus.Persistence.Sql;
using Sales.API.Application.Response;
using Sales.API.Application.Validations;
using Sales.Contracts.Commands;
using System;

namespace Sales.API
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
            try
            {
                services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
                var endPointName = "Sales.API";
                var endpointConfiguration = new EndpointConfiguration(endPointName);
                //var transport = endpointConfiguration.UseTransport<LearningTransport>();

                // Configure RabbitMQ transport
                var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
                transport.UseConventionalRoutingTopology();
                string rabbitmqUrl = Environment.GetEnvironmentVariable("RABBITMQ_PCF_NSB_URL");
                transport.ConnectionString(rabbitmqUrl);

                // Configure persistence
                var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
                persistence.SqlDialect<SqlDialect.MySql>();
                string mysqlConnectionString = Environment.GetEnvironmentVariable("MYSQL_AWS_NSB_URL");
                persistence.ConnectionBuilder(
                    connectionBuilder: () =>
                    {
                        return new MySqlConnection(mysqlConnectionString);
                    }
                );

                // Enable the Outbox.
                endpointConfiguration.EnableOutbox();

                // Make sure NServiceBus creates queues in RabbitMQ, tables in MYSQL Server, etc.
                // You might want to turn this off in production, so that DevOps can use scripts to create these.
                endpointConfiguration.EnableInstallers();

                // Turn on auditing.
                //endpointConfiguration.AuditProcessedMessagesTo("audit");

                endpointConfiguration.SendOnly();

                var routing = transport.Routing();
                routing.RouteToEndpoint(typeof(PlaceOrderCommand), "Sales.EndPoint");
                var endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

                services.AddSingleton<IMessageSession>(endpoint);
                services.AddSingleton<PlaceOrderCommandValidator>();
                services.AddSingleton<ApiResponseHandler>();
            } catch(Exception ex) {
                Console.WriteLine(ex.StackTrace);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
