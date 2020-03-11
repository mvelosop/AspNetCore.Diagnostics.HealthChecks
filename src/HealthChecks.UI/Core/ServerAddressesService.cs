﻿using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace HealthChecks.UI.Core
{
    internal class ServerAddressesService
    {
        private readonly IServer _server;

        public ServerAddressesService(IServer server)
        {
            _server = server;
        }

        internal ICollection<string> Addresses => AddressesFeature.Addresses;

        private IServerAddressesFeature AddressesFeature =>
            _server.Features.Get<IServerAddressesFeature>();

        internal string AbsoluteUriFromRelative(string relativeUrl)
        {
            var targetAddress = AddressesFeature.Addresses.First();

            Uri.TryCreate(targetAddress, UriKind.Absolute, out var original);

            if (targetAddress.EndsWith("/"))
            {
                targetAddress = targetAddress[0..^1];
            }

            if (!relativeUrl.StartsWith("/"))
            {
                relativeUrl = $"/{relativeUrl}";
            }

            var hostCheck = Uri.CheckHostName(original.DnsSafeHost);

            if (hostCheck != UriHostNameType.Dns)
            {
                targetAddress = $"{original.Scheme}://{Dns.GetHostName()}:{original.Port}";
            }

            return $"{targetAddress}{relativeUrl}";
        }
    }
}
