using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using HomelessApi.Repositories;
using HomelessApi.Security;
using HomelessApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using System.Security.Principal;
using Newtonsoft.Json.Serialization;

namespace HomelessApi
{
    public partial class Startup
    {
		readonly MongoUrl mongoUrl;
        readonly IMongoDatabase database;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
			mongoUrl = MongoUrl.Create(Configuration.GetConnectionString("MongoConnectionString"));
            var mongoClient = new MongoClient(mongoUrl);
			database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
			//services.AddMvc();
			//services.AddAuthorization();
			//services.AddDataAnnotations()

			services.AddMvcCore()
                .AddAuthorization()
                .AddDataAnnotations()
                .AddJsonFormatters(options => options.ContractResolver = new CamelCasePropertyNamesContractResolver());

			services.AddSingleton(database);
            
			services.AddSingleton<HomelessRepository>();
			//var a = mongoUrl.ToString();

            //Identity
			services.AddIdentityWithMongoStoresUsingCustomTypes<User, Security.IdentityRole>(mongoUrl.Url).AddDefaultTokenProviders();

			signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("TokenAuthentication:SecretKey").Value));
            var tokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = Configuration.GetSection("TokenAuthentication:Issuer").Value,
                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = Configuration.GetSection("TokenAuthentication:Audience").Value,
                // Validate the token expiry
                ValidateLifetime = true,
                // If you want to allow a certain amount of clock drift, set that here:
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = tokenValidationParameters;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

			ConfigureAuth(app);

            app.UseMvc();
        }
    }

	public partial class Startup
	{
		SymmetricSecurityKey signingKey;


		void ConfigureAuth(IApplicationBuilder app)
        {
            var tokenProviderOptions = new TokenProviderOptions
            {
                Path = Configuration.GetSection("TokenAuthentication:TokenPath").Value,
                Audience = Configuration.GetSection("TokenAuthentication:Audience").Value,
                Issuer = Configuration.GetSection("TokenAuthentication:Issuer").Value,
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
                IdentityResolver = GetIdentity,
                UserIdResolver = GetUserID,
                ClaimsResolver = GetClaims
            };

            app.UseAuthentication();

            app.UseMiddleware<TokenProviderMiddleware>(Options.Create(tokenProviderOptions));
        }

		Task<ClaimsIdentity> GetIdentity(string email, string password)
        {
			var collection = database.GetCollection<User>("users");
            var filter = Builders<User>.Filter.Eq("Email", email);
            var userFound = collection.Find(filter).FirstOrDefault();

            if (userFound != null)
            {
                var passwordHasher = new PasswordHasher<User>();
                var comparison = passwordHasher.VerifyHashedPassword(userFound, userFound.PasswordHash, password);

                if (comparison == PasswordVerificationResult.Success)
                    return Task.FromResult(new ClaimsIdentity(new GenericIdentity(email, "Token"), new Claim[] { }));
            }

            // Account doesn't exists
            return Task.FromResult<ClaimsIdentity>(null);
        }

        string GetUserID(string email)
        {
            var collection = database.GetCollection<User>("users");

            var userFound = collection.Find(x => x.Email == email).FirstOrDefault();

            if (userFound != null)
            {
                return userFound.Id;
            }

            return null;
        }

        List<IdentityUserClaim> GetClaims(string id)
        {
            var collection = database.GetCollection<User>("users");

            var userFound = collection.Find(x => x.Id == id).FirstOrDefault();

            if (userFound != null)
                return userFound.Claims;

            return null;
        }
        
	}

}
