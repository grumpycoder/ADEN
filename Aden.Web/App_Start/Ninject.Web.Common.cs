using Aden.Core.Services;
using ALSDE.Services;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Aden.Web.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(Aden.Web.App_Start.NinjectWebCommon), "Stop")]

namespace Aden.Web.App_Start
{
    using Aden.Core.Data;
    using Aden.Core.Repositories;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Common;
    using Ninject.Web.Common.WebHost;
    using System;
    using System.Web;
    using WebApiContrib.IoC.Ninject;

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);

                System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver = new NinjectResolver(kernel);

                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<AdenContext>().ToSelf().InRequestScope();
            kernel.Bind<IUnitOfWork>().To<UnitOfWork>();
            kernel.Bind<IFileSpecificationRepository>().To<FileSpecificationRepository>();
            kernel.Bind<IReportRepository>().To<ReportRepository>();
            kernel.Bind<ISubmissionRepository>().To<SubmissionRepository>();
            kernel.Bind<IWorkItemRepository>().To<WorkItemRepository>();
            kernel.Bind<IDocumentRepository>().To<DocumentRepository>();
            //kernel.Bind<IMembershipService>().To<InMemoryMembershipService>();
            kernel.Bind<IMembershipService>().To<IdemMembershipService>();
            kernel.Bind<INotificationService>().To<EmailNotificationService>();
            kernel.Bind<IGroupService>().To<IdemGroupService>();
            //kernel.Bind<IUserService>().To<IdemUserService>().InTransientScope();

        }
    }
}
