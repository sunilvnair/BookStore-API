using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.Mappings;
using BookStore_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BookStore_API
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
			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(
					Configuration.GetConnectionString("DefaultConnection")));
			//services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
			services.AddDefaultIdentity<IdentityUser>()
				.AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>();
			//services.AddRazorPages();

			//Cors ==> shall allow us to access API from any other domain (access persmission granting)
			services.AddCors(o=>{
				o.AddPolicy("CrosPolicy",
					builder => builder.AllowAnyOrigin()
					.AllowAnyMethod()
					.AllowAnyHeader());

			});

			//for activating automapper facility
			services.AddAutoMapper(typeof(Maps));


			//for JWT tocken configuration
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(o =>
				{
					o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						ValidIssuer = Configuration["JWT:Issuer"],
						ValidAudience = Configuration["JWT:Issuer"],
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))

					};
				});

			//Adding swagger service for the automatic document preparation
			services.AddSwaggerGen(c => {
									c.SwaggerDoc("v1.0", new OpenApiInfo
									{
										Title = "Book Store API",
										Version = "v1.0",
										Description = "This is api documention for my first book store app in .net core "
									});
				var xfileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xfilePath = Path.Combine(AppContext.BaseDirectory ,xfileName);
				c.IncludeXmlComments(xfilePath);
								});
			//to include logging interface and service into project
			services.AddSingleton<ILoggerService,LoggerService>();
			//to include author repository servis into scope
			services.AddScoped <IAuthorRepository ,AuthorRepository>()  ;
			//to include book repository servis into scope
			services.AddScoped <IBookRepository ,BookRepository>()  ;

	

			//keep AddControllers as the last services ...
			services.AddControllers();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
			UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			app.UseSwagger();
			app.UseSwaggerUI( c=> {
				c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "Book Store API");
				c.RoutePrefix = "";
			});


			app.UseHttpsRedirection();
			//	app.UseStaticFiles();

			//cors policy name from ConfigureServices
			app.UseCors("CrosPolicy");
			app.UseRouting();

			//to seed initial user and roles into the db if not exists
			SeedData.seed(userManager, roleManager).Wait();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
