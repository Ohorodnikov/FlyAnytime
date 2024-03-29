﻿using FlyAnytime.SearchEngine.EF;
using FlyAnytime.SearchEngine.Engine.ApiRequesters;
using FlyAnytime.SearchEngine.Models.DbModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SearchEngine.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace FlyAnytime.SearchEngine
{
    public class SaveSearchResultBackgroundService : BackgroundService
    {
        private readonly ChannelReader<List<ApiResultModel>> _channel;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly SearchEngineContext _dbContext;
        private readonly IServiceScope _scope;
        public SaveSearchResultBackgroundService(
            ChannelReader<List<ApiResultModel>> channelReader, 
            IServiceScopeFactory serviceScopeFactory)
        {
            _channel = channelReader;
            _serviceScopeFactory = serviceScopeFactory;
            _scope = _serviceScopeFactory.CreateScope();
            _dbContext = _scope.ServiceProvider.GetService<SearchEngineContext>();
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {                
                await foreach (var items in _channel.ReadAllAsync(cancellationToken))
                {
                    try
                    {
                        foreach (var item in items)
                        {
                            var dbModel = new SearchResultItem
                            {
                                Price = item.PriceInEur,

                                ArrivalToDestinationDateTimeUtc = item.FromDateTime.EndUtc,
                                DepartureFromDestinationDateTimeUtc = item.ReturnDateTime.StartUtc,

                                Code = ApiRequestHelper.GenerateRequestGroupName(item.CityCodeFrom, item.CityCodeTo)
                            };

                            _dbContext.Add(dbModel);
                        }
                        
                        await _dbContext.SaveChangesAsync();
                    }
                    catch (Exception e) { }
                }
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _scope.Dispose();
            return base.StopAsync(cancellationToken);
        }
    }
}
