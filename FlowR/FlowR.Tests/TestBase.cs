﻿using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FlowR.Microsoft.Extensions.Logging;
using FlowR.StepLibrary.Activities;
using Xunit.Abstractions;

namespace FlowR.Tests
{
    public abstract class TestBase : ILoggerProvider
    {
        #region TestLogger class

        private class TestLogger : ILogger
        {
            private readonly string _categoryName;
            private readonly ITestOutputHelper _testOutput;

            public TestLogger(string categoryName, ITestOutputHelper output)
            {
                if (string.IsNullOrEmpty(categoryName))
                    throw new ArgumentException("message", nameof(categoryName));

                _categoryName = categoryName;
                _testOutput = output ?? throw new ArgumentNullException(nameof(output));
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return new LogScope();
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                var message = $"{logLevel.ToString().ToUpper()}: {formatter(state, exception)}";
                _testOutput.WriteLine(message);
            }

            private class LogScope : IDisposable
            {
                public void Dispose()
                {
                }
            }
        }

        #endregion

        #region Member declarations

        protected readonly ITestOutputHelper _output;

        #endregion

        #region Constructors

        protected TestBase(ITestOutputHelper output)
        {
            _output = output;
        }

        #endregion

        #region ILoggerProvider implementation

        public ILogger CreateLogger(string categoryName)
        {
            return new TestLogger(categoryName, _output);
        }

        public void Dispose()
        {
        }

        protected (IMediator, ILogger) GetMediator<T>() where T : IFlowStepRequest
        {
            return GetMediator<T>(addServices: null);
        }

        protected (IMediator, ILogger) GetMediator<T>(Action<ServiceCollection> addServices) where T : IFlowStepRequest
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddDebugLogging(this)
                .AddMediatR(typeof(IFlowHandler).Assembly)
                .AddMediatR(typeof(SetBoolFlowValueRequest).Assembly)
                .AddTransient(typeof(IFlowLogger<>), typeof(CoreFlowLogger<>))
                .AddMediatR(typeof(T).Assembly);

            addServices?.Invoke(serviceCollection);

            serviceCollection.BuildServiceProvider(this, out var mediator, out var logger);

            return (mediator, logger);
        }

        #endregion
    }

    #region ExtensionMethods

    static class ExtensionMethods
    {
#pragma warning disable IDE0060 // Remove unused parameter
        public static ILogger<T> GetLogger<T>(this ServiceProvider serviceCollection, T obj)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            return serviceCollection
                .GetService<ILoggerFactory>()?
                .CreateLogger<T>();
        }
    }

    #endregion
}
