using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Tpk.DataServices.Server.Classes.Impl;
using Tpk.DataServices.Server.Services;
using Tpk.DataServices.Server.Services.Impl;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var allowOrigin = new[]
            {
                "http://localhost:5000", "https://localhost:5001", "http://tpkds.topstargroup.com", "https://tpkds.topstargroup.com"
            };

            // Inject configuration and options
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddSingleton(Configuration);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
            
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins(allowOrigin)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            // .SetPreflightMaxAge(TimeSpan.FromDays(1))
                            .AllowCredentials();
                    });
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Server", Version = "v1" });
            });
            
            services.AddHttpContextAccessor();

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] {"application/octet-stream"});
            });

            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddScoped<IUserPermissionService, UserPermissionService>();

            services.AddScoped<ISystemOptionService, SystemOptionService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<ISectionService, SectionService>();
            services.AddScoped<IUnitService, UnitService>();
            services.AddScoped<IContactTypeService, ContactTypeService>();

            services.AddScoped<IStockLocationService, StockLocationService>();
            services.AddScoped<IStockroomService, StockroomService>();
            services.AddScoped<IStockService, StockService>();
            services.AddScoped<IStockTransactionService, StockTransactionService>();
            services.AddScoped<IStockBookingReferenceService, StockBookingReferenceService>();
            services.AddScoped<IStockBookingDetailReferenceService, StockBookingDetailReferenceService>();
            services.AddScoped<IInventoryRequestService, InventoryRequestService>();
            services.AddScoped<IInventoryRequestLineService, InventoryRequestLineService>();
            services.AddScoped<IInventoryRequestLineDetailService, InventoryRequestLineDetailService>();
            services.AddScoped<ITransportationRequestService, TransportationRequestService>();
            services.AddScoped<ITransportationRequestReferenceService, TransportationRequestReferenceService>();
            services.AddScoped<ITransportationRequestLineService, TransportationRequestLineService>();
            services.AddScoped<ITransportationRequestLineReferenceService, TransportationRequestLineReferenceService>();
            services.AddScoped<ITransportationOrderService, TransportationOrderService>();
            services.AddScoped<ITransportationOrderLineService, TransportationOrderLineService>();

            services.AddScoped<IProductCategoryService, ProductCategoryService>();
            services.AddScoped<IProductLineService, ProductLineService>();
            services.AddScoped<IProductUnitService, ProductUnitService>();
            services.AddScoped<IProductColorService, ProductColorService>();
            services.AddScoped<IMaterialTypeService, MaterialTypeService>();
            services.AddScoped<IMaterialSubTypeService, MaterialSubTypeService>();
            services.AddScoped<IMaterialGradeService, MaterialGradeService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductPackageService, ProductPackageService>();

            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<ISupplierContactService, SupplierContactService>();
            services.AddScoped<ISupplierProductService, SupplierProductService>();
            services.AddScoped<ISupplierProductPackageService, SupplierProductPackageService>();
            services.AddScoped<ISupplierOrderService, SupplierOrderService>();
            services.AddScoped<ISupplierOrderLineService, SupplierOrderLineService>();
            services.AddScoped<ISupplierOrderReferenceService, SupplierOrderReferenceService>();
            services.AddScoped<ISupplierOrderLineReferenceService, SupplierOrderLineReferenceService>();

            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ICustomerContactService, CustomerContactService>();
            services.AddScoped<ICustomerAddressService, CustomerAddressService>();
            services.AddScoped<ICustomerProductService, CustomerProductService>();
            services.AddScoped<ICustomerProductPackageService, CustomerProductPackageService>();
            services.AddScoped<ICustomerOrderService, CustomerOrderService>();
            services.AddScoped<ICustomerOrderLineService, CustomerOrderLineService>();
            services.AddScoped<ICustomerOrderReferenceService, CustomerOrderReferenceService>();
            services.AddScoped<ICustomerOrderLineReferenceService, CustomerOrderLineReferenceService>();

            services.AddScoped<IProductionOrderService, ProductionOrderService>();
            services.AddScoped<IProductionRequestService, ProductionRequestService>();
            services.AddScoped<IProductionOrderReferenceService, ProductionOrderReferenceService>();
            services.AddScoped<IProductionInventoryReferenceService, ProductionInventoryReferenceService>();
            services.AddScoped<IProductionRequestReferenceService, ProductionRequestReferenceService>();
            
            services.AddScoped<ITransportationRequestScheduleService, TransportationRequestScheduleService>();
            services.AddScoped<IProductionOrderScheduleService, ProductionOrderScheduleService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Server v1"));
            }

            app.UseCors();
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<JwtMiddleware>();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
