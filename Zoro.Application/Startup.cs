using FluentValidation;
using LightInject;
using MediatR;
using MediatR.Pipeline;
using Zoro.Application.Infrastructure;
using System;
using System.Reflection;

namespace Zoro.Application
{
    public class IoCBootstrapper : ICompositionRoot
    {
        public void Compose(IServiceRegistry container)
        {
            //container.RegisterFrom<IoCBootstrapper>();

            RegisterMediator(container);
        }

        private void RegisterMediator(IServiceRegistry serviceContainer)
        {
            // Add MediatR pipeline
            serviceContainer.RegisterOrdered(typeof(IPipelineBehavior<,>),
                new[]
                {
                    typeof(RequestPreProcessorBehavior<,>),
                    //typeof(RequestPerformanceBehaviour<,>),
                    typeof(RequestValidationBehavior<,>),
                    typeof(RequestPostProcessorBehavior<,>),
                }, type => new PerContainerLifetime());

            // Register mediator
            serviceContainer.Register<IMediator, Mediator>();

            // Register request handlers
            var assembly = Assembly.GetAssembly(typeof(IoCBootstrapper));
            serviceContainer.RegisterAssembly(assembly, (serviceType, implementingType) =>
                serviceType.IsConstructedGenericType &&
                (
                    serviceType.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                    serviceType.GetGenericTypeDefinition() == typeof(IRequestHandler<>) ||
                    serviceType.GetGenericTypeDefinition() == typeof(INotificationHandler<>) ||
                    serviceType.GetGenericTypeDefinition() == typeof(IValidator<>)
                ));

            //serviceContainer.RegisterOrdered(typeof(IRequestPostProcessor<,>),
            //    new[]
            //    {
            //        typeof(GenericRequestPostProcessor<,>),
            //        typeof(ConstrainedRequestPostProcessor<,>)
            //    }, type => new PerContainerLifetime());

            serviceContainer.Register<ServiceFactory>(serviceFactory => serviceFactory.GetInstance);
        }

    }
}
