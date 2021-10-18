using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Core.AutoMapper
{
    public abstract class BaseProfile<TModel1, TModel2> : Profile
    {
        public BaseProfile()
        {
            var map1 = CreateMap<TModel1, TModel2>();
            var map2 = CreateMap<TModel2, TModel1>();

            MapSource2Destination(map1);
            MapDestination2Source(map2);
        }

        protected virtual void MapSource2Destination(IMappingExpression<TModel1, TModel2> mappingExpression)
        {

        }

        protected virtual void MapDestination2Source(IMappingExpression<TModel2, TModel1> mappingExpression)
        {

        }
    }

    public class DefaultAfterMapAction<TFrom, TTo> : IMappingAction<TFrom, TTo>
    {
        public virtual void Process(TFrom source, TTo destination, ResolutionContext context)
        {
            
        }
    }
}
