using System;
using BusinessLogicLayer.AttributeData;
using BusinessLogicLayer.Data;
using BusinessLogicLayer.ExcelWriter;
using BusinessLogicLayer.Matrix;
using BusinessLogicLayer.Matrix.CoverTools;
using BusinessLogicLayer.Matrix.CoverTools.GroupingManager;
using BusinessLogicLayer.Reader;
using BusinessLogicLayer.TrainingObjectsProceder;
using Core.Common.Interfaces;
using Core.Common.Interfaces.Excell;
using Core.Common.Interfaces.GroupingManager;
using GroupingAndCoveringDataApi.ActionFilters;
using GroupingAndCoveringDataApi.Controllers;
using GroupingAndCoveringDataApi.Provider;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;

namespace GroupingAndCoveringDataApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //            return;

            services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);

            services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.AllowAnyOrigin();
                builder.AllowCredentials();
            }));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //services.AddSingleton<IFileReaderProvider, FileReaderProvider>();
            //services.AddSingleton<ICoverMatrixManager, CoverMatrixManager>();
            //services.AddSingleton<ITxtExporter, TxtExporter>();
            //services.AddSingleton<IFileReaderProvider, FileReaderProvider>();
            //services.AddScoped<ISaveFileDialog, SaveFileDialogWrapper>();
            //services.AddScoped<IOpenFileDialog, OpenFileDialogWrapper>();

            services.AddTransient<ModelValidationAttribute>();
            services.AddTransient(typeof(Lazy<>), typeof(Lazier<>));

            services.AddTransient<CoverManagerAsync, CoverManagerAsync>();

            services.AddTransient<IGroupingMethod, GroupingByElements>();
            services.AddTransient<IGroupingMethod, GroupingByPercents>();
            services.AddTransient<IAttributeColumnConverter, AttributeColumnConverter>();
            services.AddTransient<IDataReader, DataReader>();
            services.AddTransient<IFileReader, FileReader>();
            services.AddTransient<IFileChecker, FileChecker>();
            services.AddTransient<ITrainingObjectsConverter, TrainingObjectsConverter>();
                     
            services.AddTransient<IPredictionMatrixWriter, PredictionMatrixWriter>();
            services.AddTransient<IAttributeWriter, AttributeWriter>();
                     
            services.AddTransient<IClassViewer, ClassViewer>();
            services.AddTransient<IDataObjectsConverter, DataObjectsConverter>();
            services.AddTransient<IMatrixToGridMatrix, MatrixToGridMatrix>();
            services.AddTransient<ICoverMatrixClassificator, CoverMatrixClassificator>();
            services.AddTransient<ICoverMatrixManager, CoverMatrixManager>();
            services.AddTransient<ICoverCalculator, CoverCalculator>();
            services.AddTransient<ICoverGradeService, CoverGradeService>();
            services.AddTransient<IGroupingManager, GroupingManager>();
            services.AddTransient<ICoverMatrixGenerator, CoverMatrixGenerator>();
            services.AddTransient<IFileReaderProvider, FileReaderProvider>();
            services.AddTransient<ITxtExporter, TxtExporter>();
            services.AddTransient<IExcelWriter, ExcelWriter>();
            //services.AddMvcCore().AddRazorViewEngine();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
                c.SchemaRegistryOptions.CustomTypeMappings.Add(typeof(IFormFile), () => new Schema() { Type = "file", Format = "binary" });
            });

            services.AddMvc()
                .AddJsonOptions(options => { options.SerializerSettings.Formatting = Formatting.Indented; })
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseExceptionHandler(config =>
            {
                config.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    var error = context.Features.Get<IExceptionHandlerFeature>();
                    if (error != null)
                    {
                        var ex = error.Error;
                        await context.Response.WriteAsync(ex.Message);
                        //{
                        //    StatusCode = 500,
                        //    //ErrorMessage = ex.Message
                        //}.ToString();
                    }
                });
            });


            app.UseMvc();
        }
    }
}
