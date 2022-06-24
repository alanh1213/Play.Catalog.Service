using MongoDB.Driver;
using Play.Catalog.Service.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Play.Catalog.Service.Repositories
{
    public class ItemsRepository
    {
        private const string collectionName = "items";  //--> Nombre de la coleccion que contiene los objetos(registros)
        private readonly IMongoCollection<Item> dbCollection;
        private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter;  //--> Filtro para usar en las queries

        public ItemsRepository()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");  //--> Conexion a mongo
            var database = mongoClient.GetDatabase("Catalog");  //--> Obtener base de datos
            dbCollection = database.GetCollection<Item>(collectionName);  //--> Obtener coleccion
        }

        public async Task<IReadOnlyCollection<Item>> GetAllAsync()
        {
            return await dbCollection.Find(filterBuilder.Empty).ToListAsync();
        }

        public async Task<Item> GetAsync(Guid id)
        {
            FilterDefinition<Item> filter = filterBuilder.Eq(item => item.Id, id);

            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Item item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));

            await dbCollection.InsertOneAsync(item);
        }

        public async Task UpdateAsync(Item item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));

            FilterDefinition<Item> filter = filterBuilder.Eq(existingItem => existingItem.Id, item.Id);

            await dbCollection.ReplaceOneAsync(filter, item);
        }

        public async Task RemoveAsync(Guid id)
        {
            FilterDefinition<Item> filter = filterBuilder.Eq(item => item.Id, id);

            await dbCollection.DeleteOneAsync(filter);
        }
    }
}
