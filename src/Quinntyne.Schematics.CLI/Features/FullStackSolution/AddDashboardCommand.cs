using FluentValidation;
using MediatR;
using Quinntyne.Schematics.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static System.IO.Directory;

namespace Quinntyne.Schematics.CLI.Features.FullStackSolution
{
    public class AddDashboardCommand
    {
        public class Request : Options, IRequest, ICodeGeneratorCommandRequest
        {
            public Request(IOptions options)
            {
                Entity = options.Entity;
                Directory = options.Directory;
                Namespace = options.Namespace;
                RootNamespace = options.RootNamespace;
                Options = options;
            }

            public dynamic Settings { get; set; }
            public IOptions Options { get; set; }
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(request => request.Entity).NotNull();
            }
        }

        public class Handler : IRequestHandler<Request>
        {
            private readonly IMediator _mediator;

            public Handler(
                IMediator mediator
                )
            {
                _mediator = mediator;
            }

            private void CreateIfDoesntExist(string directory)
            {
                Console.WriteLine(directory);

                if (!Exists(directory)) CreateDirectory(directory);
            }

            public async Task Handle(Request request, CancellationToken cancellationToken) {

                foreach (var directory in new List<string>() {
                    $@"{request.SolutionDirectory}\src\{request.RootNamespace}.API\Features\Cards\",
                    $@"{request.SolutionDirectory}\src\{request.RootNamespace}.API\Features\Dashboards\",
                    $@"{request.SolutionDirectory}\src\{request.RootNamespace}.API\Features\DashboardCards\",
                    $@"{request.SolutionDirectory}\src\{request.RootNamespace}.SPA\ClientApp\src\app\cards\",
                    $@"{request.SolutionDirectory}\src\{request.RootNamespace}.SPA\ClientApp\src\app\dashboards\",
                    $@"{request.SolutionDirectory}\src\{request.RootNamespace}.SPA\ClientApp\src\app\dashboard-cards\"
                })
                    CreateIfDoesntExist(directory);
                
                var models = new List<string>() {
                    "Card",
                    "Dashboard",
                    "DashboardCard"
                };

                var events = new List<string>() {
                    "CardCreated",
                    "CardNameChanged",
                    "CardRemoved",
                    "DashboardCardAddedToDashboard",
                    "DashboardCardCreated",
                    "DashboardCardNameChanged",
                    "DashboardCardOptionsUpdated",
                    "DashboardCardRemoved",
                    "DashboardCardRemovedFromDashboard",
                    "DashboardCreated",
                    "DashboardNameChanged",
                    "DashboardRemoved"
                };

                var cardApi = new List<string>() {
                    "CardDto",
                    "CardsController",
                    "CreateCardCommand",
                    "GetCardByIdQuery",
                    "GetCardsQuery",
                    "RemoveCardCommand",
                    "UpdateCardCommand",
                };

                var dashboardCardApi = new List<string>() {
                    "CreateDashboardCardCommand",
                    "DashboardCardDto",
                    "DashboardCardsController",
                    "GetDashboardCardByIdQuery",
                    "GetDashboardCardByIdsQuery",
                    "GetDashboardCardsQuery",
                    "RemoveDashboardCardCommand",
                    "SaveDashboardCardRangeCommand",
                    "UpdateDashboardCardCommand"
                };

                var dashboardApi = new List<string>() {
                    "CreateDashboardCommand",
                    "DashboardDto",
                    "DashboardsController",
                    "GetDashboardByDefaultQuery",
                    "GetDashboardByIdQuery",
                    "GetDashboardsQuery",
                    "RemoveDashboardCommand",
                    "UpdateDashboardCommand",
                };

                var classes = string.Join(",",models.Concat(events).Concat(cardApi).Concat(dashboardApi).Concat(dashboardCardApi).ToArray());

                var cardClient = new List<string>() {
                    "card.model.ts",
                    "card.service.ts",
                    "cards.module.ts"
                };
                
                var dashboardCardClient = new List<string>() {
                    "dashboard-card.component.css",
                    "dashboard-card.component.ts",
                    "dashboard-card.component.html",
                    "dashboard-card.model.ts",
                    "dashboard-card.service.ts",
                    "dashboard-card-configuration-overlay.component.css",
                    "dashboard-card-configuration-overlay.component.html",
                    "dashboard-card-configuration-overlay.component.ts",
                    "dashboard-card-configuration-overlay.ts",
                    "dashboard-cards.module.ts",
                };

                var dashboardClient = new List<string>() {
                    "add-dashboard-card-overlay.component.css",
                    "add-dashboard-card-overlay.component.html",
                    "add-dashboard-card-overlay.component.ts",
                    "add-dashboard-card-overlay.ts",
                    "dashboard.model.ts",
                    "dashboard.service.ts",
                    "dashboards.module.ts",
                    "dashboard-page.component.css",
                    "dashboard-page.component.html",
                    "dashboard-page.component.ts"
                };

                var files = string.Join(",",cardClient.Concat(dashboardCardClient).Concat(dashboardClient).ToArray());

                await _mediator.Send(new GenerateClassCommand.Request(request.Options)
                {
                    ClassName = classes,
                    SolutionDirectory = request.SolutionDirectory
                });

                await _mediator.Send(new GenerateFileCommand.Request(request.Options)
                {
                    Name = files,
                    SolutionDirectory = request.SolutionDirectory
                });
            }
        }
    }
}
