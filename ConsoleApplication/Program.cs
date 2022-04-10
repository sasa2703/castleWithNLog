using System;
using System.Runtime.ExceptionServices;
using Castle.Core.Logging;
using Castle.Facilities.Logging;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace ConsoleApplication
{   
    class Program
    {
        private static IObjectGraphRoot logResolved;
        static void Main()
        {          
            AppDomain.CurrentDomain.FirstChanceException += nhandledExceptionTrapper;
            BootStrapContainer();
            try
            {
                MakeError();
            }
            catch
            {

            }
            Console.ReadKey();
        }

        private static void nhandledExceptionTrapper(object sender, FirstChanceExceptionEventArgs e)
        {
            logResolved.LogError(e.Exception);            
        }        

        private static void MakeError()
        {           
            throw new NotImplementedException();         
        }

        private static void BootStrapContainer()
        {
            using (var container = new WindsorContainer())
            {
                logResolved = container.Install(new MyWindsorInstaller())
                    .Resolve<IObjectGraphRoot>();
                    
            }
        }
    }

    internal interface IObjectGraphRoot
    {
        void Log();
        void LogError(Exception ex);
    }

    internal class MyWindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.AddFacility<LoggingFacility>(f => f.LogUsing(LoggerImplementation.NLog)
                                                         .WithConfig("NLog.config"));

            container.Register(
                Component
                    .For<IObjectGraphRoot>()
                    .ImplementedBy<ObjectGraphRoot>());
        }
    }

    internal class ObjectGraphRoot : IObjectGraphRoot
    {
        private ILogger _logger = NullLogger.Instance;

        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        public void Log()
        {
            _logger.Info("Log something");
        }

        public void LogError(Exception ex)
        {
            _logger.Error("Error", ex);
        }
    }
}
