using Quinntyne.Schematics.Infrastructure.Interfaces;
using Quinntyne.Schematics.Infrastructure.Services;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Quinntyne.Schematics.Infrastructure.Behaviours
{
    public class NamespaceResolverBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private INamespaceProvider _namespaceProvider;
        public NamespaceResolverBehaviour(INamespaceProvider namespaceProvider)
        {
            _namespaceProvider = namespaceProvider;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            
            if(request as ICodeGeneratorCommandRequest != null)
            {
                var @namespace = _namespaceProvider.GetNamespace(System.Environment.CurrentDirectory); 

                if(string.IsNullOrEmpty((request as ICodeGeneratorCommandRequest).Namespace))
                    (request as ICodeGeneratorCommandRequest).Namespace = @namespace.Value;

                (request as ICodeGeneratorCommandRequest).RootNamespace = @namespace.Root;
            }

            var response = await next();

            return response;
        }
    }
}
