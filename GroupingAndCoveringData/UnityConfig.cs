using System;
using System.Linq;
using BusinessLogicLayer.AttributeData;
using BusinessLogicLayer.Data;
using BusinessLogicLayer.DataValidation;
using BusinessLogicLayer.ExcelWriter;
using BusinessLogicLayer.Matrix;
using BusinessLogicLayer.Matrix.CoverTools;
using BusinessLogicLayer.Matrix.CoverTools.GroupingManager;
using BusinessLogicLayer.Reader;
using BusinessLogicLayer.TrainingObjectsProceder;
using Core.Common.Interfaces;
using Core.Common.Interfaces.Excell;
using Core.Common.Interfaces.GroupingManager;
using Models.ViewModels;
using Unity;
using Unity.Lifetime;
using Unity.RegistrationByConvention;

namespace GroupingAndCoveringData
{
    public class UnityConfig
    {
        #region Unity Container
        private static readonly Lazy<IUnityContainer> Container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity unityContainer.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return Container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity unityContainer.</summary>
        /// <param name="unityContainer">The unity unityContainer to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity alhighs resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer unityContainer)
        {
            // NOTE: To load from web.config uncomment the line behigh. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // unityContainer.LoadConfiguration();            

            unityContainer.RegisterType<IAttributeColumnConverter, AttributeColumnConverter>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<IDataReader, DataReader>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<IFileReader, FileReader>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<IFileChecker, FileChecker>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<ITrainingObjectsConverter, TrainingObjectsConverter>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<IOpenFileDialog,OpenFileDialogWrapper>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<IPredictionMatrixWriter, PredictionMatrixWriter>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<IAttributeWriter, AttributeWriter>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<ISaveFileDialog, SaveFileDialogWrapper>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<IClassViewer, ClassViewer>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<IDataObjectsConverter, DataObjectsConverter>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<ITxtExporter, TxtExporter>(new ContainerControlledLifetimeManager());            
            unityContainer.RegisterType<IExcelWriter, ExcelWriter>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<IMatrixToGridMatrix, MatrixToGridMatrix>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<ICoverMatrixClassificator, CoverMatrixClassificator>(new ContainerControlledLifetimeManager()); 
            unityContainer.RegisterType<ICoverMatrixManager, CoverMatrixManager>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<ICoverCalculator, CoverCalculator>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<ICoverGradeService, CoverGradeService>(new ContainerControlledLifetimeManager());            
            unityContainer.RegisterTypes(
                AllClasses.FromLoadedAssemblies().
                    Where(type => typeof(IGroupingMethod).IsAssignableFrom(type)),
                WithMappings.FromAllInterfaces,
                WithName.TypeName,
                WithLifetime.Transient);
            //<IGroupingMethod,>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<IGroupingManager, GroupingManager>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<ICoverMatrixGenerator, CoverMatrixGenerator>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<IFileReaderProvider, FileReaderProvider>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<IValidateService, ValidateService>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<MainViewModel>(new ContainerControlledLifetimeManager());

        }
    }
}
