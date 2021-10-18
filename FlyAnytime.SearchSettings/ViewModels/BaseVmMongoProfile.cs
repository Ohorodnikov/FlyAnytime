using AutoMapper;
using FlyAnytime.Core.AutoMapper;
using FlyAnytime.Core.Web.AutoMapper;
using FlyAnytime.Core.Web.ViewModel;
using FlyAnytime.SearchSettings.MongoDb;
using FlyAnytime.SearchSettings.Repository;
using FlyAnytime.Tools;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.ViewModels
{
    public class BaseMongoVm : BaseViewModel
    {
        public string Id { get; set; }
    }

    public abstract class BaseVmMongoProfile<TEntity, TVm> : BaseVmProfile<TEntity, TVm>
        where TEntity : IMongoEntity
        where TVm : BaseMongoVm
    {
        protected override void MapSource2Destination(IMappingExpression<TEntity, TVm> mappingExpression)
        {
            mappingExpression
                .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id.ToString()))
                ;

            base.MapSource2Destination(mappingExpression);
        }

        protected override void MapDestination2Source(IMappingExpression<TVm, TEntity> mappingExpression)
        {
            var idEntityType = typeof(TEntity).GetProperty(nameof(IMongoEntity.Id)).PropertyType;

            Expression<Func<TVm, object>> idConverter;
            if (idEntityType == typeof(ObjectId))
            {
                idConverter = x => new ObjectId(x.Id);
            }
            else if (idEntityType == typeof(Guid))
            {
                idConverter = x => new Guid(x.Id);
            }
            else
            {
                throw new NotImplementedException();
            }

            mappingExpression
                .ForMember(x => x.Id, opt => opt.MapFrom(idConverter));

            var props = typeof(TEntity).GetAllPropsOfType(typeof(IMongoRootEntity));
            foreach (var prop in props)
            {
                var idPropName = $"{prop.Name}Id";
                var vmPropWithId = typeof(TVm).GetProperty(idPropName);
                mappingExpression
                    .ForMember(prop.Name, opt => opt.Ignore())
                    .ForMember(idPropName, opt => opt.MapFrom((vm, ent) => new ObjectId(vmPropWithId.GetValue(vm).ToString())))
                    ;
            }

            mappingExpression
                .AfterMap<DefaultMongoAfterMapVm2EntityAction<TVm, TEntity>>()
                ;

            base.MapDestination2Source(mappingExpression);
        }
    }

    public class DefaultMongoAfterMapVm2EntityAction<TVm, TEntity> : DefaultAfterMapAction<TVm, TEntity>
        where TEntity : IMongoEntity
        where TVm : BaseViewModel
    {
        private readonly IServiceProvider _serviceProvider;
        public DefaultMongoAfterMapVm2EntityAction(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override void Process(TVm source, TEntity destination, ResolutionContext context)
        {
            var entType = typeof(TEntity);

            var props = entType.GetAllPropsOfType(typeof(IMongoRootEntity));

            foreach (var prop in props)
            {
                var name = prop.Name;
                var idProp = entType.GetProperty($"{name}Id");
                if (idProp == null)
                    continue;

                var idValue = idProp.GetValue(destination);

                var repoType = typeof(IRepository<>).MakeGenericType(prop.PropertyType);
                var repoMethodName = nameof(IRepository<IMongoRootEntity>.GetById);

                var repo = _serviceProvider.GetService(repoType);

                var getByIdMethod = repoType.GetMethod(repoMethodName);

                dynamic task = getByIdMethod.Invoke(repo, new object[] { idValue });

                var res = (IMongoRepoResult)task.GetAwaiter().GetResult(); //typeof(res) == IMongoRepoResult<prop.PropertyType>

                if (res.Success)
                {
                    prop.SetValue(destination, res.Entity);
                }
            }

            base.Process(source, destination, context);
        }
    }
}
