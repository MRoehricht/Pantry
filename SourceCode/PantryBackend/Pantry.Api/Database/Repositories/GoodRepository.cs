using Microsoft.EntityFrameworkCore;
using Pantry.Api.Database.Contexts;
using Pantry.Api.Database.Entities;
using Pantry.Services.RabbitMqServices;
using Pantry.Shared.Models.MessageModes;
using Pantry.Shared.Models.RecipeModels;
using System;
using System.Text.Json;

namespace Pantry.Api.Database.Repositories {
    public class GoodRepository : IGoodRepository {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        public GoodRepository(ILoggerFactory loggerFactory, IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
            _logger = loggerFactory.CreateLogger<GoodRepository>();
            
        }

        public async Task Create(Ingredient ingredient) 
        {
            if (!ingredient.PantryItemId.HasValue) {
                _logger.LogError("PantryItemId is NULL");
                return;
            }
                
           
            if (ingredient.PantryItemId ==Guid.Empty){
                _logger.LogError("PantryItemId is nich gültig");
                return;
            }

            using IServiceScope scope = _serviceProvider.CreateScope();
            try {
                var context = scope.ServiceProvider.GetRequiredService<PantryContext>();

                var entity = new GoodEntity {
                    Name = ingredient.Name,
                    Id = ingredient.PantryItemId.Value
                };

                context.Goods.Add(entity);
                await context.SaveChangesAsync();


            } catch (InvalidOperationException ex) {
                _logger.LogError($"{ex.Message}");
            } catch (Exception ex) {
                _logger.LogError(ex.Message);
            }
        }
    }
}
