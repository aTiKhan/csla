﻿#if NETSTANDARD2_0
//-----------------------------------------------------------------------
// <copyright file="ConfigurationExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for .NET Core configuration</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Configuration
{
  /// <summary>
  /// Implement extension methods for .NET Core configuration
  /// </summary>
  public static class ConfigurationExtensions
  {
    /// <summary>
    /// Add CSLA .NET services for use by the application.
    /// </summary>
    /// <param name="services">ServiceCollection object</param>
    public static ICslaBuilder AddCsla(this IServiceCollection services)
    {
      return AddCsla(services, null);
    }

    /// <summary>
    /// Add CSLA .NET services for use by the application.
    /// </summary>
    /// <param name="services">ServiceCollection object</param>
    /// <param name="config">Implement to configure CSLA .NET</param>
    public static ICslaBuilder AddCsla(this IServiceCollection services, Action<CslaConfiguration> config)
    {
      ApplicationContext.SetServiceCollection(services);
      services.AddTransient(typeof(IDataPortal<>), typeof(DataPortal<>));
      CreateWebContextManager(services);
      config?.Invoke(CslaConfiguration.Configure());
      return new CslaBuilder();
    }

    private static void CreateWebContextManager(IServiceCollection services)
    {
      var webManagerType = Type.GetType("Csla.AspNetCore.ApplicationContextManager, Csla.AspNetCore");
      if (webManagerType != null)
      {
        ApplicationContext.WebContextManager = 
          (IContextManager)Activator.CreateInstance(webManagerType, services.BuildServiceProvider());
      }
    }

    /// <summary>
    /// Configure CSLA .NET settings from .NET Core configuration
    /// subsystem.
    /// </summary>
    /// <param name="config">Configuration object</param>
    public static IConfiguration ConfigureCsla(this IConfiguration config)
    {
      config.Bind("csla", new CslaConfigurationOptions());
      return config;
    }
  }
}
#endif