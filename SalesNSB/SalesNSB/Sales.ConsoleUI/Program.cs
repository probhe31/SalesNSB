using System;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Persistence.Sql;
using Sales.Contracts.Commands;

namespace Sales.ConsoleUI
{
    class Program
    {
        static ILog log = LogManager.GetLogger<Program>();

        static async Task Main()
        {
            var endPointName = "Sales.ConsoleUI";
            Console.Title = endPointName;
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

            var routing = transport.Routing();
            routing.RouteToEndpoint(typeof(PlaceOrderCommand), "Sales.EndPoint");
            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);
            await RunLoop(endpointInstance)
                .ConfigureAwait(false);
            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }

        static async Task RunLoop(IEndpointInstance endpointInstance)
        {
            while (true)
            {
                log.Info("Press 'P' to place an order, or 'Q' to quit.");
                var key = Console.ReadKey();
                Console.WriteLine();
                switch (key.Key)
                {
                    case ConsoleKey.P:
                        var command = new PlaceOrderCommand
                        {
                            OrderId = Guid.NewGuid().ToString(),
                            OrderData = "OrderData"
                        };
                        log.Info($"Sending PlaceOrder command, OrderId = {command.OrderId}");
                        await endpointInstance.Send(command)
                            .ConfigureAwait(false);
                        break;
                    case ConsoleKey.Q:
                        return;
                    default:
                        log.Info("Unknown input. Please try again.");
                        break;
                }
            }
        }
    }
}
