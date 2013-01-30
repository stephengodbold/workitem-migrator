using System;
using System.Collections.Generic;
using System.Web;
using Castle.Core;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using WebFormsMvp;
using WebFormsMvp.Binder;
using WebFormsMvp.Castle;
using WorkItemMigrator.Migration.Locators;
using WorkItemMigrator.Migration.Providers;
using WorkItemMigrator.Services;
using WorkItemMigrator.Views;

namespace WorkItemMigrator
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            var container = new WindsorContainer();
            container.AddFacility<TypedFactoryFacility>();

            RegisterViews(container);
            RegisterPresentersByConvention(container);
            RegisterMigrationTypes(container);
            RegisterExtensions(container);
            CatalogueExtensions(container);

            PresenterBinder.Factory = new CastlePresenterFactory(container.Kernel);
        }

        private void RegisterViews(IWindsorContainer container)
        {
            container.Register(
                Component.For<IDefaultView>()
                    .ImplementedBy<DefaultView>()
                    .LifeStyle.Is(LifestyleType.PerWebRequest),
                Component.For<ISearchView>()
                    .ImplementedBy<SearchView>()
                    .LifeStyle.Is(LifestyleType.PerWebRequest),
                Component.For<IResultView>()
                    .ImplementedBy<ResultView>()
                    .LifeStyle.Is(LifestyleType.PerWebRequest)
            );
        }

        private void RegisterPresentersByConvention(IWindsorContainer container)
        {
            container.Register(
                AllTypes.FromThisAssembly()
                    .Where(type => type.Name.EndsWith("Presenter"))
                    .ConfigureFor<IPresenter>(registration => registration.LifeStyle.PerWebRequest));
        }

        private void RegisterMigrationTypes(IWindsorContainer container)
        {
            container.Register(
                Component.For<ISearchProviderFactory>()
                    .AsFactory(f => f.SelectedWith<NameSelector>()),
                Component.For<IServiceLocatorFactory>()
                    .AsFactory(f => f.SelectedWith<NameSelector>()),
                Component.For<IRepositoryProviderFactory>()
                    .AsFactory(f => f.SelectedWith<NameSelector>()),
                Component.For<NameSelector>()
            );
        }

        private void RegisterExtensions(IWindsorContainer container)
        {
            var extensionsPath = Server.MapPath("~/bin");
            var extensionAssemblyFilter = new AssemblyFilter(extensionsPath, "*.Migration.dll");

            container.Register(
                AllTypes.FromAssemblyInDirectory(extensionAssemblyFilter)
                    .BasedOn<ISearchProvider>()
                    .ConfigureFor<ISearchProvider>(
                        registration =>
                        {
                            registration.LifeStyle.Is(LifestyleType.Transient);
                            registration.Named(registration.Implementation.Name);
                        })
                    .WithService.Select(new[] { typeof(ISearchProvider) }),

                AllTypes.FromAssemblyInDirectory(extensionAssemblyFilter)
                    .BasedOn<IRepositoryProvider>()
                    .ConfigureFor<IRepositoryProvider>(
                        registration => {
                            registration.LifeStyle.Is(LifestyleType.Transient);
                            registration.Named(registration.Implementation.Name);
                        })
                    .WithService
                    .Select(new[] {typeof(IRepositoryProvider)}),

                AllTypes.FromAssemblyInDirectory(extensionAssemblyFilter)
                    .BasedOn<IServiceLocator>()
                    .ConfigureFor<IServiceLocator>(
                        registration =>
                        {
                            registration.LifeStyle.Is(LifestyleType.Transient);
                            registration.Named(registration.Implementation.Name);
                        })
                    .WithService.Select(new[] {typeof(IServiceLocator)})
            );

            
        }

        private void CatalogueExtensions(IWindsorContainer container)
        {
            var extensionManager = new ExtensionManager
                                       {
                                           SearchProviders = new Dictionary<string, string>(),
                                           Repositories = new Dictionary<string, string>()
                                       };

            container.ResolveAll<ISearchProvider>().ForEach(
                provider => extensionManager.SearchProviders.Add(provider.GetType().Name, provider.FriendlyName));
            container.ResolveAll<IRepositoryProvider>().ForEach(
                repository => extensionManager.Repositories.Add(repository.GetType().Name, repository.FriendlyName));

            container.Register(
                Component.For<IExtensionManager>()
                    .Instance(extensionManager)
                    .LifeStyle.Is(LifestyleType.Singleton)
            );
        }

    }

    internal class ExtensionManager : IExtensionManager
    {
        public IDictionary<string, string> SearchProviders { get; set; }
        public IDictionary<string, string> Repositories { get; set; }
    }

    public interface IExtensionManager
    {
        IDictionary<string, string> SearchProviders { get; set; }
        IDictionary<string, string> Repositories { get; set; }
    }
}