using System.Web.Mvc;
using Ninject.Web.Mvc;


[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(GPManagementSytem.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(GPManagementSytem.App_Start.NinjectWebCommon), "Stop")]

namespace GPManagementSytem.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using Ninject.Web.Common.WebHost;
    using GPManagementSytem.Database;
    using GPManagementSytem.Services;
    using GPManagementSytem.SessionManager;

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

            kernel.Bind<ISessionManager>().To<SessionManager>().InRequestScope().WithConstructorArgument("context", ninjectContext => HttpContext.Current.Session);

            DependencyResolver.SetResolver(new NinjectDependencyResolver(kernel));
            // allows the session to be passed here
            // http://stackoverflow.com/a/8366402/400983

            //kernel.Bind<ILog>().ToMethod(context => LogManager.GetLogger(context.Request.Target.Member.DeclaringType));

            //kernel.BindFilter<RoleAuthorizeFilter>(FilterScope.Controller, 0).WhenControllerHas<RoleAuthorizeAttribute>();
            //kernel.BindFilter<RoleAuthorizeFilter>(FilterScope.Action, 0).WhenActionMethodHas<RoleAuthorizeAttribute>();

            //kernel.Bind<IMailSender>().To<MailSender>();
            kernel.Bind<IDatabaseEntities>().To<DatabaseEntities>();

            kernel.Bind<IUserService>().To<UserService>();
            kernel.Bind<IPracticeService>().To<PracticeService>();
            kernel.Bind<IAllocationService>().To<AllocationService>();
        }

    }

}
