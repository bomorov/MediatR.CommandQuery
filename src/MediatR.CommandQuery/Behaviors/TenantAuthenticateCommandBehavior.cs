﻿using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR.CommandQuery.Commands;
using MediatR.CommandQuery.Definitions;
using Microsoft.Extensions.Logging;

namespace MediatR.CommandQuery.Behaviors
{
    public class TenantAuthenticateCommandBehavior<TKey, TEntityModel, TResponse>
        : PipelineBehaviorBase<EntityModelCommand<TEntityModel, TResponse>, TResponse>
        where TEntityModel : class
    {

        private readonly ITenantResolver<TKey> _tenantResolver;

        public TenantAuthenticateCommandBehavior(ILoggerFactory loggerFactory, ITenantResolver<TKey> tenantResolver) : base(loggerFactory)
        {
            _tenantResolver = tenantResolver;
        }

        protected override async Task<TResponse> Process(EntityModelCommand<TEntityModel, TResponse> request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            await Authorize(request).ConfigureAwait(false);

            // continue pipeline
            return await next().ConfigureAwait(false);
        }

        private async Task Authorize(EntityModelCommand<TEntityModel, TResponse> request)
        {
            var principal = request.Principal;
            if (principal == null)
                return;

            // check principal tenant is same as model tenant
            if (!(request.Model is IHaveTenant<TKey> tenantModel))
                return;

            var tenantId = await _tenantResolver.GetTenantId(principal);
            if (Equals(tenantId, tenantModel.TenantId))
                return;

            throw new DomainException(HttpStatusCode.Forbidden, "User does not have access to specified tenant.");
        }
    }
}
