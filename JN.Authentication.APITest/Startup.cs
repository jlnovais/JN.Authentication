﻿using System;
using System.Text;
using JN.Authentication.APITest.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using JN.Authentication.Scheme;
using JN.Authentication.APITest.Services;
using JN.Authentication.Interfaces;
using Microsoft.Extensions.Hosting;

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
            services.AddControllers();

            var apiKeyAcceptsQueryString = GetAPIKeyAcceptsQueryString();
            var httpPostMethodOnly = GetHttpPostMethodOnly();

            // Basic authentication - using Scheme
            services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
                .AddBasic(options =>
                {
                    options.Realm = "api";
                    options.LogInformation = true; 
                    options.HttpPostMethodOnly = httpPostMethodOnly;
                    options.HeaderEncoding = Encoding.UTF8; 
                    options.ChallengeResponse = ValidationService.ChallengeResponse;

                });

            // Basic authentication - with post only
            services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme + "PostOnly")
                .AddBasic(options =>
                {
                    options.Realm = "api";
                    options.LogInformation = true; 
                    options.HttpPostMethodOnly = true;
                    options.HeaderEncoding = Encoding.UTF8; 
                    options.ChallengeResponse = ValidationService.ChallengeResponse;

                }, "PostOnly");

            services.AddSingleton<IBasicValidationService, BasicValidationService>();

            // END: Basic authentication - using Scheme


            // ApiKey authentication - using Scheme
            services.AddAuthentication(ApiKeyAuthenticationDefaults.AuthenticationScheme)
                .AddApiKey(options =>
                {
                    options.LogInformation = true;
                    options.HttpPostMethodOnly = httpPostMethodOnly;
                    options.AcceptsQueryString = apiKeyAcceptsQueryString;
                    options.ChallengeResponse = ValidationService.ChallengeResponse;

                });

            services.AddAuthentication(ApiKeyAuthenticationDefaults.AuthenticationScheme + "NoGet")
                .AddApiKey(options =>
                {
                    options.LogInformation = true;
                    options.HttpPostMethodOnly = httpPostMethodOnly;
                    options.AcceptsQueryString = false;
                    options.ChallengeResponse = ValidationService.ChallengeResponse;

                }, "NoGet");

            services.AddSingleton<IApiKeyValidationService, ApiKeyValidationService>();

            // END: ApiKey authentication - using Scheme


            /*this is used for Authorization using custom policies*/
            // Add custom authorization handlers
            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsAdminPolicy", policy => policy.Requirements.Add(new CustomRequirement(true)));
                options.AddPolicy("IsNotAdminPolicy", policy => policy.Requirements.Add(new CustomRequirement(false)));
            });

            services.AddSingleton<IAuthorizationHandler, CustomAuthorizationHandler>();
            /*Authorization using custom policies - end*/
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private bool GetAPIKeyAcceptsQueryString()
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
