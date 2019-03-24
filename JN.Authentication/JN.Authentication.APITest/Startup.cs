using System;
using System.Text;
using JN.Authentication.APITest.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using JN.Authentication.Scheme;

namespace JN.Authentication.APITest
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var apiKeyAcceptsQueryString = GetApiKeyAcceptsQueryString();
            var httpPostMethodOnly = GetHttpPostMethodOnly();

            // Basic authentication - using Scheme
            services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
                .AddBasic(options =>
                {
                    options.Realm = "api";
                    options.LogInformation = true; //optional, default is false;
                    options.HttpPostMethodOnly = httpPostMethodOnly;
                    options.HeaderEncoding = Encoding.UTF8; //optional, default is UTF8;
                    options.ValidateUser = ValidationService.ValidateUser;
                    options.ChallengeResponse = ValidationService.ChallengeResponse;
                });
            // END: Basic authentication - using Scheme



            // ApiKey authentication - using Scheme
            services.AddAuthentication(ApiKeyAuthenticationDefaults.AuthenticationScheme)
                .AddApiKey(options =>
                {
                    options.LogInformation = true;
                    options.HttpPostMethodOnly = httpPostMethodOnly;
                    options.AcceptsQueryString = apiKeyAcceptsQueryString;
                    options.HeaderName = "ApiKey";
                    options.ValidateKey = ValidationService.ValidateApiKey;
                    options.ChallengeResponse = ValidationService.ChallengeResponse;

                });

            // END: ApiKey authentication - using Scheme


            /*this is used for Authorization using custom policies*/
            // Add custom authorization handlers
            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsAdminPolicy", policy => policy.Requirements.Add(new CustomRequirement(true)));
                options.AddPolicy("IsNotAdminPolicy", policy => policy.Requirements.Add(new CustomRequirement(false)));
            });

            services.AddSingleton<IAuthorizationHandler,  CustomAuthorizationHandler>();
            /*Authorization using custom policies - end*/


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
            app.UseMvc();
        }

        private bool GetApiKeyAcceptsQueryString()
        {
            var res = true;

            try
            {
                res = string.IsNullOrEmpty(Configuration["ApiKeyAcceptsQueryString"]) || Convert.ToBoolean(Configuration["ApiKeyAcceptsQueryString"]);
            }
            catch
            {
                // ignored
            }

            return res;
        }

        private bool GetHttpPostMethodOnly()
        {
            //HttpPostMethodOnly
            var res = false;

            try
            {
                res = !string.IsNullOrEmpty(Configuration["HttpPostMethodOnly"]) && Convert.ToBoolean(Configuration["HttpPostMethodOnly"]);
            }
            catch
            {
                // ignored
            }

            return res;
        }
    }
}
