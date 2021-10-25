using AutoMapper;
using FlyAnytime.Core.Web;
using FlyAnytime.Core.Web.ViewModel;
using FlyAnytime.Messaging;
using FlyAnytime.SearchSettings.MongoDb;
using FlyAnytime.SearchSettings.Repository;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.Controllers
{
    public class SimpleCrudController<TEntity, TViewModel> : BaseFAController
        where TEntity : IMongoRootEntity
        where TViewModel : BaseViewModel
    {
        private readonly IMessageBus _messageBus;
        private readonly IRepository<TEntity> _repository;
        private readonly IMapper _mapper;
        public SimpleCrudController(
            IMessageBus messageBus,
            IRepository<TEntity> repository,
            IMapper mapper
            )
        {
            _messageBus = messageBus;
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public virtual async Task<IActionResult> GetMany(int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return ErrorResult("Page and Page Size must be greater than 0");
            }

            var res = new
            {
                Total = _repository.Count,
                Items = await _repository.GetNext((page - 1)*pageSize, pageSize)
            };

            return SuccessResult(res);
        }

        [HttpGet]
        public virtual async Task<IActionResult> Get(string id)
        {
            if (!ObjectId.TryParse(id, out var objId))
                return ErrorResult("Id is wrong");

            var getResult = await _repository.GetById(objId);

            return GetImpl(getResult);
        }

        [HttpGet]
        public virtual async Task<IActionResult> GetBy(string propName, string value)
        {
            var getResult = await _repository.GetOneBy(propName, value);

            return GetImpl(getResult);
        }

        private IActionResult GetImpl(IMongoRepoResult<TEntity> getResult)
        {
            if (!getResult.Success)
                return ErrorResult(getResult.ErrorModel);

            var vm = _mapper.Map<TViewModel>(getResult.Entity);

            return SuccessResult(vm);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody]TViewModel model)
        {
            var blModel = _mapper.Map<TEntity>(model);

            var createResult = await _repository.TryCreate(blModel);

            if (!createResult.Success)
                return ErrorResult(createResult.ErrorModel);

            await OnSuccessCreate(blModel);

            var data = new { savedId = blModel.Id.ToString() };

            return SuccessResult(data);
        }

        protected virtual async Task OnSuccessCreate(TEntity entity)
        {
            await Task.CompletedTask;
        }

        [HttpDelete]
        public virtual async Task<IActionResult> Delete(string id)
        {
            if (!ObjectId.TryParse(id, out var objId))
                return ErrorResult("Id is wrong");

            var deleteResult = await _repository.TryDelete(objId);

            if (!deleteResult.Success)
                return ErrorResult(deleteResult.ErrorModel);

            await OnSuccessDelete(deleteResult.Entity);

            return SuccessResult();
        }

        protected virtual async Task OnSuccessDelete(TEntity entity)
        {
            await Task.CompletedTask;
        }

        [HttpPost]
        public virtual async Task<IActionResult> FullUpdate(TViewModel model)
        {
            var blModel = _mapper.Map<TEntity>(model);

            var updateResult = await _repository.TryReplace(blModel);

            if (!updateResult.Success)
                return ErrorResult(updateResult.ErrorModel);

            await OnSuccessUpdate(updateResult.Entity);

            return SuccessResult();
        }

        protected virtual async Task OnSuccessUpdate(TEntity entity)
        {
            await Task.CompletedTask;
        }
    }
}
