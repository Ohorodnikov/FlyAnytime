using AutoMapper;
using FlyAnytime.Core.AutoMapper;
using FlyAnytime.Core.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Core.Web.AutoMapper
{
    public abstract class BaseVmProfile<TEntity, TVm> : BaseProfile<TEntity, TVm>
        where TVm: BaseViewModel
    {
    }

    public class DefaultAfterMapVm2EntityAction<TVm, TEntity> : DefaultAfterMapAction<TVm, TEntity>
    {
        public override void Process(TVm source, TEntity destination, ResolutionContext context)
        {
            base.Process(source, destination, context);
        }
    }
}
