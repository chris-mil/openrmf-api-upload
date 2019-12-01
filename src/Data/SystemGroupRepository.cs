using openrmf_upload_api.Models;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.Extensions.Options;

namespace openrmf_upload_api.Data {
    public class SystemGroupRepository : ISystemGroupRepository
    {
        private readonly SystemGroupContext _context = null;

        public SystemGroupRepository(IOptions<Settings> settings)
        {
            _context = new SystemGroupContext(settings);
        }

        public async Task<IEnumerable<SystemGroup>> GetAllSystemGroups()
        {
            try
            {
                return await _context.SystemGroups
                        .Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        private ObjectId GetInternalId(string id)
        {
            ObjectId internalId;
            if (!ObjectId.TryParse(id, out internalId))
                internalId = ObjectId.Empty;

            return internalId;
        }
        
        // query after Id or InternalId (BSonId value)
        //
        public async Task<SystemGroup> GetSystemGroup(string id)
        {
            try
            {
                ObjectId internalId = GetInternalId(id);
                return await _context.SystemGroups
                                .Find(SystemGroup => SystemGroup.InternalId == internalId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }
        
        public async Task<SystemGroup> AddSystemGroup(SystemGroup item)
        {
            try
            {
                await _context.SystemGroups.InsertOneAsync(item);
                return item;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<bool> RemoveSystemGroup(string id)
        {
            try
            {
                DeleteResult actionResult 
                    = await _context.SystemGroups.DeleteOneAsync(
                        Builders<SystemGroup>.Filter.Eq("Id", id));

                return actionResult.IsAcknowledged 
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<bool> UpdateSystemGroup(string id, SystemGroup body)
        {
            var filter = Builders<SystemGroup>.Filter.Eq(s => s.InternalId, GetInternalId(id));
            try
            {
                body.InternalId = GetInternalId(id);
                var actionResult = await _context.SystemGroups.ReplaceOneAsync(filter, body);
                return actionResult.IsAcknowledged && actionResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }
    }
}