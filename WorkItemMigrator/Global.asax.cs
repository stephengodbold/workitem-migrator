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
using WorkItemMigrator.Migration;
using WorkItemMigrator.Migration.Locators;
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
            container.AddFacility<CollectionFacility>();

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
                Component.For<IItemView>()
                    .ImplementedBy<ItemView>()
                    .LifeStyle.Is(LifestyleType.PerWebRequest),
                Component.For<IResultsView>()
                    .ImplementedBy<ResultsView>()
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
                Component.For<IServiceLocatorSelector>()
                    .AsFactory(f => f.SelectedWith<NameSelector>()),
                Component.For<IRepositorySelector>()
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
                    .BasedOn<IRepository>()
                    .ConfigureFor<IRepository>(
                        registration => {
                            registration.LifeStyle.Is(LifestyleType.Transient);
                            registration.Named(registration.Implementation.Name);
                        })
                    .WithService
                    .Select(new[] {typeof(IRepository)}),

                AllTypes.FromAssemblyInDirectory(extensionAssemblyFilter)
                    .BasedOn<IServiceLocator>()
                    .ConfigureFor<IServiceLocator>(
                        registration =>
                        {
                            registration.LifeStyle.Is(LifestyleType.Transient);
                            registration.Named(registration.Implementation.Name);
                        })
                    .WithService.Select(new[] {typeof(IServiceLocator)}),


                Component.For<SearchHub>()
                    .ImplementedBy<SearchHub>()
                    .LifeStyle.Is(LifestyleType.PerWebRequest)
            );

        }

        private void CatalogueExtensions(IWindsorContainer container)
        {
            var extensionManager = new ExtensionManager
                                       {
                                           Repositories = new Dictionary<string, string>()
                                       };

            container.ResolveAll<IRepository>().ForEach(
                repository => extensionManager.Repositories.Add(repository.GetType().Name, repository.FriendlyName));

            container.Register(
                Component.For<IExtensionManager>()
                    .Instance(extensionManager)
                    .LifeStyle.Is(LifestyleType.Singleton)
            );
        }
    }
}